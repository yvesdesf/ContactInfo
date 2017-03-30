using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactInfo
{
    public class Contact
    {
        private List<Email> _EmailList = new List<Email>();
        private List<Location> _LocationList = new List<Location>();
        private List<Phone> _PhoneList = new List<Phone>();

        public int Index { get; set; }
        public int C_ID { get; set; }
        public string Title { get; set; }
        public string fName { get; set; }
        public string mName { get; set; }
        public string lName { get; set; }
        public string bName { get; set; }
        public string Notes { get; set; }
        public string Photo { get; set; }

        public List<Email> EmailList
        {
            get { return _EmailList; }
            set { _EmailList = value; }
        }

        public List<Location> LocationList
        {
            get { return _LocationList; }
            set { _LocationList = value; }
        }

        public List<Phone> PhoneList
        {
            get { return _PhoneList; }
            set { _PhoneList = value; }
        }

        public string GetFullName() { return fName + " " + lName; }
    }

    public class Location
    {
        public int AD_ID { get; set; }
        public int C_ID { get; set; }
        public int LocationType { get; set; }
        public string street1 { get; set; }
        public string street2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string custom { get; set; }
    }

    public class Phone
    {
        public int PH_ID { get; set; }
        public int C_ID { get; set; }
        public int PhoneType { get; set; }
        public string phoneNum { get; set; }
        public string custom { get; set; }
    }

    public class Email
    {
        public int E_ID { get; set; }
        public int C_ID { get; set; }
        public int EmailType { get; set; }
        public string EAddr { get; set; }
        public string custom { get; set; }
    }
}
