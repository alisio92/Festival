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

        public static ObservableCollection<Stage> getStages()
        {
            ObservableCollection<Stage> stages = new ObservableCollection<Stage>();
            stages = DBConnection.GetDataOutDatabase<Stage>("Stage");

            foreach (Stage s in stages)
            {
                aantal++;
            }
            return stages;
        }

        public static int EditStage(Stage stage)
        {
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
                return rowsaffected;
            }
            catch (Exception)
            {
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

                string sql = "INSERT INTO Stage VALUES(@ID,@Name)";
                DbParameter par1 = Database.AddParameter("@Name", stage.Name);
                DbParameter par2 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteStage(Stage stage)
        {
            return DBConnection.DeleteItem("Stage", stage.ID);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
