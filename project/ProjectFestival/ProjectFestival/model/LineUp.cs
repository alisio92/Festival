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

        public static ObservableCollection<LineUp> lineUp = new ObservableCollection<LineUp>();

        public static ObservableCollection<LineUp> GetLineUp()
        {
            string sql = "SELECT * FROM LineUp";
            DbDataReader reader = Database.GetData(sql);

            StageList = Stage.GetStages();
            BandList = Band.GetBands();

            while (reader.Read())
            {
                lineUp.Add(Create(reader));
            }

            return lineUp;
        }

        private static LineUp Create(IDataRecord record)
        {
            LineUp cp = new LineUp();
            cp.ID = Convert.ToInt32(record["ID"]);
            cp.Date = Convert.ToDateTime(record["Date"]);
            cp.From = record["StartTime"].ToString();
            cp.Until = record["EndTime"].ToString();
            int i = (int)record["Band"];
            cp.Band = new Band()
            {
                ID = (int)record["Band"],
                Name = BandList[(int)record["Band"] - 1].Name
            };
            cp.Stage = new Stage()
            {
                ID = (int)record["Stage"],
                Name = StageList[(int)record["Stage"] - 1].Name
            };

            return cp;
        }

        public static int AddLineUp(LineUp lineUp)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO LineUp VALUES(@ID,@Date,@StartTime,@EndTime,@Stage,@Band)";
                DbParameter par1 = Database.AddParameter("@ID", 1);
                DbParameter par2 = Database.AddParameter("@Date", DateTime.Now);
                DbParameter par3 = Database.AddParameter("@StartTime", "8:00");
                DbParameter par4 = Database.AddParameter("@EndTime", "8:30");
                DbParameter par5 = Database.AddParameter("@Stage", 1);
                DbParameter par6 = Database.AddParameter("@Band", 1);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5,par6);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }
        
        public static int DeleteLineUp(LineUp lineUp)
        {
            return DBConnection.DeleteItem("LineUp", lineUp.ID);
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
    }
}
