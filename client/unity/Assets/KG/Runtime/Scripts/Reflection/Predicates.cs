using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KG.Reflection
{
    public class Predicates
    {
        public static bool MatchAttrAndParam(MethodInfo method, Type attribute, IReadOnlyList<ParamOption> paramOptions)
        {
            if (!attribute.IsSubclassOf(typeof(Attribute)))
                throw new ArgumentException();
            
            if (method.CustomAttributes.All(attr => attr.AttributeType != attribute))
                return false;
                
            if (method.ReturnType != typeof(void))
                return false;

            return MatchRequiredParams(method.GetParameters(), paramOptions);
        }

        public static bool MatchRequiredParams(ParameterInfo[] methodParams, IReadOnlyList<ParamOption> paramOptions)
        {
            foreach (var invokeParam in paramOptions.Where(x => x.required))
            {
                if (methodParams.Any(x => MatchParam(x, invokeParam)))
                    continue;

                return false;
            }

            return true;
        }

        public static bool MatchParam(ParameterInfo parameterInfo, ParamOption paramOptions)
        {
            if (!string.IsNullOrEmpty(paramOptions.name))
                return parameterInfo.Name == paramOptions.name; 
            
            if (paramOptions.acceptSubclass && parameterInfo.ParameterType.IsSubclassOf(paramOptions.type))
                return true;

            if (paramOptions.acceptSelf && (parameterInfo.ParameterType == paramOptions.type))
                return true;

            return false;
        }
    }
}