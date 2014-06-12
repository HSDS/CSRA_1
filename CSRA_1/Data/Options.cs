using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class Options
    {
        bool useLogFile;
        public bool UseLogFile
        {
            get { return useLogFile; }
            set { useLogFile = value; }
        }

        public Options()
        {
            useLogFile = true;
        }

        public Options(Options options)
        {
            useLogFile = options.UseLogFile;
        }
    }
}
