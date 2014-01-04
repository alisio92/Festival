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
    public class Stage
    {
        public static int aantal = 1;
        public static ObservableCollection<Stage> stages = new ObservableCollection<Stage>();
        public static ObservableCollection<Stage> oStages = new ObservableCollection<Stage>();

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

        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public static ObservableCollection<Stage> GetStages()
        {
            aantal = 1;
            try
            {
                string sql = "SELECT * FROM Stage";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    stages.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            oStages = stages;
            return stages;
        }

        private static Stage Create(IDataRecord record)
        {
            Stage stage = new Stage();

            stage.ID = aantal;
            stage.IDDatabase = Convert.ToInt32(record["ID"]);
            stage.Name = record["Name"].ToString();

            return stage;
        }

        public static int EditStage(Stage stage)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Stage SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", stage.Name);
                DbParameter par2 = Database.AddParameter("@ID", stage.IDDatabase);

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

        public static int AddStage(Stage stage)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Stage VALUES(@Name)";
                DbParameter par1 = Database.AddParameter("@Name", stage.Name);

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

        public static int DeleteStage(Stage stage)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Stage WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", stage.IDDatabase);

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
            stages = new ObservableCollection<Stage>();

            foreach (Stage stage in oStages)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((stage.Name.ToLower().Contains(parameter)) || (stage.ID.ToString().ToLower().Contains(parameter)))
                    {
                        stages.Add(stage);
                    }
                }
                else
                {
                    stages.Add(stage);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
