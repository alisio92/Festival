using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectFestival.errormodel
{
    class Website : ValidationRule
    {
        private int _min;
        private int _max;

        public int Min
        {
            get { return _min; }
            set { _min = value; }
        }

        public int Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            ValidationResult result = null;
            string pattern = @"^(http|https|ftp)\://[a-zA-Z0-9\-\.]+\.[a-zA-Z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*$";
            string input = value.ToString();
            if (input.Length > Min && input.Length < Max)
            {
                try
                {
                    if (Regex.IsMatch(input, pattern))
                    {
                        result = new ValidationResult(true, null);
                    }
                    else
                    {
                        result = new ValidationResult(false, "examples: \n http://www.domain.com \n https://www.domain.com");
                    }
                }
                catch (Exception)
                {
                    result = new ValidationResult(false, "examples: \n http://www.domain.com \n https://www.domain.com");
                }
            }
            else
            {
                result = new ValidationResult(false, "Het veld moet tss " + Min + " en " + Max + " liggen.");
            }
            return result;
        }
    }
}
