using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class FeaturesData
    {
        byte[] irisColor = new byte[3];
        public byte[] IrisColor
        {
            get { return irisColor; }
            set { irisColor = value; }
        }

        double[] zernikeCoefficients = new double[66];
        public double[] ZernikeCoefficients
        {
            get { return zernikeCoefficients; }
            set { zernikeCoefficients = value; }
        }
    }
}
