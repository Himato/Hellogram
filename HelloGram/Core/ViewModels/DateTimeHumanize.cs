using System;
using System.Globalization;

namespace HelloGram.Core.ViewModels
{
    public static class DateTimeHumanize
    {
        public static string Humanize(this DateTime input, DateTime comparisonBase)
        {
            var ts = new TimeSpan(Math.Abs(comparisonBase.Ticks - input.Ticks));

            int seconds = ts.Seconds, minutes = ts.Minutes, hours = ts.Hours, days = ts.Days;
            int years = 0, months = 0;

            // start approximate from smaller units towards bigger ones
            if (ts.Milliseconds >= 999)
            {
                seconds += 1;
            }

            if (seconds >= 59)
            {
                minutes += 1;
            }

            if (minutes >= 59)
            {
                hours += 1;
            }

            if (hours >= 23)
            {
                days += 1;
            }

            // month calculation 
            if (days >= 30 & days <= 31)
            {
                months = 1;
            }

            if (days > 31 && days < 365)
            {
                var factor = Convert.ToInt32(Math.Floor((double)days / 30));
                var maxMonths = Convert.ToInt32(Math.Ceiling((double)days / 30));
                months = (days >= 30 * factor) ? maxMonths : maxMonths - 1;
            }

            // year calculation
            if (days >= 365 && days <= 366)
            {
                years = 1;
            }

            if (days > 365)
            {
                var factor = Convert.ToInt32(Math.Floor((double)days / 365));
                var maxMonths = Convert.ToInt32(Math.Ceiling((double)days / 365));
                years = (days >= 365 * factor) ? maxMonths : maxMonths - 1;
            }
            
            if (years > 0)
            {
                return years + " " + ((years > 1) ? "years" : "year") + " ago";
            }

            if (months > 0)
            {
                return months + " " + ((months > 1) ? "months" : "month") + " ago";
            }

            if (days > 0)
            {
                return days + " " + ((days > 1) ? "days" : "day") + " ago";
            }

            if (hours > 0)
            {
                return hours + " " + ((hours > 1) ? "hours" : "hour") + " ago";
            }

            if (minutes > 0)
            {
                return minutes + " " + ((minutes > 1) ? "minutes" : "minute") + " ago";
            }

            return "Now";
        }
    }
}