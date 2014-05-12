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

        public static void True(bool expression)
        {
            if (!expression)
                throw new InvalidOperationException("Value was not expected");
        }

        public static void False(bool expression)
        {
            if (expression)
                throw new InvalidOperationException("Value was not expected");
        }

        public static void True(bool expression, Func<Exception> error)
        {
            if (!expression)
                throw error();
        }

        public static void False(bool expression, Func<Exception> error)
        {
            if (expression)
                throw error();
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

        /// <summary>
        /// Checks an argument to ensure it is in the specified range including the edges.
        /// </summary>
        /// <typeparam name="T">Type of the argument to check, it must be an <see cref="IComparable"/> type.
        /// </typeparam>
        /// <param name="reference">The expression containing the name of the argument.</param>
        /// <param name="value">The argument value to check.</param>
        /// <param name="from">The minimun allowed value for the argument.</param>
        /// <param name="to">The maximun allowed value for the argument.</param>
        public static void NotOutOfRangeInclusive<T>(Expression<Func<T>> reference, T value, T from, T to) where T : IComparable
        {
            if ((value.CompareTo(from) < 0 || value.CompareTo(to) > 0))
                throw new ArgumentOutOfRangeException(GetParameterName(reference));
        }

        /// <summary>
        /// Checks an argument to ensure it is in the specified range excluding the edges.
        /// </summary>
        /// <typeparam name="T">Type of the argument to check, it must be an <see cref="IComparable"/> type.
        /// </typeparam>
        /// <param name="reference">The expression containing the name of the argument.</param>
        /// <param name="value">The argument value to check.</param>
        /// <param name="from">The minimun allowed value for the argument.</param>
        /// <param name="to">The maximun allowed value for the argument.</param>
        public static void NotOutOfRangeExclusive<T>(Expression<Func<T>> reference, T value, T from, T to) where T : IComparable
        {
            if ((value.CompareTo(from) <= 0 || value.CompareTo(to) >= 0))
                throw new ArgumentOutOfRangeException(GetParameterName(reference));
        }

        static string GetParameterName(LambdaExpression reference)
        {
            var member = (MemberExpression)reference.Body;
            return member.Member.Name;
        }
    }
}