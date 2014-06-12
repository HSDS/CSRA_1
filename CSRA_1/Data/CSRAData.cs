using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class CSRAData
    {
        UserData userData;
        public UserData UserData
        {
            get { return userData; }
            set { userData = value; }
        }

        Options options;
        public Options Options
        {
            get { return options; }
            set { options = value; }
        }

        EnrollmentData enrollmentData;
        public EnrollmentData EnrollmentData
        {
            get { return enrollmentData; }
            set { enrollmentData = value; }
        }

        DecoyData decoyData;
        public DecoyData DecoyData
        {
            get { return decoyData; }
            set { decoyData = value; }
        }

        public CSRAData()
        {
            userData = new UserData();
            options = new Options();
            enrollmentData = new EnrollmentData();
            decoyData = new DecoyData();
        }

        public bool HasEnrollmentData()
        {
            return EnrollmentData.Threshold > 0 &&
                EnrollmentData.FeaturesList.Count > 0 &&
                DecoyData.FeaturesList.Count > 0;
        }

        static public CSRAData Load()
        {
            string fname = Path.Combine(System.Windows.Forms.Application.StartupPath, "CSRAData.xml");

            CSRAData data = (CSRAData)Serializer.XmlDeserialize(fname, typeof(CSRAData));

            return data;
        }

        static public bool Save(CSRAData data)
        {
            string fname = Path.Combine(System.Windows.Forms.Application.StartupPath, "CSRAData.xml");

            return Serializer.XmlSerialize(fname, data);
        }
    }
}
