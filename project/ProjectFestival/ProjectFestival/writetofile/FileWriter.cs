using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.writetofile
{
    public class FileWriter
    {
        private static bool isWriten = false;
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
    }
}
