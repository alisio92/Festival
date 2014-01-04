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
    public class Genre
    {
        public static int aantal = 1;
        public static ObservableCollection<Genre> genres = new ObservableCollection<Genre>();
        public static ObservableCollection<Genre> ogenres = new ObservableCollection<Genre>();

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
        public static ObservableCollection<Genre> Getgenres()
        {
            aantal = 1;
            try
            {
                string sql = "SELECT * FROM Genre";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    genres.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            ogenres = genres;
            return genres;
        }

        private static Genre Create(IDataRecord record)
        {
            Genre genre = new Genre();

            genre.ID = aantal;
            genre.IDDatabase = Convert.ToInt32(record["ID"]);
            genre.Name = record["Name"].ToString();

            return genre;
        }

        public static int EditGenre(Genre genre)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Genre SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", genre.Name);
                DbParameter par2 = Database.AddParameter("@ID", genre.IDDatabase);

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

        public static int AddGenre(Genre genre)
        {
            ApplicationVM.Infotxt("Genre toevoegen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Genre VALUES(@Name)";
                DbParameter par1 = Database.AddParameter("@Name", genre.Name);

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

        public static int DeleteGenre(Genre genre)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Genre WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", genre.IDDatabase);

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
            genres = new ObservableCollection<Genre>();

            foreach (Genre genre in ogenres)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((genre.Name.ToLower().Contains(parameter)) || (genre.ID.ToString().ToLower().Contains(parameter)))
                    {
                        genres.Add(genre);
                    }
                }
                else
                {
                    genres.Add(genre);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
