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

namespace ProjectFestival.model
{
    public class LineUp
    {
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

        private static ObservableCollection<Festival> _dateList;
        public static ObservableCollection<Festival> DateList
        {
            get { return _dateList; }
            set { _dateList = value; }
        }

        public static int vorigeWidth;
        public static int Height = 0;
        public static int aantal = 1;
        public static int aantal2 = 1;

        public static ObservableCollection<LineUp> lineUp = new ObservableCollection<LineUp>();
        public static ObservableCollection<LineUp> sLineUp = new ObservableCollection<LineUp>();
        public static ObservableCollection<LineUp> oLineUp = new ObservableCollection<LineUp>();

        public static ObservableCollection<LineUp> GetLineUp()
        {
            ApplicationVM.Infotxt("Inladen LineUp", "");
            StageList = Stage.GetStages();
            BandList = Band.GetBands();
            DateList = Festival.GetFestival();

            try
            {
                string sql = "SELECT * FROM LineUp";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    lineUp.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("LineUp ingeladen", "Inladen LineUp");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database LineUp niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan LineUp tabel niet inlezen", "");
            }
            oLineUp = lineUp;

            SortList();

            return lineUp;
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


        private static LineUp Create(IDataRecord record)
        {
            LineUp lineUp = new LineUp();
            lineUp.ID = Convert.ToInt32(record["ID"]);
            lineUp.Date = Convert.ToDateTime(record["Date"]);
            lineUp.From = record["StartTime"].ToString();
            lineUp.Until = record["EndTime"].ToString();
            lineUp.Band = new Band()
            {
                ID = (int)record["Band"],
                Name = BandList[(int)record["Band"] - 1].Name
            };
            lineUp.Stage = new Stage()
            {
                ID = (int)record["Stage"],
                Name = StageList[(int)record["Stage"] - 1].Name
            };
            lineUp.Width = GetWidth(lineUp, GetFrom(lineUp), GetUntil(lineUp));
            //lineUp.Margin = GetMargin(lineUp, GetFrom(lineUp), GetUntil(lineUp));

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
            //top = ((lineUp.Stage.ID - 1) * 60) - Height - ((aantal - 1) * 8); //((lineUp.Stage.ID - 1) * 60)
            top = ((ruimte - 1) * 60);// -((aantal2 - 1) * 8);
            //int height = ((lineUp.Stage.ID) * 60);
            //if (height > Height)
            //{
            //    Height = height+60;
            //}
            //Height = height;
            int uur = Convert.ToInt32(from[0]);
            double minuut = Convert.ToDouble(from[1]) / 60;
            double left = (uur * 200) + (minuut * 200);
            return Convert.ToInt32(left) + "," + top + ",0,0";
        }

        public static int AddLineUp(LineUp lineUp)
        {
            ApplicationVM.Infotxt("LineUp toevoegen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO LineUp VALUES(@ID,@Date,@StartTime,@EndTime,@Stage,@Band)";
                DbParameter par1 = Database.AddParameter("@ID", lineUp.ID);
                DbParameter par2 = Database.AddParameter("@Date", DateTime.Today);
                DbParameter par3 = Database.AddParameter("@StartTime", lineUp.From);
                DbParameter par4 = Database.AddParameter("@EndTime", lineUp.Until);
                DbParameter par5 = Database.AddParameter("@Stage", lineUp.Stage.ID);
                DbParameter par6 = Database.AddParameter("@Band", lineUp.Band.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6);

                trans.Commit();
                ApplicationVM.Infotxt("LineUp toegevoegd", "LineUp aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan LineUp niet toevoegen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int EditLineUp(LineUp lineUp)
        {
            ApplicationVM.Infotxt("LineUp aanpassen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE LineUp SET Date=@Date,StartTime=@StartTime,EndTime=@EndTime,Stage=@Stage,Band=@Band WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@ID", lineUp.ID);
                DbParameter par2 = Database.AddParameter("@Date", DateTime.Today);
                DbParameter par3 = Database.AddParameter("@StartTime", lineUp.From);
                DbParameter par4 = Database.AddParameter("@EndTime", lineUp.Until);
                DbParameter par5 = Database.AddParameter("@Stage", lineUp.Stage.ID);
                DbParameter par6 = Database.AddParameter("@Band", lineUp.Band.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6);

                trans.Commit();
                ApplicationVM.Infotxt("LineUp aangepast", "LineUp aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan LineUp niet aanpassen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteLineUp(LineUp lineUp)
        {
            ApplicationVM.Infotxt("LineUp wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM LineUp WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", lineUp.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("LineUp gewist", "LineUp wissen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan LineUp niet wissen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static void PrintTicket()
        {

            foreach (LineUp ssc in lineUp)
            {
                string filename = "testttttt.docx";
                File.Copy("template.docx", filename, true);
                WordprocessingDocument newdoc = WordprocessingDocument.Open(filename, true);
                IDictionary<String, BookmarkStart> bookmarks = new Dictionary<String, BookmarkStart>();
                foreach (BookmarkStart bms in newdoc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                {
                    bookmarks[bms.Name] = bms;
                }
                bookmarks["Name"].Parent.InsertAfter<Run>(new Run(new Text()), bookmarks["Name"]);
                bookmarks["Group"].Parent.InsertAfter<Run>(new Run(new Text(ssc.From)), bookmarks["Group"]);
                bookmarks["Total"].Parent.InsertAfter<Run>(new Run(new Text(ssc.Until.ToString())),
                bookmarks["Total"]);
                newdoc.Close();
            }


            //WordprocessingDocument wordDocument = WordprocessingDocument.Create(AppDomain.CurrentDomain.BaseDirectory + "test2.doc", WordprocessingDocumentType.Document);
            //IDictionary<String, BookmarkStart> bookmarks = new Dictionary<String, BookmarkStart>();

            //foreach (BookmarkStart bms in wordDocument.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
            //{
            //    bookmarks[bms.Name] = bms;
            //}
            //MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            //mainPart.Document = new Document();

            //foreach (LineUp l in lineUp)
            //{
            //    Body body = mainPart.Document.AppendChild(new Body());
            //    Paragraph para = body.AppendChild(new Paragraph());
            //    Run run = para.AppendChild(new Run());
            //    run.AppendChild(new Text(l.ID + " " + l.Band.Name + " " + l.Stage.Name + " " + l.Date + " " + l.From + " " + l.Until));
            //    RunProperties prop = new RunProperties();
            //    RunFonts font = new RunFonts() { Ascii = "Free 3 of 9 Extended", HighAnsi = "Free 3 of 9 Extended" };
            //    FontSize size = new FontSize() { Val = "96" };
            //    prop.Append(font);
            //    prop.Append(size);
            //    run.PrependChild<RunProperties>(prop);
            //    bookmarks["code"].Parent.InsertAfter<Run>(run, bookmarks["code"]);
            //}

            ////Run running = new Run(new Text("lolollolllo"));



            //wordDocument.Close();
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

        public static void Zoeken(string parameter)
        {
            parameter = parameter.ToLower();
            lineUp = new ObservableCollection<LineUp>();

            foreach (LineUp c in oLineUp)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((c.Band.Name.ToLower().Contains(parameter)) || (c.ID.ToString().ToLower().Contains(parameter)) || (c.From.ToLower().Contains(parameter)) || (c.Until.ToLower().Contains(parameter)) || (c.Stage.Name.ToLower().Contains(parameter)))
                    {
                        lineUp.Add(c);
                    }
                }
                else
                {
                    lineUp.Add(c);
                }
            }
        }

        public override string ToString()
        {
            return Band.Name + " " + From + " " + Until;
        }
    }
}
