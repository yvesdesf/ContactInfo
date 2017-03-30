using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Diagnostics;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace ContactInfo
{
    
    public partial class Form1 : Form
    {
       //public string dbFile = Application.StartupPath + "\\Contacts.db3";
        public string dbFile = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\Contacts.db3";
        CultureInfo ci = CultureInfo.InvariantCulture;
        public List<Contact> ContactList = new List<Contact>();
        private Contact OneContact = new Contact();

        private BindingSource bsContact = new BindingSource();
        BindingSource bsLocation = new BindingSource();
        BindingSource bsPhone = new BindingSource();
        BindingSource bsEmail = new BindingSource();

        Email ECurrent = new Email();
        Phone PCurrent = new Phone();
        Location LCurrent = new Location();

        //int DBversion = 0;

        public Form1()
        {
            InitializeComponent();
            CreateDB();
            CreateTables();
            VerifyData();
            FillCombo();
            GetData();
            BindData();
            lblDb.Text = dbFile;

        }

        private void CreateDB()
        {
            if (!(File.Exists(dbFile)))
            {
                try
                {
                    SQLiteConnection.CreateFile(dbFile);        // Create the file which will be hosting our database
                    string SQL = "CREATE TABLE IF NOT EXISTS [pgOptions] (";
                    SQL += "[OptName] TEXT  NOT NULL PRIMARY KEY";
                    SQL += ",[StrF1] TEXT  NULL";
                    SQL += ",[StrF2] TEXT  NULL";
                    SQL += ",[StrF3] TEXT  NULL";
                    SQL += ",[StrF4] TEXT  NULL";
                    SQL += ",[StrF5] TEXT  NULL";
                    SQL += ",[StrF6] TEXT  NULL";
                    SQL += ",[StrF7] TEXT  NULL";
                    SQL += ",[IntN1] INTEGER  NULL";
                    SQL += ",[IntN2] INTEGER  NULL";
                    SQL += ",[IntN3] INTEGER  NULL";
                    SQL += ",[IntN4] INTEGER  NULL";
                    SQL += ",[IntN5] INTEGER  NULL";
                    SQL += ",[IntN6] INTEGER  NULL";
                    SQL += ",[IntN7] INTEGER  NULL";
                    SQL += ",[Num1] REAL  NULL";
                    SQL += ",[Num2] REAL  NULL";
                    SQL += ",[Num3] REAL  NULL";
                    SQL += ",[Num4] REAL  NULL";
                    SQL += ",[Num5] REAL  NULL";
                    SQL += ",[Num6] REAL  NULL";
                    SQL += ",[Num7] REAL  NULL";
                    SQL += ")";
                    if (RunNonQuery(SQL) == -1)
                    {
                        throw new SQLiteException("RunNonQuery Error");
                    }

                    SQL = "INSERT INTO pgOptions (OptName,StrF1,IntN1) Values ('DBVersion','Database version',1)";     // Add database version
                    if (RunNonQuery(SQL) == -1)
                    {
                        throw new SQLiteException("RunNonQuery Error");
                    }

                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    LogError(ex.Message);

                }

            }
        }

        private void CreateTables()
        {
            try
            {
                string SQL = "";
                SQL = @"CREATE TABLE IF NOT EXISTS [Contact] (
			C_ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
			,Title  TEXT  NULL
			,fName  TEXT  NULL
			,mName  TEXT  NULL
			,lName  TEXT  NULL
			,bName  TEXT  NULL
            ,DOB  TEXT  NULL
			,Notes  TEXT  NULL
			,Photo  TEXT  NULL
			);";
                if (RunNonQuery(SQL) == -1)
                {
                    throw new SQLiteException("RunNonQuery Error");
                }
                SQL = @"CREATE TABLE IF NOT EXISTS  [Locations] (
			AD_ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
			,C_ID INTEGER not null
			,LocationType INTEGER null
			,street1  TEXT  NULL
			,street2  TEXT  NULL
			,city  TEXT  NULL
			,state  TEXT  NULL
			,zip  TEXT  NULL
			,country  TEXT  NULL
            ,Custom  TEXT  NULL
			)";
                if (RunNonQuery(SQL) == -1)
                {
                    throw new SQLiteException("RunNonQuery Error");
                }
                SQL = @"CREATE TABLE IF NOT EXISTS [Phones] (
			PH_ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
			,C_ID INTEGER not null
			,PhoneType INTEGER null
			,phoneNum  TEXT  NULL
            ,Custom  TEXT  NULL
			)";
                if (RunNonQuery(SQL) == -1)
                {
                    throw new SQLiteException("RunNonQuery Error");
                }
                SQL = @"CREATE TABLE IF NOT EXISTS [Types] (
			Ty_ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
			,[Context] TEXT null
			,[Typetext]  TEXT  NULL
            ,[LinkID] INTEGER NULL
			)";
                if (RunNonQuery(SQL) == -1)
                {
                    throw new SQLiteException("RunNonQuery Error");
                }

                SQL = @"CREATE TABLE IF NOT EXISTS [Emails] (
			E_ID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT
			,[C_ID] INTEGER not null
			,[EmailType]  INTEGER  NULL
            ,[Email]  TEXT  NULL
            ,[Custom]  TEXT  NULL
			)";
                if (RunNonQuery(SQL) == -1)
                {
                    throw new SQLiteException("RunNonQuery Error");
                }


            }

            catch (Exception ex)
            {

                Debug.Print(ex.Message);
                LogError(ex.Message);
            }


        } //end CreateTables

        private void VerifyData()
        {
            string SQL = "";
            SQLiteDataReader DataRead = null;
            SQLiteConnection con = null;

            try
            {
                con = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;");

                con.Open();
                SQL = "select * from [Types]";
                DataRead = FillDataReader(SQL, con);

                if (!(DataRead.HasRows))
                {
                    SQL = @"insert into [Types]
					(
					[Context]
					,[Typetext]
					)
					Values
					('LOCATION','Home')
					,('LOCATION','Work')
					,('LOCATION','other')
					,('LOCATION','Custom')
					,('PHONE','Home')
					,('PHONE','Mobile')
					,('PHONE','Work')
					,('PHONE','Main')
					,('PHONE','Work Fax')
					,('PHONE','Home Fax')
					,('PHONE','Pager')
					,('PHONE','other')
					,('PHONE','Custom')
                    ,('EMAIL','Home')
					,('EMAIL','Work')
					,('EMAIL','other')
					,('EMAIL','Custom')
					";
                    if (RunNonQuery(SQL) == -1)
                    {
                        throw new SQLiteException("RunNonQuery Error");
                    }
                }
                DataRead.Dispose();
                con.Close();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                LogError(ex.Message);
                //throw;
            }

            if (con != null)
            {
                con.Close();
            }
        }

        private int RunNonQuery(String SQL)
        {
            int RowNum = -1;
            SQLiteConnection con = null;
            SQLiteTransaction tr = null;
            SQLiteCommand cmd = null;

            try
            {
                con = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;");

                con.Open();

                tr = con.BeginTransaction();
                cmd = con.CreateCommand();

                cmd.Transaction = tr;
                cmd.CommandText = SQL;
                RowNum = cmd.ExecuteNonQuery();

                tr.Commit();
                con.Close();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                LogError(ex.Message);
                if (tr != null)
                {
                    tr.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
                //throw;
            }

            return RowNum;
        } //end RunNonQuery


        private int RunScalar(String SQL)
        {
            int RowNum = -1;
            object obj;
            SQLiteConnection con = null;
            SQLiteTransaction tr = null;
            SQLiteCommand cmd = null;

            try
            {
                con = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;");

                con.Open();

                tr = con.BeginTransaction();
                cmd = con.CreateCommand();

                cmd.Transaction = tr;
                cmd.CommandText = SQL;
                obj = cmd.ExecuteScalar();
                RowNum = Mod.MakeInt(obj);

                if (RowNum == -1)
                {
                    throw new SQLiteException("");
                }
                tr.Commit();
                con.Close();

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                LogError(ex.Message);
                if (tr != null)
                {
                    tr.Rollback();
                }
                if (con != null)
                {
                    con.Close();
                }
                //throw;
            }

            return RowNum;
        } //end RunNonQuery

        private SQLiteDataReader FillDataReader(String SQL, SQLiteConnection con)
        {
            SQLiteDataReader DataReader = null;
            //SQLiteConnection con = null;
            SQLiteCommand cmd = null;
            try
            {

                cmd = con.CreateCommand();

                cmd.CommandText = SQL;
                DataReader = cmd.ExecuteReader();
                //con.Close();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                LogError(ex.Message);

                //throw;
            }

            return DataReader;
        }
        private void LogError(String ErrorText)
        {
            //string path = Application.StartupPath + "\\" + Application.ProductName + "Error.log";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments) + "\\" + Application.ProductName + "Error.log";
            string DateStr = DateTime.Now.ToString("dd/MM/yyyy", ci) + " " + DateTime.Now.ToShortTimeString();
            StreamWriter sw = null;
            try
            {
                if (File.Exists(path) == false)
                {
                    sw = File.CreateText(path);
                    sw.WriteLine(ErrorText + " | " + DateStr);
                    sw.Flush();
                    sw.Close();
                }
                else
                {
                    sw = File.AppendText(path);
                    sw.WriteLine(ErrorText + " | " + DateStr);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                if (sw != null)
                {
                    sw.Dispose();
                }

            }
        }// end LogError

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutCM About = new AboutCM();
            About.Show();

        } 

        private void GetData()
        {
            string SQL = "";
            SQLiteDataReader DataRead = null;
            SQLiteDataReader DetailRead = null;
            SQLiteConnection con = null;
            int index = 0;
            try
            {
                SQL = @"select C_ID,Title,fName,mName
				,lName,bName,Notes,Photo
				from contact";
                con = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;");

                con.Open();

                DataRead = FillDataReader(SQL, con);

                if (DataRead.HasRows)
                {
                    //
                    while (DataRead.Read())
                    {
                        //
                        OneContact = new Contact();
                        OneContact.Index = index;
                        OneContact.C_ID = Mod.checkNullInt(DataRead["C_ID"]);
                        OneContact.Title = Mod.checkNullString(DataRead["Title"]);
                        OneContact.fName = Mod.checkNullString(DataRead["fName"]);
                        OneContact.mName = Mod.checkNullString(DataRead["mName"]);
                        OneContact.lName = Mod.checkNullString(DataRead["lName"]);
                        OneContact.bName = Mod.checkNullString(DataRead["bName"]);
                        OneContact.Notes = Mod.checkNullString(DataRead["Notes"]);
                        OneContact.Photo = Mod.checkNullString(DataRead["Photo"]);
                        
                       

                        //location
                        SQL = @"select AD_ID,C_ID,LocationType,street1
				            ,street2,city,state,zip,country,custom
				               from Locations where C_ID = " + OneContact.C_ID.ToString();
                        DetailRead = FillDataReader(SQL, con);
                        List<Location> LocationList = new List<Location>();

                        if (DetailRead.HasRows)
                        {
                            while (DetailRead.Read())
                            {
                                Location OneLocation = new Location();
                                
                                OneLocation.AD_ID = Mod.checkNullInt(DetailRead["AD_ID"]);
                                OneLocation.C_ID = Mod.checkNullInt(DetailRead["C_ID"]);
                                OneLocation.LocationType = Mod.checkNullInt(DetailRead["LocationType"]);
                                OneLocation.street1 = Mod.checkNullString(DetailRead["street1"]);
                                OneLocation.street2 = Mod.checkNullString(DetailRead["street2"]);
                                OneLocation.city = Mod.checkNullString(DetailRead["city"]);
                                OneLocation.state = Mod.checkNullString(DetailRead["state"]);
                                OneLocation.zip = Mod.checkNullString(DetailRead["zip"]);
                                OneLocation.country = Mod.checkNullString(DetailRead["country"]);
                                OneLocation.custom = Mod.checkNullString(DetailRead["custom"]);
                                LocationList.Add(OneLocation);
                            }
                        }
                        OneContact.LocationList = LocationList;

                        //phone
                        SQL = @"select 
				            PH_ID,C_ID,PhoneType,phoneNum,custom
				               from Phones where C_ID = " + OneContact.C_ID.ToString();
                        DetailRead = FillDataReader(SQL, con);
                        List<Phone> PhoneList = new List<Phone>();

                        if (DetailRead.HasRows)
                        {
                            while (DetailRead.Read())
                            {
                                Phone OnePhone = new Phone();
                                OnePhone.PH_ID = Mod.checkNullInt(DetailRead["PH_ID"]);
                                OnePhone.C_ID = Mod.checkNullInt(DetailRead["C_ID"]);
                                OnePhone.PhoneType = Mod.checkNullInt(DetailRead["PhoneType"]);
                                OnePhone.phoneNum = Mod.checkNullString(DetailRead["phoneNum"]);
                                OnePhone.custom = Mod.checkNullString(DetailRead["custom"]);

                                PhoneList.Add(OnePhone);
                            }
                        }
                        OneContact.PhoneList = PhoneList;

                        //email
                        SQL = @"select 
				            E_ID,C_ID,EmailType,Email,custom
				               from Emails where C_ID = " + OneContact.C_ID.ToString();
                        DetailRead = FillDataReader(SQL, con);
                        List<Email> EmailList = new List<Email>();

                        if (DetailRead.HasRows)
                        {
                            while (DetailRead.Read())
                            {
                                Email OneEmail = new Email();
                                OneEmail.E_ID = Mod.checkNullInt(DetailRead["E_ID"]);
                                OneEmail.C_ID = Mod.checkNullInt(DetailRead["C_ID"]);
                                OneEmail.EmailType = Mod.checkNullInt(DetailRead["EmailType"]);
                                OneEmail.EAddr = Mod.checkNullString(DetailRead["Email"]);
                                OneEmail.custom = Mod.checkNullString(DetailRead["custom"]);

                                EmailList.Add(OneEmail);
                            }
                        }
                        OneContact.EmailList = EmailList;

                        ContactList.Add(OneContact);
                        index += 1;
                    } // DataRead
                }
        
                  
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                LogError(ex.Message);
            }

            // cleanup
            if (DataRead != null)
            {
                DataRead.Dispose();
            }
            if (DetailRead != null)
            {
                DetailRead.Dispose();
            }
            if (con != null)
            {
                con.Close();
            }
        }

        private void BindData()
        {
            try
            {
                bsContact.DataSource = ContactList;
                NavContact.BindingSource = bsContact;

                OneContact = new Contact();
                OneContact = (Contact)bsContact.Current;
                MakePhoto();
                if (OneContact.EmailList.Count > 0)
                {
                    ECurrent = OneContact.EmailList[0];
                }
                if (OneContact.PhoneList.Count > 0)
                {
                    PCurrent = OneContact.PhoneList[0];
                }
                if (OneContact.LocationList.Count > 0)
                {
                    LCurrent = OneContact.LocationList[0];
                }

                txtfName.DataBindings.Add("Text", bsContact, "fName");
                txtlName.DataBindings.Add("Text", bsContact, "lName");
                txtTitle.DataBindings.Add("Text", bsContact, "Title");
                txtmName.DataBindings.Add("Text", bsContact, "mName");
                txtbName.DataBindings.Add("Text", bsContact, "bName");
                txtNotes.DataBindings.Add("Text", bsContact, "Notes");
                
                bsLocation.DataSource = bsContact;
                bsLocation.DataMember = "LocationList";
                navLocation.BindingSource = bsLocation;

                cboLoc.DataBindings.Add("SelectedIndex", bsLocation, "LocationType");
                txtstreet1.DataBindings.Add("Text", bsLocation, "street1");
                txtstreet2.DataBindings.Add("Text", bsLocation, "street2");
                txtCity.DataBindings.Add("Text", bsLocation, "city");
                txtState.DataBindings.Add("Text", bsLocation, "state");
                txtZip.DataBindings.Add("Text", bsLocation, "zip");
                txtCountry.DataBindings.Add("Text", bsLocation, "country");
                txtLocCustom.DataBindings.Add("Text", bsLocation, "custom");

                bsPhone.DataSource = bsContact;
                bsPhone.DataMember = "PhoneList";
                navPhone.BindingSource = bsPhone;

                txtphoneNum.DataBindings.Add("Text", bsPhone, "PhoneNum");
                cboPhone.DataBindings.Add("SelectedIndex", bsPhone, "PhoneType");
                txtPCustom.DataBindings.Add("Text", bsPhone, "custom");

                bsEmail.DataSource = bsContact;
                bsEmail.DataMember = "EmailList";
                navEmail.BindingSource = bsEmail;

                txtEmail.DataBindings.Add("Text", bsEmail, "EAddr");
                cboEmail.DataBindings.Add("SelectedIndex", bsEmail, "EmailType");
                txtECustom.DataBindings.Add("Text", bsEmail, "custom");

            }
            catch (Exception ex)
            {
                
               Debug.Print(ex.Message);
               LogError(ex.Message);
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FillCombo()
        {
            string SQL = "";
            SQLiteDataReader DataRead = null;
            SQLiteConnection con = null;

            try
            {
                con = new SQLiteConnection("Data Source=" + dbFile + ";Version=3;");

                con.Open();

                //cbPhone
                SQL = "select Typetext from Types where Context = 'PHONE'";
                DataRead = FillDataReader(SQL, con);
                while (DataRead.Read())
                {
                    cboPhone.Items.Add(Mod.checkNullString(DataRead[0]));
                }

                //cboLoc
                SQL = "select Typetext from Types where Context = 'LOCATION'";
                DataRead = FillDataReader(SQL, con);
                while (DataRead.Read())
                {
                    cboLoc.Items.Add(Mod.checkNullString(DataRead[0]));
                }

                //cboEmail
                SQL = "select Typetext from Types where Context = 'EMAIL'";
                DataRead = FillDataReader(SQL, con);
                while (DataRead.Read())
                {
                    cboEmail.Items.Add(Mod.checkNullString(DataRead[0]));
                }

            }
            catch (Exception ex)
            {
                
                Debug.Print(ex.Message);
               LogError(ex.Message);
            }

            if (DataRead != null)
            {
                DataRead.Dispose();
            }
            if (con != null)
            {
                con.Close();
            }
        }

        private void cboLoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboLoc.SelectedIndex == 3)
            {
                txtLocCustom.Visible = true;
            }
            else
            {
                txtLocCustom.Visible = false;
            }
        }

        

        private void cboPhone_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboPhone.SelectedIndex == 14)
            {
                txtPCustom.Visible = true;
            }
            else
            {
                txtPCustom.Visible = false;
            }
        }

        private void cboEmail_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboEmail.SelectedIndex == 3)
            {
                txtECustom.Visible = true;
            }
            else
            {
                txtECustom.Visible = false;
            }
        }

        

#region ManageEmail
        
        private void Email_MouseDown(object sender, MouseEventArgs e)
        {
            if (EmailPositionItem.Text == "0")
            {
                return;
            }
            EmailSaveItem.Enabled = true;
            DisableEmail();
        }

        private void DisableEmail()
        {
            EmailMoveFirstItem.Enabled = false;
            EmailMovePreviousItem.Enabled = false;
            EmailMoveNextItem.Enabled = false;
            EmailMoveLastItem.Enabled = false;
            EmailAddNewItem.Enabled = false;
        }

        private void EnableEmail()
        {
            EmailMoveFirstItem.Enabled = true;
            EmailMovePreviousItem.Enabled = true;
            EmailMoveNextItem.Enabled = true;
            EmailMoveLastItem.Enabled = true;
            EmailAddNewItem.Enabled = true;
        }

        private void EmailAddNewItem_Click(object sender, EventArgs e)
        {
            ECurrent = new Email();
            //ECurrent = OneContact.EmailList[(Mod.checkNullInt(EmailPositionItem.Text) - 1)];
            ECurrent.E_ID = -1;
            DisableEmail();
        }

        private void EmailSaveItem_Click(object sender, EventArgs e)
        {
            string SQL = "";

            try
            {
               if (ECurrent.E_ID == -1)
               {
                   ECurrent.C_ID = OneContact.C_ID;
                   ECurrent.EAddr = txtEmail.Text;
                   ECurrent.custom = txtECustom.Text;
                   ECurrent.EmailType = cboEmail.SelectedIndex;

                   SQL = "insert into Emails (C_ID,EmailType,Email,custom) values (";
                   SQL += ECurrent.C_ID + ",";
                   SQL += ECurrent.EmailType + ",";
                   SQL += "'" + Mod.CheckApos(ECurrent.EAddr) + "',";
                   SQL += "'" + Mod.CheckApos(ECurrent.custom) + "'); SELECT last_insert_rowid() FROM Emails";

                   ECurrent.E_ID = RunScalar(SQL);
                   
               }
               else
               {
                   ECurrent.EAddr = txtEmail.Text;
                   ECurrent.custom = txtECustom.Text;
                   ECurrent.EmailType = cboEmail.SelectedIndex;

                   SQL = "Update Emails ";
                   SQL += "set EmailType = " + ECurrent.EmailType + ",";
                   SQL += "Email = '" + Mod.CheckApos(ECurrent.EAddr) + "',";
                   SQL += "custom = '" + Mod.CheckApos(ECurrent.custom) + "' where E_ID = " + ECurrent.E_ID;

                   RunNonQuery(SQL);
               }
                
            }
            catch (Exception ex)
            {
               Debug.Print(ex.Message);
               LogError(ex.Message);
            }
            EnableEmail();
            EmailSaveItem.Enabled = false;
        }

        private void EmailDeleteItem_Click(object sender, EventArgs e)
        {
            string SQL = "";
            if (EmailSaveItem.Enabled == false)
            {
                try
                {
                    SQL = "delete from emails where E_ID = " + ECurrent.E_ID;
                    RunNonQuery(SQL);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    LogError(ex.Message); 
                    
                }
            }
            EnableEmail();
            EmailSaveItem.Enabled = false;
        }

        private void EmailMoveFirstItem_Click(object sender, EventArgs e)
        {
            ECurrent = new Email();
            ECurrent = OneContact.EmailList[(Mod.checkNullInt(EmailPositionItem.Text) - 1)];
        }

        private void EmailMovePreviousItem_Click(object sender, EventArgs e)
        {
            ECurrent = new Email();
            ECurrent = OneContact.EmailList[(Mod.checkNullInt(EmailPositionItem.Text) - 1)];
        }

        private void EmailMoveLastItem_Click(object sender, EventArgs e)
        {
            ECurrent = new Email();
            ECurrent = OneContact.EmailList[(Mod.checkNullInt(EmailPositionItem.Text) - 1)];
        }

        private void EmailMoveNextItem_Click(object sender, EventArgs e)
        {
            ECurrent = new Email();
            ECurrent = OneContact.EmailList[(Mod.checkNullInt(EmailPositionItem.Text) - 1)];
        }

        private void EmailPositionItem_EnabledChanged(object sender, EventArgs e)
        {
            if (EmailPositionItem.Enabled == false)
            {
                cboEmail.Enabled = false;
                txtECustom.Enabled = false;
                txtEmail.Enabled = false;
            }
            else
            {
                cboEmail.Enabled = true;
                txtECustom.Enabled = true;
                txtEmail.Enabled = true;
            }
        }

#endregion

#region ManagePhone
        private void PhoneMoveFirstItem_Click(object sender, EventArgs e)
        {
            PCurrent = new Phone();
            PCurrent = OneContact.PhoneList[(Mod.checkNullInt(PhonePositionItem.Text) - 1)];

        }

        private void PhoneMovePreviousItem_Click(object sender, EventArgs e)
        {
            PCurrent = new Phone();
            PCurrent = OneContact.PhoneList[(Mod.checkNullInt(PhonePositionItem.Text) - 1)];
        }

        private void PhoneMoveNextItem_Click(object sender, EventArgs e)
        {
            PCurrent = new Phone();
            PCurrent = OneContact.PhoneList[(Mod.checkNullInt(PhonePositionItem.Text) - 1)];
        }

        private void PhoneMoveLastItem_Click(object sender, EventArgs e)
        {
            PCurrent = new Phone();
            PCurrent = OneContact.PhoneList[(Mod.checkNullInt(PhonePositionItem.Text) - 1)];
        }

        private void disablePhone()
        {
            PhoneMoveFirstItem.Enabled = false;
            PhoneMovePreviousItem.Enabled = false;
            PhoneMoveNextItem.Enabled = false;
            PhoneMoveLastItem.Enabled = false;
            PhoneAddNewItem.Enabled = false;
        }

        private void enablePhone()
        {
            PhoneMoveFirstItem.Enabled = true;
            PhoneMovePreviousItem.Enabled = true;
            PhoneMoveNextItem.Enabled = true;
            PhoneMoveLastItem.Enabled = true;
            PhoneAddNewItem.Enabled = true;
        }

        private void Phone_MouseDown(object sender, MouseEventArgs e)
        {
            if (PhonePositionItem.Text == "0")
            {
                return;
            }
            PhoneSaveItem.Enabled = true;
            disablePhone();
        }

        private void PhoneAddNewItem_Click(object sender, EventArgs e)
        {
            PCurrent = new Phone();
            //PCurrent = OneContact.PhoneList[(Mod.checkNullInt(PhonePositionItem.Text) - 1)];
            PCurrent.PH_ID = -1;
            disablePhone();
        }

        private void PhoneDeleteItem_Click(object sender, EventArgs e)
        {
            string SQL = "";
            if (PhoneSaveItem.Enabled == false)
            {
                try
                {
                    SQL = "delete from Phones where PH_ID = " + PCurrent.PH_ID;
                    RunNonQuery(SQL);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    LogError(ex.Message);

                }
            }
            enablePhone();
            PhoneSaveItem.Enabled = false;
        }

        private void PhoneSaveItem_Click(object sender, EventArgs e)
        {
            string SQL = "";

            try
            {
                if (PCurrent.PH_ID == -1)
                {
                    PCurrent.C_ID = OneContact.C_ID;
                    PCurrent.phoneNum = txtphoneNum.Text;
                    PCurrent.custom = txtPCustom.Text;
                    PCurrent.PhoneType = cboPhone.SelectedIndex;

                    SQL = "insert into Phones (C_ID,PhoneType,phoneNum,custom) values (";
                    SQL += PCurrent.C_ID + ",";
                    SQL += PCurrent.PhoneType + ",";
                    SQL += "'" + Mod.CheckApos(PCurrent.phoneNum) + "',";
                    SQL += "'" + Mod.CheckApos(PCurrent.custom) + "'); SELECT last_insert_rowid() FROM Phones";
                    PCurrent.PH_ID = RunScalar(SQL);
                    //RunNonQuery(SQL);
                }
                else
                {
                    PCurrent.phoneNum = txtphoneNum.Text;
                    PCurrent.custom = txtPCustom.Text;
                    PCurrent.PhoneType = cboPhone.SelectedIndex;

                    SQL = "update Phones  set ";
                    SQL += "PhoneType = " + PCurrent.PhoneType + ",";
                    SQL += "phoneNum = '" + Mod.CheckApos(PCurrent.phoneNum) + "',";
                    SQL += "custom = '" + Mod.CheckApos(PCurrent.custom) + "'";

                    RunNonQuery(SQL);
                }
                
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                LogError(ex.Message);
            }
            enablePhone();
            PhoneSaveItem.Enabled = false;
        }

        private void PhonePositionItem_EnabledChanged(object sender, EventArgs e)
        {
            if (PhonePositionItem.Enabled == false)
            {
                cboPhone.Enabled = false;
                txtPCustom.Enabled = false;
                txtphoneNum.Enabled = false;
            }
            else
            {
                cboPhone.Enabled = true;
                txtPCustom.Enabled = true;
                txtphoneNum.Enabled = true;
            }
        }

#endregion

#region ManageLocation
        private void LocMoveFirstItem_Click(object sender, EventArgs e)
        {
            LCurrent = new Location();
            LCurrent = OneContact.LocationList[(Mod.checkNullInt(LocPositionItem.Text) - 1)];

        }

        private void LocMovePreviousItem_Click(object sender, EventArgs e)
        {
            LCurrent = new Location();
            LCurrent = OneContact.LocationList[(Mod.checkNullInt(LocPositionItem.Text) - 1)];
        }

        private void LocMoveNextItem_Click(object sender, EventArgs e)
        {
            LCurrent = new Location();
            LCurrent = OneContact.LocationList[(Mod.checkNullInt(LocPositionItem.Text) - 1)];
        }

        private void LocMoveLastItem_Click(object sender, EventArgs e)
        {
            LCurrent = new Location();
            LCurrent = OneContact.LocationList[(Mod.checkNullInt(LocPositionItem.Text) - 1)];
        }

        private void LocAddNewItem_Click(object sender, EventArgs e)
        {
            LCurrent = new Location();
            //LCurrent = OneContact.LocationList[(Mod.checkNullInt(LocPositionItem.Text) - 1)];
            LCurrent.AD_ID = -1;
            DisableLocation();
        }

        private void LocDeleteItem_Click(object sender, EventArgs e)
        {
            string SQL = "";
            if (LocSaveItem.Enabled == false)
            {
                try
                {
                    SQL = "delete from Locations where AD_ID = " + LCurrent.AD_ID;
                    RunNonQuery(SQL);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    LogError(ex.Message);

                }
            }
            enablePhone();
            PhoneSaveItem.Enabled = false;
        }

        private void LocSaveItem_Click(object sender, EventArgs e)
        {
            string SQL = "";

            try
            {
                LCurrent.LocationType = cboLoc.SelectedIndex;
                LCurrent.custom = txtLocCustom.Text;
                LCurrent.street1 = txtstreet1.Text;
                LCurrent.street2 = txtstreet2.Text;
                LCurrent.city = txtCity.Text;
                LCurrent.state = txtState.Text;
                LCurrent.zip = txtZip.Text;
                LCurrent.country = txtCountry.Text;

                if (LCurrent.AD_ID == -1)
                {
                    LCurrent.C_ID = OneContact.C_ID;

                    SQL = "insert into Locations (C_ID,LocationType,street1,street2,city,state,zip,country,custom) values (";
                    SQL += LCurrent.C_ID + ",";
                    SQL += LCurrent.LocationType + ",";
                    SQL += "'" + Mod.CheckApos(LCurrent.street1) + "',";
                    SQL += "'" + Mod.CheckApos(LCurrent.street2) + "',";
                    SQL += "'" + Mod.CheckApos(LCurrent.city) + "',";
                    SQL += "'" + Mod.CheckApos(LCurrent.state) + "',";
                    SQL += "'" + Mod.CheckApos(LCurrent.zip) + "',";
                    SQL += "'" + Mod.CheckApos(LCurrent.country) + "',";
                    SQL += "'" + Mod.CheckApos(LCurrent.custom) + "'); SELECT last_insert_rowid() FROM Locations";

                    LCurrent.AD_ID = RunScalar(SQL);
                }
                else
                {


                    SQL = "update Locations  set ";
                    SQL += "LocationType = " + LCurrent.LocationType + ",";
                    SQL += "street1 = '" + Mod.CheckApos(LCurrent.street1) + "',";
                    SQL += "street2 = '" + Mod.CheckApos(LCurrent.street2) + "',";
                    SQL += "city = '" + Mod.CheckApos(LCurrent.city) + "',";
                    SQL += "state = '" + Mod.CheckApos(LCurrent.state) + "',";
                    SQL += "zip = '" + Mod.CheckApos(LCurrent.zip) + "',";
                    SQL += "country = '" + Mod.CheckApos(LCurrent.country) + "',";
                    SQL += "custom = '" + Mod.CheckApos(LCurrent.custom) + "' where AD_ID = " + LCurrent.AD_ID;

                    RunNonQuery(SQL);
                }

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                LogError(ex.Message);
            }
            EnableLocation();
            LocSaveItem.Enabled = false;
        }

        private void Location_MouseDown(object sender, MouseEventArgs e)
        {
            if (LocPositionItem.Text == "0")
            {
                return;
            }
            LocSaveItem.Enabled = true;
            DisableLocation();
        }
        
        private void EnableLocation()
        {
            LocMoveFirstItem.Enabled = true;
            LocMovePreviousItem.Enabled = true;
            LocMoveNextItem.Enabled = true;
            LocMoveLastItem.Enabled = true;
            LocAddNewItem.Enabled = true;
        }

        private void DisableLocation()
        {
            LocMoveFirstItem.Enabled = false;
            LocMovePreviousItem.Enabled = false;
            LocMoveNextItem.Enabled = false;
            LocMoveLastItem.Enabled = false;
            LocAddNewItem.Enabled = false;
            
        }

        private void LocPositionItem_EnabledChanged(object sender, EventArgs e)
        {
            if (LocPositionItem.Enabled == false)
            {
                cboLoc.Enabled = false;
                txtLocCustom.Enabled = false;
                txtstreet1.Enabled = false;
                txtstreet2.Enabled = false;
                txtCity.Enabled = false;
                txtState.Enabled = false;
                txtZip.Enabled = false;
                txtCountry.Enabled = false;
            }
            else
            {
                cboLoc.Enabled = true;
                txtLocCustom.Enabled = true;
                txtstreet1.Enabled = true;
                txtstreet2.Enabled = true;
                txtCity.Enabled = true;
                txtState.Enabled = true;
                txtZip.Enabled = true;
                txtCountry.Enabled = true;
            }
        }

#endregion

#region ManageContact
        private void ContactMoveNextItem_Click(object sender, EventArgs e)
        {
            OneContact = new Contact();
            OneContact = (Contact)bsContact.Current;
            MakePhoto();
            
        }

        private void ContactMovePreviousItem_Click(object sender, EventArgs e)
        {
            OneContact = new Contact();
            OneContact = (Contact)bsContact.Current;
            MakePhoto();
        }

        private void ContactMoveLastItem_Click(object sender, EventArgs e)
        {
            OneContact = new Contact();
            OneContact = (Contact)bsContact.Current;
            MakePhoto();
        }

        private void ContactMoveFirstItem_Click(object sender, EventArgs e)
        {
            OneContact = new Contact();
            OneContact = (Contact)bsContact.Current;
            MakePhoto();
        }

        private void ContactAddNewItem_Click(object sender, EventArgs e)
        {
            OneContact = new Contact();
            OneContact.C_ID = -1;
            DisableContact();
            ContactSaveItem.Enabled = true;
            MakePhoto();

        }

        private void ContactDeleteItem_Click(object sender, EventArgs e)
        {
            // delete also in location,phone,email
            string SQL = "";
            if (ContactSaveItem.Enabled == false)
            {
                try
                {
                    SQL = "delete from Locations where C_ID = " + OneContact.C_ID + ";";
                    SQL += "delete from Phones where C_ID = " + OneContact.C_ID + ";";
                    SQL += "delete from emails where C_ID = " + OneContact.C_ID + ";";
                    SQL += "delete from Contact where C_ID = " + OneContact.C_ID + ";";
                    RunNonQuery(SQL);
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                    LogError(ex.Message);

                }
            }
            EnableContact();
            ContactSaveItem.Enabled = false;
        }

        private void ContactSaveItem_Click(object sender, EventArgs e)
        {
            string SQL = "";
            try
            {
                if (txtfName.Text == "")
                {
                    MessageBox.Show("Please enter a firstName", "Required");
                    txtfName.Focus();
                    return;
                }

                OneContact.Title = txtTitle.Text;
                OneContact.fName = txtfName.Text;
                OneContact.mName = txtmName.Text;
                OneContact.lName = txtlName.Text;
                OneContact.bName = txtbName.Text;
                OneContact.Notes = txtNotes.Text;
                //OneContact.Photo = "";

                if (OneContact.C_ID == -1)
                {
                    SQL = "insert into Contact (Title,fName,mName,lName,bName,Notes,Photo) values (";
                    SQL += "'" + Mod.CheckApos(OneContact.Title) + "',";
                    SQL += "'" + Mod.CheckApos(OneContact.fName) + "',";
                    SQL += "'" + Mod.CheckApos(OneContact.mName) + "',";
                    SQL += "'" + Mod.CheckApos(OneContact.lName) + "',";
                    SQL += "'" + Mod.CheckApos(OneContact.bName) + "',";
                    SQL += "'" + Mod.CheckApos(OneContact.Notes) + "',";
                    SQL += "'" + OneContact.Photo + "'); SELECT last_insert_rowid() FROM contact";

                    OneContact.C_ID = RunScalar(SQL);
                }
                else
                {
                    SQL = "update Contact  set ";
                    SQL += "Title = '" + Mod.CheckApos(OneContact.Title) + "',";
                    SQL += "fName = '" + Mod.CheckApos(OneContact.fName) + "',";
                    SQL += "mName = '" + Mod.CheckApos(OneContact.mName) + "',";
                    SQL += "lName = '" + Mod.CheckApos(OneContact.lName) + "',";
                    SQL += "bName = '" + Mod.CheckApos(OneContact.bName) + "',";
                    SQL += "Notes = '" + Mod.CheckApos(OneContact.Notes) + "',";
                    SQL += "Photo = '" + OneContact.Photo + "' where C_ID = " + OneContact.C_ID;

                    RunNonQuery(SQL);

                }
            }
            catch (Exception ex)
            {

                Debug.Print(ex.Message);
                LogError(ex.Message);
            }

            EnableContact();
            ContactSaveItem.Enabled = false;
        }

        private void EnableContact()
        {
            ContactMoveFirstItem.Enabled = true;
            ContactMovePreviousItem.Enabled = true;
            ContactMoveNextItem.Enabled = true;
            ContactMoveLastItem.Enabled = true;
            ContactAddNewItem.Enabled = true;
            gbEmails.Enabled = true;
            gbLocation.Enabled = true;
            gbPhone.Enabled = true;
        }

        private void DisableContact()
        {
            ContactMoveFirstItem.Enabled = false;
            ContactMovePreviousItem.Enabled = false;
            ContactMoveNextItem.Enabled = false;
            ContactMoveLastItem.Enabled = false;
            ContactAddNewItem.Enabled = false;
            gbEmails.Enabled = false;
            gbLocation.Enabled = false;
            gbPhone.Enabled = false; 
        }

        private void Contact_MouseDown(object sender, MouseEventArgs e)
        {
            if (ContactPositionItem.Text == "0")
            {
                return;
            }
            ContactSaveItem.Enabled = true;
            DisableContact();
        }

        private void ContactPositionItem_EnabledChanged(object sender, EventArgs e)
        {
            if (ContactPositionItem.Enabled == false)
            {
                gbContact.Enabled = false;
            }
            else
            {
                gbContact.Enabled = true;
            }
        }

        private void btnPhoto_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFD = new OpenFileDialog();
            if (OFD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                
                string image = OFD.FileName;
                Image image2 = Image.FromFile(image);
                pbPhoto.Image = image2;
                OneContact.Photo = Mod.ImageToBase64(image2);
            }
        }

        private void MakePhoto()
        {
            try
            {
                if ((OneContact.Photo != null) && (OneContact.Photo != ""))
                {
                    pbPhoto.Image = Mod.Base64ToImage(OneContact.Photo);
                }
                else
                {
                    pbPhoto.Image = null;
                }

            }
            catch (Exception)
            {

                //throw;
            }
        }
#endregion

        private void ContactPositionItem_TextChanged(object sender, EventArgs e)
        {
            MakePhoto();
        }

        private void searchMenuItem_Click(object sender, EventArgs e)
        {
            int Ind = -1;
            using (frmSearch Search = new frmSearch())
            {
                Search.ContactList = ContactList;
                Search.ShowDialog();               
                Ind = Search.Ind;  
            }
            if (Ind != -1)
            {
                OneContact = new Contact();
                bsContact.Position = Ind;
                OneContact = (Contact)bsContact.Current;
                MakePhoto();
            }
            
        }

        
        
    } // end Form1
}
