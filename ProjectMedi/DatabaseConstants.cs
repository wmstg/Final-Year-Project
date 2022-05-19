using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectMedi
{
    static class DatabaseConstants
    {
        /// <summary>
        /// Table Names
        /// </summary>
        public static String ACCESS_RIGHTS_TABLE = "AccessRights";
        public static String CONSULTATIONS_TABLE = "Consultations";
        public static String PATIENT_APPOINTMENTS_TABLE = "PatientAppointments";
        public static String PATIENTS_TABLE = "Patients";
        public static String PRESCRIPTIONS_TABLE = "Prescriptions";
        public static String ROLE_TABLE = "Role";
        public static String STAFF_TABLE = "Staff";
        public static String USER_ADDRESS_TABLE = "UserAddress";
        public static String USERS_TABLE = "Users";

        /// <summary>
        /// Access Rights Table Fields
        /// </summary>
        /// 
        public static String IS_ADMINISTRATOR = "IsAdministrator";
        public static String REGISTER_PATIENT = "RegisterPatient";
        public static String REMOVE_PATIENT = "RemovePatient";
        public static String SEARCH_PATIENTS = "SearchPatients";
        public static String MANAGE_APPOINTMENTS = "ManageAppointments";
        public static String ADD_DOCTOR = "AddDoctor";
        public static String REMOVE_DOCTOR = "RemoveDoctor";
        public static String SEARCH_DOCTORS = "SearchDoctors";
        public static String MANAGE_PRESCRIPTIONS = "ManagePrescriptions";
        public static String MANAGE_MEDICAL_RECORDS = "ManageMedicalRecords";

        public static String PATIENT_ID = "PatientId";
        public static String USER_ID = "UserId";
        public static String GENDER = "Gender";
        public static String CONTACT_NUMBER = "ContactNumber";
        public static String DATE_OF_BIRTH = "DateOfBirth";
        public static String NATIONALITY = "Nationality";
        public static String DATE_JOINED_SURGERY = "DateJoinedSurgery";

        public static String ADDRESS_LINE_1 = "AddressLine1";
        public static String ADDRESS_LINE_2 = "AddressLine2";
        public static String POST_CODE = "PostCode";
        public static String CITY = "City";
        public static String IS_PRIMARY_ADDRESS = "IsPrimaryAddress";

        public static String FIRSTNAME = "FirstName";
        public static String LASTNAME = "LastName";
        public static String LOGIN_ID = "LoginId";
        public static String PASSWORD = "Password";
        public static String PASSWORD_SALT = "PasswordSalt";
        public static String EMAIL_ADDRESS = "EmailAddress";

        public static String STAFF_ID = "StaffId";
        public static String START_DATE = "StartDate";
        public static String END_DATE = "EndDate";
        public static String IS_DOCTOR = "IsDoctor";

        public static String CONTENT = "Content";
        public static String CONSULTATION_DATE = "ConsultationDate";
        public static String CONSULTANT_ID = "ConsultantId";
        public static String CONSULTANT_NAME = "ConsultantName";
        public static String CONSULTANT_PRACTICE = "ConsultantPractice";

        public static String APPOINTMENT_DATE = "AppointmentDate";
        public static String DURATION = "Duration";
        public static String ATTENDED = "Attended";
        public static String ROLE_TYPE = "RoleType";

    }
}
