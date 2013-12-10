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
    public class Genre
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

        public static ObservableCollection<Genre> getGenres()
        {
            ObservableCollection<Genre> genres = new ObservableCollection<Genre>();
            genres = DBConnection.GetDataOutDatabase<Genre>("genres");

            foreach (Genre g in genres)
            {
                aantal++;
            }
            return genres;
        }

        public static int EditGenre(Genre genre)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Genre SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", genre.Name);
                DbParameter par2 = Database.AddParameter("@ID", genre.ID);

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

        public static int AddGenre(Genre genre)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Genre VALUES(@ID,@Name)";
                DbParameter par1 = Database.AddParameter("@Name", genre.Name);
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

        public static int DeleteGenre(Genre genre)
        {
            return DBConnection.DeleteItem("Genre", genre.ID);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
