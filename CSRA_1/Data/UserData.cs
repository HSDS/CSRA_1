using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class UserData
    {
        public string Name = "";
        public string PIN = "";

        public UserData()
        {

        }

        public UserData(UserData data)
        {
            Name = data.Name;
            PIN = data.PIN;
        }

         override public string ToString()
        {
            string newLine = System.Environment.NewLine;

            string s = "Name: " + Name + newLine;
            s += "PIN: " + PIN;

            return s;
        }
    }
}
