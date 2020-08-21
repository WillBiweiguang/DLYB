/**
 * 2010-4-2
 * 
 * 何英
 * 
 * 该实体类主要是通过反射机制来获取泛型实体类的实例的
 * 
 * 
 * */

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Data;
//using  DEF.Infrastructure.Framework.DBCommon;

namespace Infrastructure.Utility.Reflection
{

    /// <summary>
    /// Abstraction of the function of accessing member of a object at runtime.
    /// </summary>
    public interface IMemberAccessor {
        /// <summary>
        /// Get the member value of an object.
        /// </summary>
        /// <param name="instance">The object to get the member value from.</param>
        /// <param name="memberName">The member name, could be the name of a property of field. Must be public member.</param>
        /// <returns>The member value</returns>
        object GetValue(object instance, string memberName);

        /// <summary>
        /// Set the member value of an object.
        /// </summary>
        /// <param name="instance">The object to get the member value from.</param>
        /// <param name="memberName">The member name, could be the name of a property of field. Must be public member.</param>
        /// <param name="newValue">The new value of the property for the object instance.</param>
        void SetValue(object instance, string memberName, object newValue);
    }

    /// <summary>
    /// 该实体类主要是通过反射机制来获取泛型实体类的实例的
    /// </summary>
    public class MemberFactory<T> : IMemberAccessor {


        internal static Func<object, string, object> GetValueDelegate;
        internal static Action<object, string, object> SetValueDelegate;


        internal static Func<IDataReader, T> SetEntityValueDelegate;

        public object GetValue(T instance, string memberName) {
            return GetValueDelegate(instance, memberName);
        }

        public void SetValue(T instance, string memberName, object newValue) {
            SetValueDelegate(instance, memberName, newValue);
        }

        public T SetValueEntiry(IDataReader dr) {
            return SetEntityValueDelegate(dr);
        }

        public object GetValue(object instance, string memberName) {
            return GetValueDelegate(instance, memberName);
        }

        public void SetValue(object instance, string memberName, object newValue) {
            SetValueDelegate(instance, memberName, newValue);
        }

        static MemberFactory() {
            GetValueDelegate = GenerateGetValue();
            SetValueDelegate = GenerateSetValue();

            // SetEntityValueDelegate = getExpressionDelegate();
        }

        private static Func<object, string, object> GenerateGetValue() {
            var type = typeof(T);
            var instance = Expression.Parameter(typeof(object), "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in type.GetProperties()) {
                var property = Expression.Property(Expression.Convert(instance, typeof(T)), propertyInfo.Name);
                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Func<object, string, object>>(methodBody, instance, memberName).Compile();
        }

        private static Action<object, string, object> GenerateSetValue() {
            var type = typeof(T);
            var instance = Expression.Parameter(typeof(object), "instance");
            var memberName = Expression.Parameter(typeof(string), "memberName");
            var newValue = Expression.Parameter(typeof(object), "newValue");
            var nameHash = Expression.Variable(typeof(int), "nameHash");
            var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
            var cases = new List<SwitchCase>();
            foreach (var propertyInfo in type.GetProperties()) {
                var property = Expression.Property(Expression.Convert(instance, typeof(T)), propertyInfo.Name);


                // Expression.Call(newValue, typeof(object).GetMethod("GetType"));
                // var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType, typeof(GetTypeClass<propertyInfo.PropertyType>).GetMethod("GetTypeEx")));

                //  var setValue = Expression.Assign(property, Expression.Convert(newVtypealue, propertyInfo.PropertyType));
                Expression setValue;


                //if (propertyInfo.GetCustomAttributes(typeof(IgnoreAttribute), false).Length > 0)
                //{
                //    continue;
                //}

                //临时解决（将数字类型先转成decimal然后转成对应的类型）
                if (propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType != typeof(DateTime?) && propertyInfo.PropertyType != typeof(Boolean?))
                {
                    setValue = Expression.Assign(property, Expression.Convert(Expression.Convert(newValue, typeof(System.Decimal)), propertyInfo.PropertyType));
                } else {
                    setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
                }


                var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

                cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
            }
            var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
            var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

            return Expression.Lambda<Action<object, string, object>>(methodBody, instance, memberName, newValue).Compile();
        }


        private static Func<IDataReader, T> getExpressionDelegate() {
            // hang on to row[string] property 
            var indexerProperty = typeof(IDataReader).GetProperty("Item", new[] { typeof(string) });

            // list of statements in our dynamic method
            var statements = new List<Expression>();

            // store instance for setting of properties
            ParameterExpression instanceParameter = Expression.Variable(typeof(T));
            ParameterExpression sqlDataReaderParameter = Expression.Parameter(typeof(IDataReader));

            // create and assign new T to variable: var instance = new T();
            BinaryExpression createInstance = Expression.Assign(instanceParameter, Expression.New(typeof(T)));
            statements.Add(createInstance);

            foreach (var property in typeof(T).GetProperties()) {

                // instance.MyProperty
                MemberExpression getProperty = Expression.Property(instanceParameter, property);

                // row[property] -- NOTE: this assumes column names are the same as PropertyInfo names on T
                IndexExpression readValue = Expression.MakeIndex(sqlDataReaderParameter, indexerProperty, new[] { Expression.Constant(property.Name) });

                BinaryExpression assignProperty;

                //instance.MyProperty = row[property]
                if (readValue.Type != property.PropertyType) {
                    assignProperty = Expression.Assign(getProperty, Expression.Convert(Expression.Call(property.PropertyType, "Parse", null, new Expression[] { Expression.ConvertChecked(readValue, typeof(string)) }), property.PropertyType));
                    statements.Add(assignProperty);
                } else {
                    // instance.MyProperty = row[property]
                    assignProperty = Expression.Assign(getProperty, Expression.Convert(readValue, property.PropertyType));
                    statements.Add(assignProperty);
                }

                // instance.MyProperty = row[property]
                //  BinaryExpression assignProperty = Expression.Assign(getProperty, Expression.Convert(readValue, property.PropertyType));
                statements.Add(assignProperty);
            }

            var returnStatement = instanceParameter;
            statements.Add(returnStatement);

            var body = Expression.Block(instanceParameter.Type, new[] { instanceParameter }, statements.ToArray());

            var lambda = Expression.Lambda<Func<IDataReader, T>>(body, sqlDataReaderParameter);

            // cache me!
            return lambda.Compile();
        }



        public static Expression GetConvertExpression(Expression instanceExpr, Type targetType) {
            var mediateType = instanceExpr.Type;

            if (mediateType == typeof(object)) {
                // (TargetType)instance
                return Expression.Convert(instanceExpr, targetType);
            }

            while (mediateType != typeof(object)) {
                try {
                    // (MediateType)instace
                    var mediateExpr = Expression.Convert(instanceExpr, mediateType);
                    // (TargetType)(MediateType)instance
                    return Expression.Convert(mediateExpr, targetType);
                } catch {
                    mediateType = mediateType.BaseType;
                }
            }

            throw new Exception("");
        }

    }


    ///// <summary>
    ///// 该实体类主要是通过反射机制来获取泛型实体类的实例的
    ///// </summary>
    //public class MemberFactory : IMemberAccessor
    //{


    //    internal static Func<object, string, object> GetValueDelegate;
    //    internal static Action<object, string, object> SetValueDelegate;

    //    public object GetValue(IEntity instance, string memberName)
    //    {
    //        return GetValueDelegate(instance, memberName);
    //    }

    //    public void SetValue(IEntity instance, string memberName, object newValue)
    //    {
    //        SetValueDelegate(instance, memberName, newValue);
    //    }

    //    public object GetValue(object instance, string memberName)
    //    {
    //        return GetValueDelegate(instance, memberName);
    //    }

    //    public void SetValue(object instance, string memberName, object newValue)
    //    {
    //        SetValueDelegate(instance, memberName, newValue);
    //    }

    //    static MemberFactory()
    //    {
    //        GetValueDelegate = GenerateGetValue(tEntity);
    //        SetValueDelegate = GenerateSetValue(tEntity);
    //    }

    //    private static Func<object, string, object> GenerateGetValue(Type tEntity)
    //    {
    //        var type = tEntity;
    //        var instance = Expression.Parameter(typeof(object), "instance");
    //        var memberName = Expression.Parameter(typeof(string), "memberName");
    //        var nameHash = Expression.Variable(typeof(int), "nameHash");
    //        var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
    //        var cases = new List<SwitchCase>();
    //        foreach (var propertyInfo in type.GetProperties())
    //        {
    //            var property = Expression.Property(Expression.Convert(instance, tEntity), propertyInfo.Name);
    //            var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

    //            cases.Add(Expression.SwitchCase(Expression.Convert(property, typeof(object)), propertyHash));
    //        }
    //        var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
    //        var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

    //        return Expression.Lambda<Func<object, string, object>>(methodBody, instance, memberName).Compile();
    //    }

    //    private static Action<object, string, object> GenerateSetValue(Type tEntity)
    //    {
    //        var type = tEntity;
    //        var instance = Expression.Parameter(typeof(object), "instance");
    //        var memberName = Expression.Parameter(typeof(string), "memberName");
    //        var newValue = Expression.Parameter(typeof(object), "newValue");
    //        var nameHash = Expression.Variable(typeof(int), "nameHash");
    //        var calHash = Expression.Assign(nameHash, Expression.Call(memberName, typeof(object).GetMethod("GetHashCode")));
    //        var cases = new List<SwitchCase>();
    //        foreach (var propertyInfo in type.GetProperties())
    //        {
    //            var property = Expression.Property(Expression.Convert(instance, tEntity), propertyInfo.Name);
    //            var setValue = Expression.Assign(property, Expression.Convert(newValue, propertyInfo.PropertyType));
    //            var propertyHash = Expression.Constant(propertyInfo.Name.GetHashCode(), typeof(int));

    //            cases.Add(Expression.SwitchCase(Expression.Convert(setValue, typeof(object)), propertyHash));
    //        }
    //        var switchEx = Expression.Switch(nameHash, Expression.Constant(null), cases.ToArray());
    //        var methodBody = Expression.Block(typeof(object), new[] { nameHash }, calHash, switchEx);

    //        return Expression.Lambda<Action<object, string, object>>(methodBody, instance, memberName, newValue).Compile();
    //    }
    //}
}
