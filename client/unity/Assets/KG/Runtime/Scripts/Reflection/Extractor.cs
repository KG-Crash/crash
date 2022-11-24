using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KG.Reflection
{
    public static class Extractor
    {
        private static bool Always(MethodInfo _) => true;
        
        public static MethodInfo[] ExtractMethodAsAttr<T, ATTR>(BindingFlags flag, Func<MethodInfo, bool> predicate = null) where T : class where ATTR : Attribute
        {
            return ExtractMethodAsAttr<ATTR>(typeof(T), flag, predicate?? Always);
        }

        public static MethodInfo[] ExtractMethodAsAttr<T>(Type type, BindingFlags flag, Func<MethodInfo, bool> predicate) where T : Attribute
        {
            return type.GetMethods(flag)
                .Where(x => x.GetCustomAttribute<T>() != null)
                .Where(predicate)
                .ToArray();
        }

        public static TAttr GetAttribute<TAttr>(Type type) where TAttr : Attribute
        {
            return type.GetCustomAttributes().FirstOrDefault(x => x is TAttr) as TAttr;
        }
        
        public static (TAttr0, TAttr1) GetAttributes<TAttr0, TAttr1>(Type type) 
            where TAttr0 : Attribute 
            where TAttr1 : Attribute
        {
            var attrs = type.GetCustomAttributes();
            return (
                attrs.OfType<TAttr0>().FirstOrDefault(), 
                attrs.OfType<TAttr1>().FirstOrDefault()
            );
        }

        public static MethodInfo GetMethod(Type type, BindingFlags flags, Func<MethodInfo, bool> predicate)
        {
            return type.GetMethods(flags).FirstOrDefault(predicate);
        }
        
        public static (MethodInfo, MethodInfo) GetMethods(
            Type type, BindingFlags flags, Func<MethodInfo, bool> pred0, Func<MethodInfo, bool> pred1
        )
        {
            var methods = type.GetMethods(flags);
            return (methods.FirstOrDefault(pred0), methods.FirstOrDefault(pred1));
        }
    }
}