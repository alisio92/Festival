using ProjectFestival.database;
using ProjectFestival.viewmodel;
using ProjectFestival.writetofile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.model
{
    public class Festival
    {
        public static int aantal = 1;
        public static ObservableCollection<Festival> festivals = new ObservableCollection<Festival>();
        public static ObservableCollection<Festival> oFestivals = new ObservableCollection<Festival>();

        private int _idDatabase;
        public int IDDatabase
        {
            get { return _idDatabase; }
            set { _idDatabase = value; }
        }
        
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        
        private DateTime _startDate;
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        private DateTime _endDate;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value; }
        }
        public static ObservableCollection<Festival> GetFestival()
        {
            aantal = 1;
            try
            {
                string sql = "SELECT * FROM Festival";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    festivals.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            oFestivals = festivals;
            return festivals;
        }

        private static Festival Create(IDataRecord record)
        {
            Festival festival = new Festival();

            festival.ID = aantal;
            festival.IDDatabase = Convert.ToInt32(record["ID"]);
            festival.StartDate = (DateTime)record["StartDate"];
            festival.EndDate = (DateTime)record["EndDate"];
            return festival;
        }

        public static int EditFestival(Festival festival)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Festival SET StartDate=@StartDate,EndDate=@EndDate WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@StartDate", festival.StartDate);
                DbParameter par2 = Database.AddParameter("@EndDate", festival.EndDate);
                DbParameter par3 = Database.AddParameter("@ID", festival.IDDatabase);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2,par3);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
                trans.Rollback();
                return 0;
            }
        }

        public static int AddFestival(Festival festival)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Festival VALUES(@StartDate,@EndDate)";
                DbParameter par1 = Database.AddParameter("@StartDate", festival.StartDate);
                DbParameter par2 = Database.AddParameter("@EndDate", festival.EndDate);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteFestival(Festival festival)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Festival WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", festival.IDDatabase);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
                trans.Rollback();
                return 0;
            }
        }

        public static void Zoeken(string parameter)
        {
            parameter = parameter.ToLower();
            festivals = new ObservableCollection<Festival>();

            foreach (Festival festival in oFestivals)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((festival.StartDate.Year.ToString().Contains(parameter)) || (festival.ID.ToString().ToLower().Contains(parameter)) || (festival.StartDate.Month.ToString().Contains(parameter)) || (festival.StartDate.Day.ToString().Contains(parameter)))
                    {
                        festivals.Add(festival);
                    }
                }
                else
                {
                    festivals.Add(festival);
                }
            }
        }
        
        public override string ToString()
        {
            return StartDate + " - " + EndDate;
        }
    }
}
