using System;
using System.Globalization;

namespace RN_TaskManager.Web.Helpers
{
    public static class DateHelper
    {
        public static DateTime? ToSimpleDate(this string date, int partIndex = -1, bool lastDayInMonth = false)
        {
            if (string.IsNullOrEmpty(date))
                return null;

            if (partIndex != -1 && !date.Contains(","))
                return null;
            else if (date.Contains(","))
                date = date.Split(",")[partIndex];

            DateTime.TryParseExact(date.Replace(((char)8206).ToString(), ""),
                "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime customDate);

            // первый и последний день месяца
            if (customDate != null && customDate != DateTime.MinValue)
                if (lastDayInMonth)
                    customDate = new DateTime(customDate.Year, customDate.Month, DateTime.DaysInMonth(customDate.Year, customDate.Month));
                else if (partIndex != -1)
                    customDate = new DateTime(customDate.Year, customDate.Month, 1);


            return customDate;
        }

        
    }
}
