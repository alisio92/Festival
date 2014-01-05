using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace ProjectFestival.writetofile
{
    public class FileWriter
    {
        private static bool isWriten = false;
        private static int nummer = 10000;
        public static void WriteToFile(string line)
        {
            if (!isWriten)
            {
                StreamWriter sw = null;
                isWriten = true;

                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "log.txt"))
                {
                    sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "log.txt", true);
                }
                else
                {
                    sw = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "log.txt", true);
                    sw.WriteLine("ErrorLogboek Festival");
                    sw.WriteLine("Gemaakt op " + DateTime.Now);
                    sw.WriteLine("");
                }
                sw.WriteLine(line);
                sw.WriteLine("Gemaakt op " + DateTime.Now);
                sw.WriteLine("");
                sw.Close();
            }
        }

        public static void PrintTicket(Ticket ticket)
        {
            Random random = new Random();
            string barcode = random.Next(100000000, 999999999).ToString();
            Stream stream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            double price = 0;

            foreach (TicketType type in TicketType.ticketTypes)
            {
                if (type.Name == ticket.TicketType.Name)
                {
                    price = type.Price;
                }
            }

            saveFileDialog.Filter = "docx files (*.docx)|*.docx";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((stream = saveFileDialog.OpenFile()) != null)
                {
                    stream.Close();
                    FileStream fs = stream as FileStream;
                    File.Copy("template.docx", fs.Name, true);
                    WordprocessingDocument newdoc = WordprocessingDocument.Open(fs.Name, true);
                    IDictionary<String, BookmarkStart> bookmarks = new Dictionary<String, BookmarkStart>();
                    foreach (BookmarkStart bms in newdoc.MainDocumentPart.RootElement.Descendants<BookmarkStart>())
                    {
                        bookmarks[bms.Name] = bms;
                    }

                    double prijs = price * ticket.Amount;
                    string date = DateTime.Today.Day + "-" + DateTime.Today.Month + "-" + DateTime.Today.Year;

                    bookmarks["Date"].Parent.InsertAfter<Run>(new Run(new Text(date)), bookmarks["Date"]);
                    bookmarks["Number"].Parent.InsertAfter<Run>(new Run(new Text(nummer.ToString())), bookmarks["Number"]);
                    bookmarks["Name"].Parent.InsertAfter<Run>(new Run(new Text(ticket.TicketHolder)), bookmarks["Name"]);
                    bookmarks["Type"].Parent.InsertAfter<Run>(new Run(new Text(ticket.TicketType.Name)), bookmarks["Type"]);
                    bookmarks["Amount"].Parent.InsertAfter<Run>(new Run(new Text(ticket.Amount.ToString())), bookmarks["Amount"]);
                    bookmarks["Price"].Parent.InsertAfter<Run>(new Run(new Text(price.ToString())), bookmarks["Price"]);
                    bookmarks["Total"].Parent.InsertAfter<Run>(new Run(new Text(prijs.ToString())), bookmarks["Total"]);
                    bookmarks["BarcodeNumber"].Parent.InsertAfter<Run>(new Run(new Text(barcode)), bookmarks["BarcodeNumber"]);
                    Run run = new Run(new Text(barcode));
                    RunProperties prop = new RunProperties();
                    RunFonts font = new RunFonts() { Ascii = "Free 3 of 9 Extended", HighAnsi = "Free 3 of 9 Extended" };
                    FontSize size = new FontSize() { Val = "96" };
                    prop.Append(font);
                    prop.Append(size);
                    run.PrependChild<RunProperties>(prop);
                    bookmarks["Barcode"].Parent.InsertAfter<Run>(run, bookmarks["Barcode"]);
                    nummer++;
                    newdoc.Close();
                }
            }
        }

        public static void JsonWegschrijven()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("\\ProjectFestival\\bin\\Debug\\", "\\Festival\\Data\\");
            StreamWriter sw = new StreamWriter(path + "Festival.txt");
            int key = 1000;
            sw.WriteLine("[ ");

            for (int i = 0; i < Band.bands.Count(); i++)
            {
                int aantal = 0;
                foreach (Genre g in Band.bands[i].GenreListBand)
                {
                    int value = key + aantal;
                    ImageWegschrijven(g.Name+"\\" + Band.bands[i].Name + ".jpg",Band.bands[i].Picture);
                    sw.WriteLine("{ ");
                    sw.WriteLine("\"backgroundImage\" : \"images/" + g.Name + "/" + Band.bands[i].Name + ".jpg\",");
                    sw.WriteLine("\"discription\" : \"" + Band.bands[i].Description + "\",");
                    //sw.WriteLine("\"group\" : { \"backgroundImage\" : \"images/" + g.Name + "/" + g.Name + "_group_detail.jpg\",");
                    sw.WriteLine("\"group\" : { \"backgroundImage\" : \"images/Chinese/chinese_group_detail.png\",");
                    sw.WriteLine("\"description\" : \"" + Band.bands[i].Description + "\",");
                    //sw.WriteLine("\"groupImage\" : \"images/" + g.Name + "/" + g.Name + "_group.jpg\",");
                    sw.WriteLine("\"groupImage\" : \"images/Chinese/chinese_group_detail.png\",");
                    sw.WriteLine("\"key\" : \"" + g.Name + "\",");
                    sw.WriteLine("\"shortTitle\" : \"" + g.Name + "\",");
                    sw.WriteLine("\"title\" : \"" + g.Name + "\"");
                    sw.WriteLine("},");
                    sw.WriteLine("\"genres\" : [ ");
                    for (int i2 = 0; i2 < Band.bands[i].GenreListBand.Count(); i2++)
                    {
                        sw.WriteLine("\"" + Band.bands[i].GenreListBand[i2].Name + "\"");
                        if (Band.bands[i].GenreListBand.Count() != 1 && Band.bands[i].GenreListBand.Count() - 1 != i2)
                        {
                            sw.WriteLine(",");
                        }
                    }
                    sw.WriteLine("],");
                    sw.WriteLine("\"key\" : " + value + ",");
                    sw.WriteLine("\"facebook\" : \"" + Band.bands[i].Facebook + "\",");
                    sw.WriteLine("\"twitter\" : \"" + Band.bands[i].Twitter + "\",");
                    sw.WriteLine("\"shortTitle\" : \"" + Band.bands[i].Name + "\",");
                    sw.WriteLine("\"tileImage\" : \"images/" + g.Name + "/" + Band.bands[i].Name + ".jpg\",");
                    sw.WriteLine("\"title\" : \"" + Band.bands[i].Name + "\"");

                    if (Band.bands.Count() == 1 || Band.bands.Count() - 1 == i)
                    {
                        sw.WriteLine("}");
                    }
                    else
                    {
                        sw.WriteLine("},");
                    }
                    aantal++;
                }
                key += 1000;
            }
            sw.WriteLine("]");
            sw.Close();
        }

        private static void ImageWegschrijven(string pathSource, Byte[] bytes)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("\\ProjectFestival\\bin\\Debug\\", "\\Festival\\Images\\");
            if (bytes != null && bytes.Length != 0)
            {
                Image imageOutput = Image.FromStream(new MemoryStream(bytes));
                imageOutput.Save(path + pathSource, ImageFormat.Jpeg);
            }
        }

        public static void MakeMap(string name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("\\ProjectFestival\\bin\\Debug\\", "\\Festival\\Images\\");
            try
            {
                if (Directory.Exists(path + name))
                {
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(path + name);
            }
            catch (Exception e)
            {
                WriteToFile(e.Message);
            }
        }
        public static void EditMap(string oldName, string name)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("\\ProjectFestival\\bin\\Debug\\", "\\Festival\\Images\\");
            try
            {
                File.Move(path + oldName, path + name);
            }
            catch (Exception e)
            {

            }
        }
    }
}
