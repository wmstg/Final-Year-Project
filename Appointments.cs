using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMedi
{
    class Appointments
    {
        public int PatientId { get; set; }

        public int AppointmentId { get; set; }

        public string AppointmentDate { get; set; }

        public int Duration { get; set; }

        public int ConsultantId { get; set; }

        public bool Attended { get; set; }

        public String PatientName
        {
            get
            {
                Patient patient = new Patient();
                patient.PatientId = PatientId;
                patient.GetUIDFromPatientId();
                patient.GetUserData();

                return String.Format("{0}, {1}", patient.LastName, patient.FirstName);
            }
        }

        public String ConsultantName
        {
            get
            {
                Staff staff = new Staff();
                staff.StaffId = ConsultantId;
                staff.GetUIDFromStaffId();
                staff.GetUserData();

                return String.Format("{0} {1}", "Dr", staff.LastName);
            }
        }

        public Appointments()
        {
            
        }
    }
}
