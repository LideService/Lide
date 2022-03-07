using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Lide.Core.Provider
{
    public class MethodDependencies
    {
        private readonly List<OpCode> _opCodes;

        public MethodDependencies()
        {
            var opCodeFields = typeof(OpCodes).GetFields();
            _opCodes = new List<OpCode>();
            foreach (var opCodeField in opCodeFields)
            {
                var value = opCodeField.GetValue(null);
                _opCodes.Add((OpCode)value!);
            }
        }

        public List<Type> GetDependencies(MethodInfo mi)
        {
            var result = new HashSet<Type>();
            GetDependenciesInternal(mi, result, 0);
            return result.ToList();
        }

        public List<Type> GetDependencies(MethodBase mi)
        {
            var result = new HashSet<Type>();
            GetDependenciesInternal(mi, result, 0);
            return result.ToList();
        }

        private void GetDependenciesInternal(MethodBase mi, ISet<Type> dependencies, int level)
        {
            if (level > 10)
            {
                return;
            }

            var mb = mi?.GetMethodBody();
            var il = mb?.GetILAsByteArray();

            if (mi == null || mb == null || il == null)
            {
                return;
            }

            var mappedIl = il.Select(op => _opCodes.FirstOrDefault(opCode => opCode.Value == op)).ToList();
            for (var i = 0; i < mappedIl.Capacity; i++)
            {
                if (mappedIl[i].OperandType != OperandType.InlineMethod)
                {
                    continue;
                }

                int operand = mappedIl[++i].Value;
                operand |= mappedIl[++i].Value << 8;
                operand |= mappedIl[++i].Value << 16;
                operand |= mappedIl[++i].Value << 24;
                var resMethod = TryResolveMethod(mi.Module, operand);
                if (resMethod == null)
                {
                    continue;
                }

                if (resMethod.DeclaringType == mi.DeclaringType)
                {
                    GetDependenciesInternal(resMethod, dependencies, level + 1);
                }
                else if (resMethod.DeclaringType != null)
                {
                    dependencies.Add(resMethod.DeclaringType);
                }
            }
        }

        private static MethodBase TryResolveMethod(Module m, int token)
        {
            try
            {
                return m.ResolveMethod(token);
            }
            catch
            {
                return null;
            }
        }
    }

    public static class OriginalIL
    {
        public static string DumpMethod(MethodInfo mi)
        {
            // For aggregating our response
            StringBuilder sb = new StringBuilder();

            // First we need to extract out the raw IL
            var mb = mi.GetMethodBody();
            var il = mb?.GetILAsByteArray();

            // We'll also need a full set of the IL opcodes so we
            // can remap them over our method body
            var opCodes = typeof(System.Reflection.Emit.OpCodes)
                .GetFields()
                .Select(fi => (System.Reflection.Emit.OpCode)fi.GetValue(null));

            // For each byte in our method body, try to match it to an opcode
            var mappedIL = il.Select(op =>
                opCodes.FirstOrDefault(opCode => opCode.Value == op));

            // OpCode/Operand parsing:
            //     Some opcodes have no operands, some use ints, etc.
            //  let's try to cover all cases
            var ilWalker = mappedIL.GetEnumerator();
            while (ilWalker.MoveNext())
            {
                var mappedOp = ilWalker.Current;
                if (mappedOp.OperandType != OperandType.InlineNone)
                {
                    var byteCount = 4;
                    long operand = 0;
                    string token = string.Empty;

                    var module = mi.Module;
                    Func<int, string> tokenResolver = tkn => string.Empty;
                    switch (mappedOp.OperandType)
                    {
                        // These are all 32bit metadata tokens
                        case OperandType.InlineMethod:
                            tokenResolver = tkn =>
                            {
                                var resMethod = module.SafeResolveMethod((int)tkn);
                                return string.Format("({0}())", resMethod == null ? "unknown" : resMethod.Name);
                            };
                            break;
                        case OperandType.InlineField:
                            tokenResolver = tkn =>
                            {
                                var field = module.SafeResolveField((int)tkn);
                                return string.Format("({0})", field == null ? "unknown" : field.Name);
                            };
                            break;
                        case OperandType.InlineSig:
                            tokenResolver = tkn =>
                            {
                                var sigBytes = module.SafeResolveSignature((int)tkn);
                                var catSig = string.Join(",", sigBytes);
                                return string.Format("(SIG:{0})", catSig == null ? "unknown" : catSig);
                            };
                            break;
                        case OperandType.InlineString:
                            tokenResolver = tkn =>
                            {
                                var str = module.SafeResolveString((int)tkn);
                                return string.Format("('{0}')", str == null ? "unknown" : str);
                            };
                            break;
                        case OperandType.InlineType:
                            tokenResolver = tkn =>
                            {
                                var type = module.SafeResolveType((int)tkn);
                                return string.Format("(typeof({0}))", type == null ? "unknown" : type.Name);
                            };
                            break;
                        case OperandType.InlineI:
                        case OperandType.InlineBrTarget:
                        case OperandType.InlineSwitch:
                        case OperandType.ShortInlineR:
                            break;
                        case OperandType.InlineI8:
                        case OperandType.InlineR:
                            byteCount = 8;
                            break;
                        case OperandType.ShortInlineBrTarget:
                        case OperandType.ShortInlineI:
                        case OperandType.ShortInlineVar:
                            byteCount = 1;
                            break;
                    }

                    // Based on byte count, pull out the full operand
                    for (int i = 0; i < byteCount; i++)
                    {
                        ilWalker.MoveNext();
                        operand |= ((long)ilWalker.Current.Value) << (8 * i);
                    }

                    var resolved = tokenResolver((int)operand);
                    resolved = string.IsNullOrEmpty(resolved) ? operand.ToString() : resolved;
                    sb.AppendFormat("{0} {1}",
                            mappedOp.Name,
                            resolved)
                        .AppendLine();
                }
                else
                {
                    sb.AppendLine(mappedOp.Name);
                }
            }

            return sb.ToString();
        }
    }

    public static class Ext
    {
        public static FieldInfo SafeResolveField(this Module m, int token)
        {
            FieldInfo fi;
            m.TryResolveField(token, out fi);
            return fi;
        }

        public static bool TryResolveField(this Module m, int token, out FieldInfo fi)
        {
            var ok = false;
            try
            {
                fi = m.ResolveField(token);
                ok = true;
            }
            catch
            {
                fi = null;
            }

            return ok;
        }

        public static MethodBase SafeResolveMethod(this Module m, int token)
        {
            MethodBase fi;
            m.TryResolveMethod(token, out fi);
            return fi;
        }

        public static bool TryResolveMethod(this Module m, int token, out MethodBase fi)
        {
            var ok = false;
            try
            {
                fi = m.ResolveMethod(token);
                ok = true;
            }
            catch
            {
                fi = null;
            }

            return ok;
        }

        public static string SafeResolveString(this Module m, int token)
        {
            string fi;
            m.TryResolveString(token, out fi);
            return fi;
        }

        public static bool TryResolveString(this Module m, int token, out string fi)
        {
            var ok = false;
            try
            {
                fi = m.ResolveString(token);
                ok = true;
            }
            catch
            {
                fi = null;
            }

            return ok;
        }

        public static byte[] SafeResolveSignature(this Module m, int token)
        {
            byte[] fi;
            m.TryResolveSignature(token, out fi);
            return fi;
        }

        public static bool TryResolveSignature(this Module m, int token, out byte[] fi)
        {
            var ok = false;
            try
            {
                fi = m.ResolveSignature(token);
                ok = true;
            }
            catch
            {
                fi = null;
            }

            return ok;
        }

        public static Type SafeResolveType(this Module m, int token)
        {
            Type fi;
            m.TryResolveType(token, out fi);
            return fi;
        }

        public static bool TryResolveType(this Module m, int token, out Type fi)
        {
            var ok = false;
            try
            {
                fi = m.ResolveType(token);
                ok = true;
            }
            catch
            {
                fi = null;
            }

            return ok;
        }
    }
}