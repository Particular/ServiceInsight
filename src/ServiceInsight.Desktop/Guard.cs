namespace Particular.ServiceInsight.Desktop
{
    using System;
    using System.Linq.Expressions;

    public static class Guard
    {
        /// <summary>
        /// Ensures the given <paramref name="value"/> is not null.
        /// Throws <see cref="ArgumentNullException"/> otherwise.
        /// </summary>
        public static void NotNull<T>(Expression<Func<T>> reference, T value, Func<Exception> error = null) where T : class
        {
            if (value == null)
            {
                if(error == null)
                {
                    throw new ArgumentNullException(GetParameterName(reference));
                }
                throw error();
            }
        }

        /// <summary>
        /// Ensures the given string <paramref name="value"/> is not null or empty.
        /// Throws <see cref="ArgumentNullException"/> in the first case, or 
        /// <see cref="ArgumentException"/> in the latter.
        /// </summary>
        public static void NotNullOrEmpty(Expression<Func<string>> reference, string value, string errorMessage = "Argument can not be empty")
        {
            NotNull(reference, value);
            if (value.Length == 0)
                throw new ArgumentException(errorMessage, GetParameterName(reference));
        }


        static string GetParameterName(LambdaExpression reference)
        {
            var member = (MemberExpression)reference.Body;
            return member.Member.Name;
        }
    }
}