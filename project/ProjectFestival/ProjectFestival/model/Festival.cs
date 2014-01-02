using ProjectFestival.database;
using ProjectFestival.viewmodel;
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

        public static int aantal = 1;

        public static ObservableCollection<Festival> festival = new ObservableCollection<Festival>();
        public static ObservableCollection<Festival> oFestival = new ObservableCollection<Festival>();
        public static ObservableCollection<Festival> GetFestival()
        {
            ApplicationVM.Infotxt("Inladen festival", "");
            try
            {
                string sql = "SELECT * FROM Festival";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    festival.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("festival ingeladen", "Inladen festival");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database festival niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan festival tabel niet inlezen", "");
            }
            oFestival = festival;
            return festival;
        }

        private static Festival Create(IDataRecord record)
        {
            Festival festival = new Festival();

            festival.ID = Convert.ToInt32(record["ID"]);
            festival.StartDate = (DateTime)record["StartDate"];
            festival.EndDate = (DateTime)record["EndDate"];
            return festival;
        }

        public static int EditFestival(Festival festival)
        {
            ApplicationVM.Infotxt("Festival aanpassen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Festival SET StartDate=@StartDate,EndDate=@EndDate WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@StartDate", festival.StartDate);
                DbParameter par2 = Database.AddParameter("@EndDate", festival.EndDate);
                DbParameter par3 = Database.AddParameter("@ID", festival.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2,par3);

                trans.Commit();
                ApplicationVM.Infotxt("Festival aangepast", "Festival aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Festival niet aanpassen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int AddFestival(Festival festival)
        {
            ApplicationVM.Infotxt("Festival toevoegen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Festival VALUES(@ID,@StartDate,@EndDate)";
                DbParameter par1 = Database.AddParameter("@StartDate", festival.StartDate);
                DbParameter par2 = Database.AddParameter("@EndDate", festival.EndDate);
                DbParameter par3 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2,par3);

                trans.Commit();
                ApplicationVM.Infotxt("Genre toegevoegd", "Genre aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Genre niet toevoegen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteFestival(Festival festival)
        {
            ApplicationVM.Infotxt("Genre wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Festival WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", festival.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("Genre gewist", "Genre wissen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Genre niet wissen", "");
                trans.Rollback();
                return 0;
            }
        }

        public override string ToString()
        {
            return StartDate + " - " + EndDate;
        }

    }
}
