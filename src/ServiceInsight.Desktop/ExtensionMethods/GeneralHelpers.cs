namespace Particular.ServiceInsight.Desktop.ExtensionMethods
{
    using System;

    public static class GeneralHelpers
    {
        public static T TryGetValue<T>(this Func<T> retreival, T defaultValue = default(T))
        {
            try
            {
                return retreival();
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public static R TryGetValue<T, R>(this Func<T, R> retreival, T parameter, R defaultValue = default(R))
        {
            try
            {
                return retreival(parameter);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }
    }
}