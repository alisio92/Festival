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
    public class Stage
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public static int aantal = 1;

        public static ObservableCollection<Stage> stages = new ObservableCollection<Stage>();

        public static ObservableCollection<Stage> GetStages()
        {
            ApplicationVM.Infotxt("Inladen Stages", "");
            try
            {
                string sql = "SELECT * FROM Stage";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    stages.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("Stages ingeladen", "Inladen Stages");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database Stage niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Stage tabel niet inlezen", "");
            }
            return stages;
        }

        private static Stage Create(IDataRecord record)
        {
            Stage stage = new Stage();

            stage.ID = Convert.ToInt32(record["ID"]);
            stage.Name = record["Name"].ToString();

            return stage;
        }

        public static int EditStage(Stage stage)
        {
            ApplicationVM.Infotxt("Stage aanpassen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Stage SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", stage.Name);
                DbParameter par2 = Database.AddParameter("@ID", stage.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
                ApplicationVM.Infotxt("Stage aangepast", "Stage aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Stage niet aanpassen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int AddStage(Stage stage)
        {
            ApplicationVM.Infotxt("Stage toevoegen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Stage VALUES(@ID,@Name)";
                DbParameter par1 = Database.AddParameter("@Name", stage.Name);
                DbParameter par2 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
                ApplicationVM.Infotxt("Stage toegevoegd", "Stage aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Stage niet toevoegen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteStage(Stage stage)
        {
            ApplicationVM.Infotxt("Stage wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Stage WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", stage.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("Stage gewist", "Stage wissen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Stage niet wissen", "");
                trans.Rollback();
                return 0;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
