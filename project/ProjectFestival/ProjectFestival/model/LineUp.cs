using ProjectFestival.database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.model
{
    public class LineUp
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        private String _from;
        public String From
        {
            get { return _from; }
            set { _from = value; }
        }

        private String _until;
        public String Until
        {
            get { return _until; }
            set { _until = value; }
        }

        private Stage _stage;
        public Stage Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }

        private Band _band;
        public Band Band
        {
            get { return _band; }
            set { _band = value; }
        }

        private static ObservableCollection<Stage> _stageList;
        public static ObservableCollection<Stage> StageList
        {
            get { return _stageList; }
            set { _stageList = value; }
        }

        public static ObservableCollection<LineUp> GetLineUp()
        {
            ObservableCollection<LineUp> lineUp = new ObservableCollection<LineUp>();

            string sql = "SELECT * FROM LineUp";
            DbDataReader reader = Database.GetData(sql);

            StageList = Stage.getStages();

            while (reader.Read())
            {
                lineUp.Add(Create(reader));
            }

            return lineUp;
        }

        private static LineUp Create(IDataRecord record)
        {
            LineUp cp = new LineUp();
            cp.ID = Convert.ToInt32(record["ID"]);
            cp.Date = Convert.ToDateTime(record["Date"]);
            cp.From = record["StartTime"].ToString();
            cp.Until = record["EndTime"].ToString();
            cp.Band = new Band()
            {
                Name = record["JobTitle"].ToString(),
            };
            cp.Stage = new Stage()
            {
                Name = record["Stage"].ToString(),
            };

            return cp;
        }

        public static int DeleteLineUp(LineUp lineUp)
        {
            return DBConnection.DeleteItem("LineUp", lineUp.ID);
        }
    }
}
