using System;
using System.Linq.Expressions;

namespace NServiceBus.Profiler.Tests.Helpers
{
    public static class TestExtensions
    {
        public static TObj SetPrivate<TObj, TProp>(this TObj obj, Expression<Func<TObj, TProp>> property, TProp value)
        {
            var propertyName = ((MemberExpression)property.Body).Member.Name;

            obj.GetType().GetProperty(propertyName).SetValue(obj, value);

            return obj;
        }
    }
}