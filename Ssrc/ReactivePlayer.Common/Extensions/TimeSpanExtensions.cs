using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Common.Extensions
{
    public static class TimeSpanExtensions
    {
        public static string ToTimeString(
            this TimeSpan timeSpan,
            bool showUnits = true,
            bool useDoubleDigit = true,
            bool alwaysShowDays = false,
            bool alwaysShowHours = false,
            bool alwaysShowMinutes = false,
            bool alwaysShowSeconds = false,
            bool alwaysShowMilliseconds = false)
        {
            var formatBuilder = new StringBuilder();

            if (alwaysShowDays || timeSpan.Days > 0)
            {
                formatBuilder.Append("{0");
                if (useDoubleDigit)
                    formatBuilder.Append(":00");
                formatBuilder.Append("}");

                if (showUnits)
                    formatBuilder.Append(" d");
            }

            if (alwaysShowHours || timeSpan.Hours > 0)
            {
                if (formatBuilder.Length > 0)
                    formatBuilder.Append(" ");

                formatBuilder.Append("{1");
                if (useDoubleDigit)
                    formatBuilder.Append(":00");
                formatBuilder.Append("}");

                if (showUnits)
                    formatBuilder.Append(" h");
            }

            if (alwaysShowMinutes || timeSpan.Minutes > 0)
            {
                if (formatBuilder.Length > 0)
                    formatBuilder.Append(" ");

                formatBuilder.Append("{2");
                if (useDoubleDigit)
                    formatBuilder.Append(":00");
                formatBuilder.Append("}");

                if (showUnits)
                    formatBuilder.Append(" m");
            }

            if (alwaysShowSeconds || timeSpan.Seconds > 0 || timeSpan.Milliseconds > 0)
            {
                if (formatBuilder.Length > 0)
                    formatBuilder.Append(" ");

                formatBuilder.Append("{3");
                if (useDoubleDigit)
                    formatBuilder.Append(":00");
                formatBuilder.Append("}");

                if (showUnits)
                    formatBuilder.Append(" s");
            }

            if (alwaysShowMilliseconds || timeSpan.Milliseconds > 0)
            {
                if (formatBuilder.Length > 0)
                    formatBuilder.Append(" ");

                formatBuilder.Append("{4");
                if (useDoubleDigit)
                    formatBuilder.Append(":000");
                formatBuilder.Append("}");

                if (showUnits)
                    formatBuilder.Append(" ms");
            }

            return string.Format(formatBuilder.ToString(), timeSpan.Days, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        }
    }
}