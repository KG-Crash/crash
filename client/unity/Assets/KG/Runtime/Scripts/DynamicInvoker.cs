using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KG
{
    [Flags]
    public enum MatchParamFlag
    {
        Self = 0x01,
        SubClass = 0x02,
        All = 0x04,
    }
    
    public class ParamOption
    {
        public Type type;
        public MatchParamFlag acceptFlag;
        public bool required;
        public string name;

        public void SetFlag(bool on, MatchParamFlag flag)
        {
            if (on)
                acceptFlag |= flag;
            else
                acceptFlag &= ~flag;
        }

        public bool acceptSelf
        {
            get => (acceptFlag & MatchParamFlag.Self) > 0;
            set => SetFlag(value, MatchParamFlag.Self);
        } 

        public bool acceptSubclass
        {
            get => (acceptFlag & MatchParamFlag.SubClass) > 0;
            set => SetFlag(value, MatchParamFlag.SubClass);
        }
    }
    
    public static class DynamicInvoker
    {
        public static bool MatchAttrAndParam(MethodInfo method, Type attribute, IReadOnlyList<ParamOption> paramOptions)
        {
            if (!attribute.IsSubclassOf(typeof(System.Attribute)))
                throw new ArgumentException();
            
            if (method.CustomAttributes.All(attr => attr.AttributeType != attribute))
                return false;
                
            if (method.ReturnType != typeof(void))
                return false;

            return MatchRequiredParams(method.GetParameters(), paramOptions);
        }

        private static bool MatchRequiredParams(ParameterInfo[] methodParams, IReadOnlyList<ParamOption> paramOptions)
        {
            foreach (var invokeParam in paramOptions.Where(x => x.required))
            {
                if (methodParams.Any(x => MatchParam(x, invokeParam)))
                    continue;

                return false;
            }

            return true;
        }

        private static bool MatchParam(ParameterInfo parameterInfo, ParamOption paramOptions)
        {
            if (!string.IsNullOrEmpty(paramOptions.name))
                return parameterInfo.Name == paramOptions.name; 
            
            if (paramOptions.acceptSubclass && parameterInfo.ParameterType.IsSubclassOf(paramOptions.type))
                return true;

            if (paramOptions.acceptSelf && (parameterInfo.ParameterType == paramOptions.type))
                return true;

            return false;
        }

        public static void Invoke(object obj, MethodInfo info, 
            object[] expectedParameters, IReadOnlyList<ParamOption> paramOptions, Func<int, object[]> allocator)
        {
            var methodParams = info.GetParameters();
            if (!MatchRequiredParams(methodParams, paramOptions))
                throw new ArgumentException("");

            var count = GetMatchParamCount(methodParams, paramOptions);
            var paramArray = allocator(count);
            BuildParam(paramArray, methodParams, expectedParameters, paramOptions);
            
            info.Invoke(obj, paramArray);
        }

        private static int GetMatchParamCount(ParameterInfo[] methodParams, IReadOnlyList<ParamOption> paramOptions)
        {
            return paramOptions.Count(x => methodParams.Any(y => MatchParam(y, x)));
        }

        private static void BuildParam(object[] paramArray, ParameterInfo[] methodParams,
            IReadOnlyList<object> parameters, IReadOnlyList<ParamOption> paramOptions)
        {
            // 같은게 있으면 덮어씌움
            for (var optionIndex = paramOptions.Count - 1; optionIndex >= 0; optionIndex--)
            {
                var paramOption = paramOptions[optionIndex];
                var index = Array.FindIndex(methodParams, x => MatchParam(x, paramOption)); 
                if (index < 0) continue;

                paramArray[index] = parameters[optionIndex];
            }
        }
    }
}