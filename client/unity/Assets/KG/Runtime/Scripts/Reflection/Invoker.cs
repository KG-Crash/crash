using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KG.Reflection
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
    
    public static class Invoker
    {
        public static void Invoke(object obj, MethodInfo info, 
            object[] expectedParameters, IReadOnlyList<ParamOption> paramOptions, Func<int, object[]> allocator)
        {
            var methodParams = info.GetParameters();
            if (!Predicates.MatchRequiredParams(methodParams, paramOptions))
                throw new ArgumentException("");

            var count = GetMatchParamCount(methodParams, paramOptions);
            var paramArray = allocator(count);
            BuildParam(paramArray, methodParams, expectedParameters, paramOptions);
            
            info.Invoke(obj, paramArray);
        }

        private static int GetMatchParamCount(ParameterInfo[] methodParams, IReadOnlyList<ParamOption> paramOptions)
        {
            return paramOptions.Count(x => methodParams.Any(y => Predicates.MatchParam(y, x)));
        }

        private static void BuildParam(object[] paramArray, ParameterInfo[] methodParams,
            IReadOnlyList<object> parameters, IReadOnlyList<ParamOption> paramOptions)
        {
            // 같은게 있으면 덮어씌움
            for (var optionIndex = paramOptions.Count - 1; optionIndex >= 0; optionIndex--)
            {
                var paramOption = paramOptions[optionIndex];
                var index = Array.FindIndex(methodParams, x => Predicates.MatchParam(x, paramOption)); 
                if (index < 0) continue;

                paramArray[index] = parameters[optionIndex];
            }
        }
    }
}