using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

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
            GetDependenciesInternal(mi, result);
            return result.ToList();
        }

        public List<Type> GetDependencies(MethodBase mi)
        {
            var result = new HashSet<Type>();
            GetDependenciesInternal(mi, result);
            return result.ToList();
        }

        private void GetDependenciesInternal(MethodBase mi, ISet<Type> dependencies)
        {
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
                    GetDependenciesInternal(resMethod, dependencies);
                }
                else
                if (resMethod.DeclaringType != null)
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
}