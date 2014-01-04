using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectFestival.errormodel
{
    class Twitter : ValidationRule
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
            string pattern = @"^@[a-zA-Z0-9]+$";
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
                        result = new ValidationResult(false, "Het veld moet met een @ starten.");
                    }
                }
                catch (Exception)
                {
                    result = new ValidationResult(false, "Het veld moet met een @ starten.");
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
