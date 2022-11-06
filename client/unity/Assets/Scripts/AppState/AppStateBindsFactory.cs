using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class AppStateBindsFactory
{
    public static AppStateBinds Extract(Type type)
    {
        var bindFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
        var allMethods = type.GetMethods(bindFlags);

        return new AppStateBinds(
            AttrToValue<bool, StateBindAttribute>(type, x => x.FlatBuffer),
            AttrToValue<bool, StateBindAttribute>(type, x => x.Action),
            AttrToValues<Type, UIBindAttribute>(type, x => x.TypeAndShow.Select(x => x.type)).ToArray(),
            AttrToValues<bool, UIBindAttribute>(type, x => x.TypeAndShow.Select(x => x.showBeforeInit)).ToArray(),
            allMethods.FirstOrDefault(AppStateInvoker.IsInitializeMethod),
            allMethods.FirstOrDefault(AppStateInvoker.IsClearMethod)
        );
    }

    private static T AttrToValue<T, Attr>(Type type, Func<Attr, T> convert) where Attr : Attribute
    {
        return type.GetCustomAttributes()
            .Where(attr => attr is Attr)
            .Select(attr => convert((attr as Attr)))
            .FirstOrDefault();
    }

    private static IEnumerable<T> AttrToValues<T, Attr>(Type type, Func<Attr, IEnumerable<T>> convert)
        where Attr : Attribute
    {
        return type.GetCustomAttributes()
            .Where(attr => attr is Attr)
            .SelectMany(attr => convert((attr as Attr)));
    }
}