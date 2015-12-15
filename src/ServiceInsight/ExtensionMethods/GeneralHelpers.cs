namespace ServiceInsight.ExtensionMethods
{
    using System;

    public static class GeneralHelpers
    {
        public static T TryGetValue<T>(this Func<T> retrieval, T defaultValue = default(T))
        {
            try
            {
                return retrieval();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static R TryGetValue<T, R>(this Func<T, R> retrieval, T parameter, R defaultValue = default(R))
        {
            try
            {
                return retrieval(parameter);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}