using System;

namespace AppSettings.API.Extensions
{
    public static class StringExtension
    {
        public static bool Available(this string value)
        {
            return !String.IsNullOrEmpty(value);
        }
    }
}
