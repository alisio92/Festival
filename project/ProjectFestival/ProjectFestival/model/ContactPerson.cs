using ProjectFestival.database;
using ProjectFestival.viewmodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Xml;
using System.Xml.Serialization;

namespace ProjectFestival.model
{
    public class ContactPerson
    {
        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private String _company;
        public String Company
        {
            get { return _company; }
            set { _company = value; }
        }

        private static ObservableCollection<ContactPersonType> _jobRoleList;
        public static ObservableCollection<ContactPersonType> JobRoleList
        {
            get { return _jobRoleList; }
            set { _jobRoleList = value; }
        }

        private static ObservableCollection<ContactPersonTitle> _jobTitleList;
        public static ObservableCollection<ContactPersonTitle> JobTitleList
        {
            get { return _jobTitleList; }
            set { _jobTitleList = value; }
        }

        private ContactPersonType _jobRole;
        public ContactPersonType JobRole
        {
            get { return _jobRole; }
            set { _jobRole = value; }
        }

        private ContactPersonTitle _jobTitle;
        public ContactPersonTitle JobTitle
        {
            get { return _jobTitle; }
            set { _jobTitle = value; }
        }

        private String _city;
        public String City
        {
            get { return _city; }
            set { _city = value; }
        }

        private String _email;
        public String Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private String _phone;
        public String Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        private String _cellphone;
        public String Cellphone
        {
            get { return _cellphone; }
            set { _cellphone = value; }
        }

        public static ObservableCollection<ContactPerson> contactPersons = new ObservableCollection<ContactPerson>();

        public static int aantal = 1;

        public static ObservableCollection<ContactPerson> GetContactPerson()
        {
            ApplicationVM.Infotxt("Inladen contact personen", "");
            JobRoleList = ContactPersonType.getContactPersonType();
            JobTitleList = ContactPersonTitle.getContactPersonTitle();

            try
            {
                string sql = "SELECT * FROM ContactPerson";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    contactPersons.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("Contact personen ingeladen", "Inladen contact personen");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database ContactPersoon niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            return contactPersons;
        }

        private static ContactPerson Create(IDataRecord record)
        {
            ContactPerson contactPerson = new ContactPerson();

            contactPerson.ID = Convert.ToInt32(record["ID"]);
            contactPerson.Name = record["Name"].ToString();
            contactPerson.JobRole = new ContactPersonType()
            {
                ID = (int)record["JobRole"],
                Name = ContactPerson.JobRoleList[(int)record["JobRole"]-1].Name
            };
            contactPerson.JobTitle = new ContactPersonTitle()
            {
                ID = (int)record["JobTitle"],
                Name = ContactPerson.JobTitleList[(int)record["JobTitle"]-1].Name
            };
            contactPerson.City = record["City"].ToString();
            contactPerson.Email = record["Email"].ToString();
            contactPerson.Cellphone = record["Cellphone"].ToString();
            contactPerson.Company = record["Company"].ToString();
            contactPerson.Phone = record["Phone"].ToString();

            return contactPerson;
        }

        public static int EditContact(ContactPerson contactPersoon)
        {
            ApplicationVM.Infotxt("Data aanpassen in contactPerson op lijn " + contactPersoon.ID, "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Contactperson SET Name=@Name,Company=@Company,JobRole=@JobRole,Jobtitle=@JobTitle,City=@City,Email=@Email,Phone=@Phone,Cellphone=@Cellphone WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoon.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPersoon.ID);
                DbParameter par3 = Database.AddParameter("@Company", contactPersoon.Company);
                DbParameter par4 = Database.AddParameter("@JobRole", contactPersoon.JobRole.ID);
                DbParameter par5 = Database.AddParameter("@JobTitle", contactPersoon.JobTitle.ID);
                DbParameter par6 = Database.AddParameter("@City", contactPersoon.City);
                DbParameter par7 = Database.AddParameter("@Email", contactPersoon.Email);
                DbParameter par8 = Database.AddParameter("@Phone", contactPersoon.Phone);
                DbParameter par9 = Database.AddParameter("@Cellphone", contactPersoon.Cellphone);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);

                trans.Commit();
                ApplicationVM.Infotxt("Data aangepast in contactPerson op lijn " + contactPersoon.ID, "Data aanpassen in contactPerson op lijn " + contactPersoon.ID);
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int AddContact(ContactPerson contactPersoon)
        {
            //ApplicationVM.Infotxt("Niewe lijn toevoegen aan ContactPerson", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Contactperson VALUES(@ID,@Name,@Company,@JobRole,@JobTitle,@City,@Email,@Phone,@Cellphone)";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoon.Name);
                DbParameter par2 = Database.AddParameter("@Company", contactPersoon.Company);
                DbParameter par3 = Database.AddParameter("@JobRole", contactPersoon.JobRole.ID);
                DbParameter par4 = Database.AddParameter("@JobTitle", contactPersoon.JobTitle.ID);
                DbParameter par5 = Database.AddParameter("@City", contactPersoon.City);
                DbParameter par6 = Database.AddParameter("@Email", contactPersoon.Email);
                DbParameter par7 = Database.AddParameter("@Phone", contactPersoon.Phone);
                DbParameter par8 = Database.AddParameter("@Cellphone", contactPersoon.Cellphone);
                DbParameter par9 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);

                trans.Commit();
                //ApplicationVM.Infotxt("Niewe lijn toegevoegd aan ContactPerson", "Niewe lijn toevoegen aan ContactPerson");
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteContact(ContactPerson contactPersoon)
        {
            return DBConnection.DeleteItem("Contactperson", contactPersoon.ID);
        }

        public override string ToString()
        {
            return ID + " " + Name + " " + Company + " " + JobRole + " " + JobTitle + " " + City + " " + Email + " " + Phone + " " + Cellphone;
        }
    }
}
