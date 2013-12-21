using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.model
{
    public class clock
    {
        private string _uur;
        public string Uur
        {
            get { return _uur; }
            set { _uur = value; }
        }

        public static ObservableCollection<clock> uren = new ObservableCollection<clock>();

        public static ObservableCollection<clock> GetUren()
        {
            for (int i = 0; i < 24; i++)
            {
                clock u = new clock();
                u.Uur = i + ":00";
                clock u2 = new clock();
                u2.Uur = i + ":30";
                uren.Add(u);
                uren.Add(u2);
            }
            return uren;
        }
        public override string ToString()
        {
            return Uur;
        }
    }
}
