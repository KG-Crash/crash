using System;
using System.Linq;
using System.Reflection;

namespace KG
{
    public static class MethodExtractor
    {
        public static MethodInfo[] ExtractAsAttribute<T, ATTR>() where T : class where ATTR : Attribute
        {
            return ExtractAsAttribute<ATTR>(typeof(T));
        }

        public static MethodInfo[] ExtractAsAttribute<T, ATTR, P0>() where T : class where ATTR : Attribute
        {
            return ExtractAsAttribute<ATTR>(typeof(T));
        }
        
        public static MethodInfo[] ExtractAsAttribute<T, ATTR, P0, P1>() where T : class where ATTR : Attribute
        {
            return ExtractAsAttribute<ATTR>(typeof(T));
        }

        public static MethodInfo[] ExtractAsAttribute<T>(Type type) where T : Attribute
        {
            return type.GetMethods()
                .Where(x => x.GetCustomAttribute<T>() != null)
                .ToArray();
        }
    }
}