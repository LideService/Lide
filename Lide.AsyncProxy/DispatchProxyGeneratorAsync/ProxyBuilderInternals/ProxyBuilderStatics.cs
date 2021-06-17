using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;

namespace Lide.AsyncProxy.DispatchProxyGeneratorAsync.ProxyBuilderInternals
{
    internal static class ProxyBuilderStatics
    {
        private static readonly (OpCode convCode, OpCode ldindCode, OpCode stindCode) EnumTypes =
            (OpCodes.Nop, OpCodes.Ldind_Ref, OpCodes.Stind_Ref);

        private static readonly (OpCode convCode, OpCode ldindCode, OpCode stindCode) NullType =
            (OpCodes.Nop, OpCodes.Nop, OpCodes.Nop);

        private static readonly Dictionary<Type, (OpCode convCode, OpCode ldindCode, OpCode stindCode)> TypeCodes =
            new ()
        {
            { typeof(object), (OpCodes.Nop, OpCodes.Nop, OpCodes.Nop) },
            { typeof(DBNull), (OpCodes.Nop, OpCodes.Nop, OpCodes.Nop) },
            { typeof(bool), (OpCodes.Conv_I1, OpCodes.Ldind_I1, OpCodes.Stind_I1) },
            { typeof(char), (OpCodes.Conv_I2, OpCodes.Ldind_I2, OpCodes.Stind_I2) },
            { typeof(sbyte), (OpCodes.Conv_I1, OpCodes.Ldind_I1, OpCodes.Stind_I1) },
            { typeof(byte), (OpCodes.Conv_U1, OpCodes.Ldind_U1, OpCodes.Stind_I1) },
            { typeof(short), (OpCodes.Conv_I2, OpCodes.Ldind_I2, OpCodes.Stind_I2) },
            { typeof(ushort), (OpCodes.Conv_U2, OpCodes.Ldind_U2, OpCodes.Stind_I2) },
            { typeof(int), (OpCodes.Conv_I4, OpCodes.Ldind_I4, OpCodes.Stind_I4) },
            { typeof(uint), (OpCodes.Conv_U4, OpCodes.Ldind_U4, OpCodes.Stind_I4) },
            { typeof(long), (OpCodes.Conv_I8, OpCodes.Ldind_I8, OpCodes.Stind_I8) },
            { typeof(ulong), (OpCodes.Conv_U8, OpCodes.Ldind_I8, OpCodes.Stind_I8) },
            { typeof(float), (OpCodes.Conv_R4, OpCodes.Ldind_R4, OpCodes.Stind_R4) },
            { typeof(double), (OpCodes.Conv_R8, OpCodes.Ldind_R8, OpCodes.Stind_R8) },
            { typeof(decimal), (OpCodes.Nop, OpCodes.Nop, OpCodes.Nop) },
            { typeof(DateTime), (OpCodes.Nop, OpCodes.Nop, OpCodes.Nop) },
            { typeof(string), (OpCodes.Nop, OpCodes.Nop, OpCodes.Nop) },
        };

        public static bool IsGenericTask(Type type)
        {
            var current = type;
            while (current != null)
            {
                if (current.GetTypeInfo().IsGenericType && current.GetGenericTypeDefinition() == typeof(Task<>))
                {
                    return true;
                }

                current = current.GetTypeInfo().BaseType;
            }

            return false;
        }

        public static Type[] ParamTypes(ParameterInfo[] parameterInfos, bool noByRef)
        {
            Type[] types = new Type[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                types[i] = parameterInfos[i].ParameterType;
                if (noByRef && types[i].IsByRef)
                {
                    types[i] = types[i].GetElementType();
                }
            }

            return types;
        }

        public static void Convert(ILGenerator ilGenerator, Type sourceType, Type targetType, bool isAddress)
        {
            if (targetType == sourceType)
            {
                return;
            }

            TypeInfo sourceTypeInfo = sourceType.GetTypeInfo();
            TypeInfo targetTypeInfo = targetType.GetTypeInfo();

            if (sourceType.IsByRef)
            {
                Type argType = sourceType.GetElementType();
                Ldind(ilGenerator, argType);
                Convert(ilGenerator, argType, targetType, isAddress);
                return;
            }

            if (targetTypeInfo.IsValueType)
            {
                if (sourceTypeInfo.IsValueType)
                {
                    OpCode opCode = GetTypeCodes(targetType).convCode;
                    ilGenerator.Emit(opCode);
                }
                else
                {
                    ilGenerator.Emit(OpCodes.Unbox, targetType);
                    if (!isAddress)
                    {
                        Ldind(ilGenerator, targetType);
                    }
                }
            }
            else if (targetTypeInfo.IsAssignableFrom(sourceTypeInfo))
            {
                if (sourceTypeInfo.IsValueType || sourceType.IsGenericParameter)
                {
                    if (isAddress)
                    {
                        Ldind(ilGenerator, sourceType);
                    }

                    ilGenerator.Emit(OpCodes.Box, sourceType);
                }
            }
            else
            {
                ilGenerator.Emit(targetType.IsGenericParameter ? OpCodes.Unbox_Any : OpCodes.Castclass, targetType);
            }
        }

        public static void Stind(ILGenerator il, Type type)
        {
            OpCode opCode = GetTypeCodes(type).stindCode;
            if (!opCode.Equals(OpCodes.Nop))
            {
                il.Emit(opCode);
            }
            else
            {
                il.Emit(OpCodes.Stobj, type);
            }
        }

        private static (OpCode convCode, OpCode ldindCode, OpCode stindCode) GetTypeCodes(Type type)
        {
            if (type == null)
            {
                return NullType;
            }

            if (TypeCodes.ContainsKey(type))
            {
                return TypeCodes[type];
            }

            return EnumTypes;
        }

        private static void Ldind(ILGenerator il, Type type)
        {
            OpCode opCode = GetTypeCodes(type).ldindCode;
            if (!opCode.Equals(OpCodes.Nop))
            {
                il.Emit(opCode);
            }
            else
            {
                il.Emit(OpCodes.Ldobj, type);
            }
        }
    }
}