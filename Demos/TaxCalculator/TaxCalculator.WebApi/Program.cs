using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace TaxCalculator.WebApi
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var mi = typeof(Test).GetMethods()[0];
            Console.WriteLine(Parser.DumpMethod(mi));
            //CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }

    public class Test
    {
        public void Do1()
        {
            Console.WriteLine(1);
            Do2();
        }

        public void Do2()
        {
            Console.WriteLine(1);
        }

        public class Data
        {
            public int A { get; set; }
            public int B { get; set; }
        }
        // Entry point -> serialized
        // A = 10, B = 10
        public void  D1(Data input)
        {
            // A = 5, B = 20
            var d2 = new Data()
            {
                A = input.A / 2,
                B = input.B * 2,
            };
            D2(d2);
            
            // A = 15, B = 13
            Console.WriteLine(d2.A);
            Console.WriteLine(d2.B);
            // File.WriteAllLines(d2)
        }

        // Non deterministic -> serialized
        // A = 5, B = 20
        public void D2(Data input)
        {
            // A = 15, B = 13
            input.A += 10;
            input.B -= DateTime.Now.Millisecond%7;
            // something else non determ
        }
        
    }

    public static class Parser
    {
        public static string DumpMethod(MethodInfo mi)
        {
            // For aggregating our response
            StringBuilder sb = new StringBuilder();

            // First we need to extract out the raw IL
            var mb = mi.GetMethodBody();
            var il = mb.GetILAsByteArray();

            // We'll also need a full set of the IL opcodes so we
            // can remap them over our method body
            var opCodes = typeof(System.Reflection.Emit.OpCodes)
                .GetFields()
                .Select(fi => (System.Reflection.Emit.OpCode)fi.GetValue(null));

            //opCodes.Dump();

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
                    // For operand inference:
                    // MOST operands are 32 bit, 
                    // so we'll start there
                    var byteCount = 4;
                    long operand = 0;
                    string token = string.Empty;

                    // For metadata token resolution            
                    var module = mi.Module;
                    Func<int, string> tokenResolver = tkn => string.Empty;
                    switch (mappedOp.OperandType)
                    {
                        // These are all 32bit metadata tokens
                        case OperandType.InlineMethod:
                            tokenResolver = tkn =>
                            {
                                var resMethod = module.SafeResolveMethod((int)tkn);
                                Console.WriteLine(resMethod.DeclaringType.FullName);
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
                                var catSig = string
                                    .Join(",", sigBytes);
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
                                return string.Format("(typeof({0}))", type == null ? "unknown" : type.FullName);
                            };
                            break;
                        // These are plain old 32bit operands
                        case OperandType.InlineI:
                        case OperandType.InlineBrTarget:
                        case OperandType.InlineSwitch:
                        case OperandType.ShortInlineR:
                            break;
                        // These are 64bit operands
                        case OperandType.InlineI8:
                        case OperandType.InlineR:
                            byteCount = 8;
                            break;
                        // These are all 8bit values
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