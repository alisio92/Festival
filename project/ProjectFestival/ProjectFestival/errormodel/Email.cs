using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ProjectFestival.errormodel
{
    class Email : ValidationRule
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
            string pattern = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";
            string input = value.ToString();
            if (input.Length > Min && input.Length < Max)
            {
                if (Regex.IsMatch(input, pattern))
                {
                    result = new ValidationResult(true, null);
                }
                else
                {
                    result = new ValidationResult(false, "De email moet van deze structuur zijn naam0123@msn.com");
                }
            }
            else
            {
                result = new ValidationResult(false, "De email moet tss " + Min + " en " + Max + " liggen.");
            }
            return result;
        }
    }
}
