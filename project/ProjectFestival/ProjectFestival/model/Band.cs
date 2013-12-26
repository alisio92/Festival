﻿using ProjectFestival.database;
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

        private ObservableCollection<Genre> _genreListBand = new ObservableCollection<Genre>();
        public ObservableCollection<Genre> GenreListBand
        {
            get { return _genreListBand; }
            set { _genreListBand = value; }
        }

        public static int aantal = 1;
        public static int aantalgenres = 1;

        public static ObservableCollection<Band> bands = new ObservableCollection<Band>();
        public static ObservableCollection<Band> oBands = new ObservableCollection<Band>();

        public static ObservableCollection<Band> GetBands()
        {
            ApplicationVM.Infotxt("Inladen Band", "");
            GenreList = Genre.Getgenres();
            BandGenre.GenreList = GenreList;

            try
            {
                string sql = "SELECT * FROM Band";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    bands.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("Bands ingeladen", "Inladen Bands");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database Band niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Band tabel niet inlezen", "");
            }
            oBands = bands;
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

            string[] split = record["Genre"].ToString().Split(new Char[] { ';' });

            for (int i = 0; i < split.Count() - 1; i++)
            {
                Genre genre = new Genre();
                genre.ID = Convert.ToInt32(split[i]);
                genre.Name = GenreList[Convert.ToInt32(split[i]) - 1].Name;
                band.GenreListBand.Add(genre);
                aantalgenres++;
            }

            return band;
        }

        public static int EditBand(Band band)
        {
            ApplicationVM.Infotxt("Band aanpassen", "");
            DbTransaction trans = null;

            string genres = "";

            foreach (Genre g in band.GenreListBand)
            {
                genres += g.ID + ";";
            }

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
                DbParameter par7 = Database.AddParameter("@Genre", genres);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7);

                trans.Commit();
                ApplicationVM.Infotxt("Band aangepast", "Band aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Band niet aanpassen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int AddBand(Band band)
        {
            ApplicationVM.Infotxt("Band toevoegen", "");
            DbTransaction trans = null;

            string genres = "";

            foreach (Genre g in band.GenreListBand)
            {
                genres += g.ID + ";";
            }

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Band VALUES(@ID,@Name,@Picture,@Description,@Twitter,@Facebook,@Genre)";
                DbParameter par1 = Database.AddParameter("@Name", band.Name);
                DbParameter par3 = Database.AddParameter("@Picture", band.Picture);
                DbParameter par4 = Database.AddParameter("@Description", band.Description);
                DbParameter par5 = Database.AddParameter("@Twitter", band.Twitter);
                DbParameter par6 = Database.AddParameter("@Facebook", band.Facebook);
                DbParameter par7 = Database.AddParameter("@Genre", genres);
                DbParameter par2 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7);

                trans.Commit();
                ApplicationVM.Infotxt("Band toegevoegd", "Band aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Band niet toevoegen", "");
                trans.Rollback();
                return 0;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public static int DeleteBand(Band band)
        {
            ApplicationVM.Infotxt("Band wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Band WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", band.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("Band gewist", "Band wissen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Band niet wissen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static void Zoeken(string parameter)
        {
            parameter = parameter.ToLower();
            bands = new ObservableCollection<Band>();

            foreach (Band c in oBands)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((c.Name.ToLower().Contains(parameter)) || (c.ID.ToString().ToLower().Contains(parameter)) || (c.Facebook.ToString().ToLower().Contains(parameter)) || (c.Twitter.ToString().ToLower().Contains(parameter)))
                    {
                        bands.Add(c);
                    }
                    foreach (Genre g in c.GenreListBand)
                    {
                        if (g.Name.ToLower().Contains(parameter))
                        {
                            if (!bands.Contains(c))
                            {
                                bands.Add(c);
                            }
                        }
                    }
                }
                else
                {
                    bands.Add(c);
                }
            }
        }
    }
}
