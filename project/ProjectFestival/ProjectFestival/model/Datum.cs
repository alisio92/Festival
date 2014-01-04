using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.model
{
    public class Datum
    {
        public static int aantal = 1;
        public static ObservableCollection<Datum> dates = new ObservableCollection<Datum>();
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private DateTime _DatePerformence;
        public DateTime DatePerformence
        {
            get { return _DatePerformence; }
            set { _DatePerformence = value; }
        }

        public static ObservableCollection<Datum> GetDates()
        {
            ObservableCollection<Festival> festivals = festivals = Festival.GetFestival();
            aantal = 1;
            foreach (Festival festival in festivals)
            {
                int day = (int)(festival.EndDate - festival.StartDate).TotalDays;
                DateTime dateTime = festival.StartDate;

                for(int i = 0;i<=day;i++)
                {
                    Datum date = new Datum();
                    date.ID = aantal;
                    date.DatePerformence = dateTime; //dateTime.Day + "/" + dateTime.Month + "/" + dateTime.Year;
                    dates.Add(date);
                    aantal++;
                    dateTime = dateTime.AddDays(1);
                }
            }
            return dates;
        }
        public override string ToString()
        {
            //return DatePerformence.Day + "-" + DatePerformence.Month + "-" + DatePerformence.Year;
            return DatePerformence.ToString();
        }
    }
}
