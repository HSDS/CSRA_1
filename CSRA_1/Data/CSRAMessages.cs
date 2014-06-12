using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class CSRAMessages
    {
        static public Dictionary<string, string> Messages = new Dictionary<string, string>();

        static CSRAMessages()
        {
            Messages["001"] = "Database not verified.  Only Enrollment is allowed.";
            Messages["002"] = "Enrollment data for (user’s name) successfully saved.";
            Messages["003"] = "Acquire failed.  Want to retry?";
            Messages["004"] = "Acquire failed.  Exiting enrollment without saving.";
            Messages["005"] = "Acquire failed.  Exiting verification.";
            Messages["006"] = "Verification result:  Genuine";
            Messages["007"] = "Verification result:  Impostor";
            Messages["008"] = "Options saved.";
            Messages["009"] = "No options saved.";
        }
    }
}
