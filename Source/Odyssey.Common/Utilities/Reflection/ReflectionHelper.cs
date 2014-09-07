#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

#endregion Using Directives

namespace Odyssey.Utilities.Reflection
{
    public static class ReflectionHelper
    {
        public static bool AreTypesDerived(IEnumerable<Type> derivedTypes, Type baseType)
        {
            TypeInfo baseTypeInfo = baseType.GetTypeInfo();
            return derivedTypes.All(t => baseTypeInfo.IsAssignableFrom(t.GetTypeInfo()));
        }

        public static bool ContainsProperty(Type type, string propertyName)
        {
            return GetProperty(type, propertyName) != null;
        }

        public static PropertyInfo GetProperty(Type type, string propertyName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(propertyName));
            return type.GetRuntimeProperty(propertyName);
        }

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            return type.GetRuntimeProperties();
        }

        public static IEnumerable<FieldInfo> GetFields(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            return type.GetRuntimeFields();
        }

        public static IEnumerable<EventInfo> GetEvents(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            return type.GetRuntimeEvents();
        }

        public static MethodInfo GetMethod(Type type, string methodName, Type[] parameters)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(methodName));
            return type.GetRuntimeMethod(methodName, parameters);
        }

        public static IEnumerable<MethodInfo> GetMethods(Type type)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            return type.GetRuntimeMethods();
        }

        public static MethodInfo GetMethod(Type type, string methodName)
        {
            return GetMethods(type).First(m => string.Equals(m.Name, methodName));
        }

        public static FieldInfo GetField(Type type, string fieldName)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(fieldName));
            return type.GetRuntimeField(fieldName);
        }

        public static FieldInfo FindFieldPath(Type type, string path, out PropertyInfo containingProperty)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            TypeInfo currentType = type.GetTypeInfo();
            containingProperty = null;

            foreach (string propertyName in path.Split('.'))
            {
                PropertyInfo property = currentType.GetDeclaredProperty(propertyName);

                if (property == null)
                    return currentType.GetDeclaredField(propertyName);
                else
                {
                    currentType = property.PropertyType.GetTypeInfo();
                    containingProperty = property;
                }
            }

            return null;
        }

        public static PropertyInfo FindPropertyPath(Type type, string path)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            TypeInfo currentType = type.GetTypeInfo();
            PropertyInfo propertyInfo = null;
            foreach (string propertyName in path.Split('.'))
            {
                propertyInfo = currentType.GetDeclaredProperty(propertyName);
                currentType = propertyInfo.PropertyType.GetTypeInfo();
            }

            return propertyInfo;
        }

        public static MemberInfo FindMemberPath(Type type, string path, out PropertyInfo containingProperty)
        {
            Contract.Requires<ArgumentNullException>(type != null, "type");
            const string rArrayPattern = @"(?<array>\w*)\[(?<index>\d+)\]";
            Regex rArray = new Regex(rArrayPattern);
            ;
            TypeInfo currentType = type.GetTypeInfo();
            containingProperty = null;
            MemberInfo member = null;

            foreach (string propertyName in path.Split('.'))
            {
                var match = rArray.Match(propertyName);
                if (match.Success)
                {
                    string array = match.Groups["array"].Value;

                    currentType = (currentType.GetDeclaredProperty(array)).GetType().GetTypeInfo();

                }

                member = currentType.GetDeclaredProperty(propertyName) as MemberInfo ?? currentType.GetDeclaredField(propertyName) as MemberInfo;

                if (member !=null)
                {
                    currentType = member.GetType().GetTypeInfo();
                    containingProperty = (PropertyInfo)member;
                }
            }

            return member;
        }

        public static TAttribute GetAttribute<TAttribute>(Type sourceType)
            where TAttribute : Attribute
        {
            TypeInfo typeInfo = sourceType.GetTypeInfo();
            return typeInfo.GetCustomAttribute<TAttribute>();
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(Type sourceType)
            where TAttribute : Attribute
        {
            TypeInfo typeInfo = sourceType.GetTypeInfo();
            return typeInfo.GetCustomAttributes<TAttribute>();
        }

        public static IEnumerable<Type> GetDerivedTypes(Assembly assembly, Type type)
        {
            TypeInfo baseTypeInfo = type.GetTypeInfo();
            var types = from typeInfo in assembly.DefinedTypes
                        where baseTypeInfo.IsAssignableFrom(typeInfo)
                        select typeInfo.AsType();

            return types;
        }

        public static string GetPropertyName<TClass>(Expression<Func<TClass, object>> propertyExpression)
        {
            var body = propertyExpression.ToString();
            int start = body.IndexOf('.')+1;
            int end = body.Length - start;
            if (body.EndsWith(")"))
                end -= 1;
            body = body.Substring(start, end);
            return body;
        }

        [Pure]
        public static bool IsTypeDerived(Type derivedType, Type baseType)
        {
            return baseType.GetTypeInfo().IsAssignableFrom(derivedType.GetTypeInfo());
        }
    }
}