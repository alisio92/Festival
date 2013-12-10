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
    public class Band
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

        private Byte[] _picture;
        public Byte[] Picture
        {
            get { return _picture; }
            set { _picture = value; }
        }

        private String _description;
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private String _twitter;
        public String Twitter
        {
            get { return _twitter; }
            set { _twitter = value; }
        }

        private String _facebook;
        public String Facebook
        {
            get { return _facebook; }
            set { _facebook = value; }
        }

        private static ObservableCollection<Genre> _genreList;
        public static ObservableCollection<Genre> GenreList
        {
            get { return _genreList; }
            set { _genreList = value; }
        }

        private Genre _genre;
        public Genre Genre
        {
            get { return _genre; }
            set { _genre = value; }
        }

        public static int aantal = 1;
        
        public static ObservableCollection<Band> getBands()
        {
            string sql = "SELECT * FROM Band";
            DbDataReader reader = Database.GetData(sql);

            ObservableCollection<Band> bands = new ObservableCollection<Band>();

            GenreList = Genre.getGenres();
            
            while (reader.Read())
            {
                bands.Add(Create(reader));
                aantal++;
            }
            return bands;
        }

        private static Band Create(IDataRecord record)
        {
            Band band = new Band();
            band.ID = Convert.ToInt32(record["ID"]);
            band.Name = record["Name"].ToString();

            if (!DBNull.Value.Equals(record["Picture"]))
            {
                band.Picture = (byte[])(record["Picture"]);
            }
            else
            {
                Byte[] b = new Byte[0];
                band.Picture = b;
            }

            band.Description = record["Description"].ToString();
            band.Twitter = record["Twitter"].ToString();
            band.Facebook = record["Facebook"].ToString();
            band.Genre = new Genre()
            {
                Name = record["Genre"].ToString(),
            };

            return band;
        }

        public static int EditBand(Band band)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Band SET Name=@Name,Picture=@Picture,Description=@Description,Twitter=@Twitter,Facebook=@Facebook,Genre=@Genre WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", band.Name);
                DbParameter par2 = Database.AddParameter("@ID", band.ID);
                DbParameter par3 = Database.AddParameter("@Picture", band.Picture);
                DbParameter par4 = Database.AddParameter("@Description", band.Description);
                DbParameter par5 = Database.AddParameter("@Twitter", band.Twitter);
                DbParameter par6 = Database.AddParameter("@Facebook", band.Facebook);
                DbParameter par7 = Database.AddParameter("@Genre", band.Genre.Name);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int AddBand(Band band)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Band VALUES(@ID,@Name,@Picture,@Description,@Twitter,@Facebook,@Genre)";
                DbParameter par1 = Database.AddParameter("@Name", band.Name);
                DbParameter par3 = Database.AddParameter("@Picture", band.Picture);
                DbParameter par4 = Database.AddParameter("@Description", band.Description);
                DbParameter par5 = Database.AddParameter("@Twitter", band.Twitter);
                DbParameter par6 = Database.AddParameter("@Facebook", band.Facebook);
                DbParameter par7 = Database.AddParameter("@Genre", band.Genre.Name);
                DbParameter par2 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteBand(Band band)
        {
            return DBConnection.DeleteItem("Band", band.ID);
        }
    }
}
