using System;

namespace AppSettings.API.Extensions
{
    public static class ObjectExtension
    {
        public static bool Available(this object value)
        {
            return value != null;
        }
    }
}
