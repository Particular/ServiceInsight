namespace Particular.ServiceInsight.Desktop.Core.Infrastructure
{
    using System;
    using System.Linq;
    using Castle.DynamicProxy;

    public class CallForwarderInterceptor : IInterceptor
    {
        private readonly object target;
        private readonly Type targetType;

        public CallForwarderInterceptor(Type targetType)
        {
            this.targetType = targetType;
            target = null;
        }

        public CallForwarderInterceptor(object target)
        {
            targetType = target.GetType();
            this.target = target;
        }

        public void Intercept(IInvocation invocation)
        {
            var targetMethod = invocation.Method;
            var argumentTypes = invocation.Arguments.Select(a => a.GetType()).ToArray();
            var matchingTypeMethods = targetType.GetMethods().Where(m =>
            {
                var parameterInfos = m.GetParameters();

                return m.Name == targetMethod.Name &&
                                 argumentTypes.Length == parameterInfos.Length &&
                                 parameterInfos.Select((p, i) => new { p.ParameterType, Index = i })
                                               .All(p => p.ParameterType.IsAssignableFrom(argumentTypes[p.Index]));
            }).ToList();

            if (matchingTypeMethods.Count == 0) return;

            object result;
            if (targetMethod.IsGenericMethod)
            {
                result = matchingTypeMethods.Single(m => m.IsGenericMethodDefinition)
                                            .MakeGenericMethod(targetMethod.GetGenericArguments())
                                            .Invoke(target, invocation.Arguments);
            }
            else
            {
                result = matchingTypeMethods.Single(m => !m.IsGenericMethodDefinition).Invoke(target, invocation.Arguments);
            }

            invocation.ReturnValue = result;
        }
    }
}