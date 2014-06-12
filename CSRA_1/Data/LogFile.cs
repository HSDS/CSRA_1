using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class LogFile
    {
        static StreamWriter sw;

        static LogFile()
        {
            string fname = Path.Combine(System.Windows.Forms.Application.StartupPath, "LogFile.txt");
            sw = new StreamWriter(fname);
        }

        public static void Write(string s)
        {
            if (sw != null)
            {
                string str = DateTime.Now.ToShortDateString() + ", " + DateTime.Now.ToLongTimeString() + ", " + s;
                sw.WriteLine(str);
            }
        }

        public static void Close()
        {
            if (sw != null)
            {
                sw.Flush();
                sw.Close();
            }
        }
    }
}
