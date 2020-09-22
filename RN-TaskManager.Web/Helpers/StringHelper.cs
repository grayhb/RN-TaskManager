using System;
using System.Linq;

namespace RN_TaskManager.Web.Helpers
{
    public static class StringHelper
    {
        public static int[] ToIntList(this string value)
        {
            return value.Split(",").Select(e => Convert.ToInt32(e)).ToArray();
        }

        public static int ToInt(this string value)
        {
            int.TryParse(value, out int result);
            return result;
        }

        public static string ToSimpleString(this DateTime? date)
        {
            return date != null ? $"{date:dd.MM.yyyy}" : "";
        }

        public static string ToSimpleString(this DateTime date)
        {
            return $"{date:dd.MM.yyyy}";
        }
    }
}
