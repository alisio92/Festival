using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectFestival.errormodel
{
    class Uur : ValidationRule
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
            string pattern = @"^[0-9]{1,2}:[0-9]{2}$";
            string input = value.ToString();
            if (input.Length > Min && input.Length < Max && input.Contains(":"))
            {
                try
                {
                    string [] split = input.Split(new Char [] {':'});
                    if (Convert.ToInt32(split[0]) >= 0 && Convert.ToInt32(split[0]) < 24 && Convert.ToInt32(split[1]) >= 0 && Convert.ToInt32(split[1]) < 60)
                    {
                        if (Regex.IsMatch(input, pattern))
                        {
                            result = new ValidationResult(true, null);
                        }
                        else
                        {
                            result = new ValidationResult(false, "Het veld mag enkel cijfers bevatten");
                        }
                    }
                    else
                    {
                        result = new ValidationResult(false, "Het veld mag enkel cijfers bevatten");
                    }
                }
                catch (Exception)
                {
                    result = new ValidationResult(false, "Het veld mag enkel cijfers bevatten");
                }
            }
            else
            {
                result = new ValidationResult(false, "Het veld Moet tss " + Min + " en " + Max + " liggen.");
            }
            return result;
        }
    }
}
