using Newtonsoft.Json;
using ProjectFestival.database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Newtonsoft.Json.Converters;
using ProjectFestival.viewmodel;
using System.Data.SqlClient;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ProjectFestival.writetofile;
using System.Windows.Forms;

namespace ProjectFestival.model
{
    public class LineUp
    {
        public static int vorigeWidth;
        public static int Height = 0;
        public static int aantal = 1;
        public static int aantal2 = 1;
        public static ObservableCollection<LineUp> lineUp = new ObservableCollection<LineUp>();
        public static ObservableCollection<LineUp> sLineUp = new ObservableCollection<LineUp>();
        public static ObservableCollection<LineUp> oLineUp = new ObservableCollection<LineUp>();

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

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        private String _from;
        public String From
        {
            get { return _from; }
            set { _from = value; }
        }

        private String _until;
        public String Until
        {
            get { return _until; }
            set { _until = value; }
        }

        private Stage _stage;
        public Stage Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }

        private Band _band;
        public Band Band
        {
            get { return _band; }
            set { _band = value; }
        }

        private string _width;
        public string Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private string _margin;
        public string Margin
        {
            get { return _margin; }
            set { _margin = value; }
        }

        private static ObservableCollection<Stage> _stageList;
        public static ObservableCollection<Stage> StageList
        {
            get { return _stageList; }
            set { _stageList = value; }
        }

        private static ObservableCollection<Band> _bandList;
        public static ObservableCollection<Band> BandList
        {
            get { return _bandList; }
            set { _bandList = value; }
        }

        private static ObservableCollection<Datum> _dateList;
        public static ObservableCollection<Datum> DateList
        {
            get { return _dateList; }
            set { _dateList = value; }
        }

        public static ObservableCollection<LineUp> GetLineUp()
        {
            aantal = 1;
            StageList = Stage.GetStages();
            BandList = Band.GetBands();
            DateList = Datum.GetDates();

            try
            {
                string sql = "SELECT * FROM LineUp";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    lineUp.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            SortList();
            oLineUp = lineUp;

            return lineUp;
        }

        private static LineUp Create(IDataRecord record)
        {
            LineUp lineUp = new LineUp();
            lineUp.ID = aantal;
            lineUp.IDDatabase = Convert.ToInt32(record["ID"]);
            lineUp.Date = (DateTime)record["Date"];
            lineUp.From = record["StartTime"].ToString();
            lineUp.Until = record["EndTime"].ToString();
            foreach (Band band in BandList)
            {
                if (band.IDDatabase == (int)record["Band"])
                {
                    lineUp.Band = new Band()
                    {
                        IDDatabase = band.IDDatabase,
                        ID = band.ID,
                        Name = band.Name,
                        Description = band.Description,
                        Facebook = band.Facebook,
                        Twitter = band.Twitter,
                        Picture = band.Picture,
                        GenreListBand = band.GenreListBand
                    };
                }
            }
            foreach (Stage stage in StageList)
            {
                if (stage.IDDatabase == (int)record["Stage"])
                {
                    lineUp.Stage = new Stage()
                    {
                        IDDatabase = stage.IDDatabase,
                        ID = stage.ID,
                        Name = stage.Name
                    };
                }
            }
            lineUp.Width = GetWidth(lineUp, GetFrom(lineUp), GetUntil(lineUp));

            return lineUp;
        }

        public static string[] GetUntil(LineUp lineUp)
        {
            String[] until = lineUp.Until.Split(new Char[] { ':' });
            return until;
        }

        public static string[] GetFrom(LineUp lineUp)
        {
            String[] from = lineUp.From.Split(new Char[] { ':' });
            return from;
        }

        public static string GetWidth(LineUp lineUp, string[] from, string[] until)
        {
            int uurUntil = Convert.ToInt32(until[0]);
            int uurFrom = Convert.ToInt32(from[0]);
            double minutenUntil = Convert.ToDouble(until[1]);
            double minutenFrom = Convert.ToDouble(from[1]);

            double uur = (uurUntil + minutenUntil / 60) - (uurFrom + minutenFrom / 60);

            int width = (int)(100 * uur * 2);

            return width.ToString();
        }

        public static string GetMargin(LineUp lineUp, string[] from, string[] until, int ruimte)
        {
            int top = 0;
            top = ((ruimte - 1) * 60);
            int uur = Convert.ToInt32(from[0]);
            double minuut = Convert.ToDouble(from[1]) / 60;
            double left = (uur * 200) + (minuut * 200);
            return Convert.ToInt32(left) + "," + top + ",0,0";
        }

        public static int AddLineUp(LineUp lineUp)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO LineUp VALUES(@Date,@StartTime,@EndTime,@Stage,@Band)";
                DbParameter par1 = Database.AddParameter("@Date", lineUp.Date);
                DbParameter par2 = Database.AddParameter("@StartTime", lineUp.From);
                DbParameter par3 = Database.AddParameter("@EndTime", lineUp.Until);
                DbParameter par4 = Database.AddParameter("@Stage", StageList[lineUp.Stage.ID - 1].IDDatabase);
                DbParameter par5 = Database.AddParameter("@Band", BandList[lineUp.Band.ID - 1].IDDatabase);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5);

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

        public static int EditLineUp(LineUp lineUp)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE LineUp SET Date=@Date,StartTime=@StartTime,EndTime=@EndTime,Stage=@Stage,Band=@Band WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@ID", lineUp.IDDatabase);
                DbParameter par2 = Database.AddParameter("@Date", lineUp.Date);
                DbParameter par3 = Database.AddParameter("@StartTime", lineUp.From);
                DbParameter par4 = Database.AddParameter("@EndTime", lineUp.Until);
                DbParameter par5 = Database.AddParameter("@Stage", StageList[lineUp.Stage.ID - 1].IDDatabase);
                DbParameter par6 = Database.AddParameter("@Band", BandList[lineUp.Band.ID - 1].IDDatabase);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6);

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

        public static int DeleteLineUp(LineUp lineUp)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM LineUp WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", lineUp.IDDatabase);

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

        public static void JsonWegschrijven()
        {
            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "test.txt");
            //sw.WriteLine("this \"word\" test");
            //lineUp.RemoveAt(1);
            //lineUp.RemoveAt(1);
            //lineUp.RemoveAt(1);
            sw.WriteLine("[ ");
            for (int i = 0; i < lineUp.Count(); i++)
            {
                foreach (Band b in Band.bands)
                {
                    if (b.Name == lineUp[i].Band.Name)
                    {
                        foreach (Genre g in Band.GenreList)
                        {
                            WegTeSchrijvenItem(sw, i, g,b);
                        }
                    }
                }
            }
            sw.WriteLine("]");
            sw.Close();
        }

        private static void WegTeSchrijvenItem(StreamWriter sw, int i, Genre g,Band b)
        {
            sw.WriteLine("{ ");
            sw.WriteLine("\"backgroundImage\" : \"images/" + g.Name + "/" + lineUp[i].Band.Name + ".jpg,");
            sw.WriteLine("\"discription\" : \"" + b.Description+"\",");
            sw.WriteLine("\"group\" : { \"backgroundImage\" : \"images/" + g.Name + "/" + g.Name+"_group_detail.jpg\",");

            if (lineUp.Count() == 1 || lineUp.Count() - 1 == i)
            {
                sw.WriteLine("}");
            }
            else
            {
                sw.WriteLine("},");
            }
        }

        private static void SortList()
        {
            ObservableCollection<LineUp> temp1 = new ObservableCollection<LineUp>();

            foreach (LineUp l in lineUp)
            {
                temp1.Add(l);
            }
            ObservableCollection<LineUp> temp2 = new ObservableCollection<LineUp>();
            int ruimte = 0;
            //sLineUp.Add(lineUp[0]);

            int id = 1;
            while (temp1.Count() > 0)
            {
                bool isToegevoegd = false;
                for (int i = 0; i < temp1.Count(); i++)
                {
                    if (temp1[i].Stage.ID == id)
                    {
                        temp1[i].Margin = GetMargin(temp1[i], GetFrom(temp1[i]), GetUntil(temp1[i]), temp1[i].Stage.ID - ruimte);
                        ruimte = temp1[i].Stage.ID;
                        temp2.Add(temp1[i]);
                        temp1.RemoveAt(i);
                        isToegevoegd = true;
                        aantal2++;
                    }
                }
                if (!isToegevoegd)
                {
                    id++;
                }
            }
            sLineUp = temp2;
        }

        public static ObservableCollection<LineUp> SortOnDate(DateTime date)
        {
            ObservableCollection<LineUp> dLineUp = new ObservableCollection<LineUp>();
            foreach(LineUp lineup in sLineUp)
            {
                if(lineup.Date == date)
                {
                    dLineUp.Add(lineup);
                }
            }
            return dLineUp;
        }
        
        public static void Zoeken(string parameter)
        {
            parameter = parameter.ToLower();
            lineUp = new ObservableCollection<LineUp>();

            foreach (LineUp lineup in oLineUp)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((lineup.Band.Name.ToLower().Contains(parameter)) || (lineup.ID.ToString().ToLower().Contains(parameter)) || (lineup.From.ToLower().Contains(parameter)) || (lineup.Until.ToLower().Contains(parameter)) || (lineup.Stage.Name.ToLower().Contains(parameter)) || (lineup.Date.ToString().ToLower().Contains(parameter)))
                    {
                        lineUp.Add(lineup);
                    }
                }
                else
                {
                    lineUp.Add(lineup);
                }
            }
        }

        public override string ToString()
        {
            return Band.Name + " " + From + " " + Until + " " + Date;
        }
    }
}
