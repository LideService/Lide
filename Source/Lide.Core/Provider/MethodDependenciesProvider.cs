using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Lide.Core.Contract;

namespace Lide.Core.Provider
{
    public class MethodDependenciesProvider : IMethodDependenciesProvider
    {
        private readonly List<OpCode> _opCodes;

        public MethodDependenciesProvider()
        {
            var opCodeFields = typeof(OpCodes).GetFields();
            _opCodes = new List<OpCode>();
            foreach (var opCodeField in opCodeFields)
            {
                var value = opCodeField.GetValue(null);
                _opCodes.Add((OpCode)value!);
            }
        }

        public List<MemberInfo> GetDependentMembers(MethodInfo mi)
        {
            var result = new HashSet<MemberInfo>();
            GetDependenciesInternal(mi, result);
            return result.ToList();
        }

        public List<Type> GetDependencies(MethodInfo mi)
        {
            var result = new HashSet<MemberInfo>();
            GetDependenciesInternal(mi, result);
            return result.Select(x => x.DeclaringType).ToHashSet().ToList();
        }

        public List<Type> GetDependencies(MethodBase mi)
        {
            var result = new HashSet<MemberInfo>();
            GetDependenciesInternal(mi, result);
            return result.Select(x => x.DeclaringType).ToHashSet().ToList();
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "It is what it is")]
        private void GetDependenciesInternal(MethodBase mi, ISet<MemberInfo> dependencies)
        {
            var mb = mi?.GetMethodBody();
            var il = mb?.GetILAsByteArray();
            var pos = 0;

            if (mi == null || mb == null || il == null || il.Length == 0)
            {
                return;
            }

            var methodGenericArguments = mi.GetGenericArguments();
            var typeGenericArguments = mi.DeclaringType?.GetGenericArguments();

            byte ReadByte() => il[pos++];
            short ReadInt16() => (short)(il[pos++] | il[pos++] << 8);
            int ReadInt32() => il[pos++] | il[pos++] << 8 | il[pos++] << 16 | il[pos++] << 24;
            long ReadInt64() => (long)ReadInt32() | (long)ReadInt32() << 32;
            double ReadSingle() => BitConverter.IsLittleEndian ? BitConverter.ToSingle(ReadArray(4)) : BitConverter.ToSingle(ReadArray(4).Reverse().ToArray());
            double ReadDouble() => BitConverter.IsLittleEndian ? BitConverter.ToDouble(ReadArray(8)) : BitConverter.ToDouble(ReadArray(8).Reverse().ToArray());
            byte[] ReadArray(int len)
            {
                var result = il[pos..len].ToArray();
                pos += len;
                return result;
            }

            while (true)
            {
                if (pos >= il.Length - 1)
                {
                    break;
                }

                var op = (short)il[pos++];
                if (op == 0xFE)
                {
                    op = (short)(op << 8 | il[pos++]);
                }

                var opCode = _opCodes.FirstOrDefault(x => x.Value == op);
                switch (opCode.OperandType)
                {
                    case OperandType.InlineBrTarget:
                        _ = ReadInt32() + pos;
                        break;
                    case OperandType.InlineField:
                    case OperandType.InlineMethod:
                    case OperandType.InlineTok:
                    case OperandType.InlineType:
                        var member = mi.Module.ResolveMember(ReadInt32(), typeGenericArguments, methodGenericArguments);
                        dependencies.Add(member);
                        break;
                    case OperandType.InlineI:
                        _ = ReadInt32();
                        break;
                    case OperandType.InlineI8:
                        _ = ReadInt64();
                        break;
                    case OperandType.InlineNone:
                        break;
                    case OperandType.InlineR:
                        _ = ReadDouble();
                        break;
                    case OperandType.InlineSig:
                        _ = (object)mi.Module.ResolveSignature(ReadInt32());
                        break;
                    case OperandType.InlineString:
                        _ = (object)mi.Module.ResolveString(ReadInt32());
                        break;
                    case OperandType.InlineSwitch:
                        var length = ReadInt32();
                        var num = pos + 4 * length;
                        var numArray = new int[length];
                        for (var index = 0; index < length; ++index)
                        {
                            numArray[index] = ReadInt32() + num;
                        }

                        _ = (object)numArray;
                        break;
                    case OperandType.InlineVar:
                        _ = ReadInt16();
                        break;
                    case OperandType.ShortInlineBrTarget:
                        _ = ReadByte() + pos;
                        break;
                    case OperandType.ShortInlineI:
                        _ = ReadByte();
                        break;
                    case OperandType.ShortInlineR:
                        _ = ReadSingle();
                        break;
                    case OperandType.ShortInlineVar:
                        _ = ReadByte();
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}