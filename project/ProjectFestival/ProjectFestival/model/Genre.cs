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

        public static ObservableCollection<Genre> genres = new ObservableCollection<Genre>();
        public static ObservableCollection<Genre> ogenres = new ObservableCollection<Genre>();
        public static ObservableCollection<Genre> Getgenres()
        {
            ApplicationVM.Infotxt("Inladen genres", "");
            try
            {
                string sql = "SELECT * FROM Genre";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    genres.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("genres ingeladen", "Inladen genres");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database Genre niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Genre tabel niet inlezen", "");
            }
            ogenres = genres;
            return genres;
        }

        private static Genre Create(IDataRecord record)
        {
            Genre genre = new Genre();

            genre.ID = Convert.ToInt32(record["ID"]);
            genre.Name = record["Name"].ToString();

            return genre;
        }

        public static int EditGenre(Genre genre)
        {
            ApplicationVM.Infotxt("Genre aanpassen", "");
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
                ApplicationVM.Infotxt("Genre aangepast", "Genre aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Genre niet aanpassen", "");
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

                string sql = "INSERT INTO Genre VALUES(@ID,@Name)";
                DbParameter par1 = Database.AddParameter("@Name", genre.Name);
                DbParameter par2 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

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

        public static int DeleteGenre(Genre genre)
        {
            ApplicationVM.Infotxt("Genre wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Genre WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", genre.ID);

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
            return Name;
        }

        public static void Zoeken(string parameter)
        {
            parameter = parameter.ToLower();
            genres = new ObservableCollection<Genre>();

            foreach (Genre c in ogenres)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((c.Name.ToLower().Contains(parameter)) || (c.ID.ToString().ToLower().Contains(parameter)))
                    {
                        genres.Add(c);
                    }
                }
                else
                {
                    genres.Add(c);
                }
            }
        }
    }
}
