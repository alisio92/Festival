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

        public static int vorigeWidth;
        
        public static ObservableCollection<LineUp> lineUp = new ObservableCollection<LineUp>();
        public static ObservableCollection<LineUp> oLineUp = new ObservableCollection<LineUp>();

        public static ObservableCollection<LineUp> GetLineUp()
        {
            ApplicationVM.Infotxt("Inladen LineUp", "");
            StageList = Stage.GetStages();
            BandList = Band.GetBands();

            try
            {
                string sql = "SELECT * FROM LineUp";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    lineUp.Add(Create(reader));
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
            return lineUp;
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
            lineUp.Margin = GetMargin(lineUp, GetFrom(lineUp), GetUntil(lineUp));

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

        public static string GetWidth(LineUp lineUp,string[] from,string[] until)
        {
            int uurUntil = Convert.ToInt32(until[0]);
            int uurFrom = Convert.ToInt32(from[0]);
            double minutenUntil = Convert.ToDouble(until[1]);
            double minutenFrom = Convert.ToDouble(from[1]);

            double uur = (uurUntil + minutenUntil/60) - (uurFrom + minutenFrom/60);

            int width = (int)(100 * uur*2);

            return width.ToString();
        }

        public static string GetMargin(LineUp lineUp, string[] from, string[] until)
        {
            double multiply = (double)(Convert.ToInt32(from[1]) / 3 * 10);
            double multiply2 = (double)(Convert.ToInt32(until[1]) / 3 * 10);
            int start = (int)(100 * (Convert.ToInt32(from[0])*2 + multiply/100));
            int left = (int)(100 * (Convert.ToInt32(from[0])*2 + multiply/100))-vorigeWidth;

            int top = (Convert.ToInt32(lineUp.Stage.ID)-1)*70+10;

            vorigeWidth = Convert.ToInt32(lineUp.Width)+start;
            
            return left+","+top+",0,0";
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
                DbParameter par2 = Database.AddParameter("@Date", lineUp.Date);
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
                DbParameter par2 = Database.AddParameter("@Date", lineUp.Date);
                DbParameter par3 = Database.AddParameter("@StartTime", lineUp.From);
                DbParameter par4 = Database.AddParameter("@EndTime", lineUp.Until);
                DbParameter par5 = Database.AddParameter("@Stage", lineUp.Stage.ID);
                DbParameter par6 = Database.AddParameter("@Band", lineUp.Band.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5,par6);

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

        public static void JsonWegschrijven()
        {
            for (int i = 0; i < 5; i++)
            {
                LineUp l = new LineUp();
                l.ID = i;
                l.From = "17/12/2013-18:00";
                l.Until = "17/12/2013-18:00";
                l.Band = Band.bands[0];
                l.Stage = new Stage()
                {
                    ID = i,
                    Name = "bob" + i,
                };
                lineUp.Add(l);
            }

            StreamWriter sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "test.txt");
            sw.WriteLine("this \"word\" test");
            sw.Close();

            WordprocessingDocument wordDocument = WordprocessingDocument.Create(AppDomain.CurrentDomain.BaseDirectory + "test2.doc", WordprocessingDocumentType.Document);
            MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();

            mainPart.Document = new Document();

            foreach (LineUp l in lineUp)
            {
                Body body = mainPart.Document.AppendChild(new Body());
                Paragraph para = body.AppendChild(new Paragraph());
                Run run = para.AppendChild(new Run());
                run.AppendChild(new Text(l.ID + " " + l.Band.Name + " " + l.Stage.Name + " " + l.Date + " " + l.From + " " + l.Until));
            }
            wordDocument.Close();
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
    }
}
