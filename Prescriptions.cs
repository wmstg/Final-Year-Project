using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMedi
{
    class Prescriptions
    {
        public int PrescriptionId { get; set; }
        public int PatientId { get; set; }
        public String BeginDate { get; set; }
        public String EndDate { get; set; }
        public int Quantity { get; set; }
        public String Dosage { get; set; }
        public String Title { get; set; }
        public String Notes { get; set; }

        public String Details {
            get
            {
                return String.Format("{0}\n{1} {2} {3}", Title, Quantity, Dosage, Notes);
            }
        }

        public Prescriptions()
        {

        }
    }
}
