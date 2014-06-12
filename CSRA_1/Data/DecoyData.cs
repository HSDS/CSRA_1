using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRA_1.Data
{
    public class DecoyData
    {
        List<FeaturesData> featuresList = new List<FeaturesData>();
        public List<FeaturesData> FeaturesList
        {
            get { return featuresList; }
            set { featuresList = value; }
        }

    }
}
