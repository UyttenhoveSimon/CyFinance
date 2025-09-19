using System;
using System.Globalization;

namespace YahooFinanceClient.Conversion
{
    public class InputConverter
    {
        public DateTime? ConvertStringToDate(string data)
        {
            if (!IsAcceptableInput(data))
            {
                return null;
            }

            var dateWithoutQuotes = data.Replace("\"", string.Empty);

            return DateTime.Parse(dateWithoutQuotes, CultureInfo.InvariantCulture);
        }

        public decimal? ConvertStringToDecimal(string data)
        {
            if (!IsAcceptableInput(data))
            {
                return null;
            }

            return Convert.ToDecimal(data, CultureInfo.InvariantCulture);
        }

        public decimal? ConvertStringToPercentDecimal(string data)
        {
            if (!IsAcceptableInput(data))
            {
                return null;
            }

            var direction = data.ToCharArray()[0];
            var number = data.Substring(1, data.Length - 2);

            if (direction == '-')
            {
                return -Convert.ToDecimal(number, CultureInfo.InvariantCulture);
            }

            return Convert.ToDecimal(number, CultureInfo.InvariantCulture);
        }

        public string CheckIfNotAvailable(string data)
        {
            if (!IsAcceptableInput(data))
            {
                return null;
            }

            return data;
        }

        private bool IsAcceptableInput(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return false;
            }
            
            var dataWithoutSpaces = data.Replace(" ", string.Empty);

            return !(dataWithoutSpaces == ""
                || dataWithoutSpaces == "N/A"
                || dataWithoutSpaces == "N/A\n"
                || dataWithoutSpaces == "N/A\n"
                || dataWithoutSpaces == "n/a"
                || dataWithoutSpaces == "n/a\n"
                || dataWithoutSpaces == "n/a\n");
        }
    }
}
