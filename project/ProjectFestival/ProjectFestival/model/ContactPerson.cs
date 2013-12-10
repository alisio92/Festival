using ProjectFestival.database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
            JobRoleList = ContactPersonType.getContactPersonType();
            JobTitleList = ContactPersonTitle.getContactPersonTitle();
            contactPersons = DBConnection.GetDataOutDatabase<ContactPerson>("Contactpersoon");

            foreach(ContactPerson cp in contactPersons)
            {
                aantal++;
            }

            return contactPersons;
        }

        public static int EditContact(ContactPerson contactPersoon)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Contactpersoon SET Name=@Name,Company=@Company,JobRole=@JobRole,Jobtitle=@JobTitle,City=@City,Email=@Email,Phone=@Phone,Cellphone=@Cellphone WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoon.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPersoon.ID);
                DbParameter par3 = Database.AddParameter("@Company", contactPersoon.Company);
                DbParameter par4 = Database.AddParameter("@JobRole", contactPersoon.JobRole.Name);
                DbParameter par5 = Database.AddParameter("@JobTitle", contactPersoon.JobTitle.Name);
                DbParameter par6 = Database.AddParameter("@City", contactPersoon.City);
                DbParameter par7 = Database.AddParameter("@Email", contactPersoon.Email);
                DbParameter par8 = Database.AddParameter("@Phone", contactPersoon.Phone);
                DbParameter par9 = Database.AddParameter("@Cellphone", contactPersoon.Cellphone);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8,par9);

                trans.Commit();
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
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Contactpersoon VALUES(@ID,@Name,@Company,@JobRole,@JobTitle,@City,@Email,@Phone,@Cellphone)";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoon.Name);
                DbParameter par2 = Database.AddParameter("@Company", contactPersoon.Company);
                DbParameter par3 = Database.AddParameter("@JobRole", contactPersoon.JobRole.Name);
                DbParameter par4 = Database.AddParameter("@JobTitle", contactPersoon.JobTitle.Name);
                DbParameter par5 = Database.AddParameter("@City", contactPersoon.City);
                DbParameter par6 = Database.AddParameter("@Email", contactPersoon.Email);
                DbParameter par7 = Database.AddParameter("@Phone", contactPersoon.Phone);
                DbParameter par8 = Database.AddParameter("@Cellphone", contactPersoon.Cellphone);
                DbParameter par9 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8,par9);

                trans.Commit();
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
            return DBConnection.DeleteItem("Contactpersoon",contactPersoon.ID);
        }

        public override string ToString()
        {
            return ID + " " + Name + " " + Company + " " + JobRole + " " + JobTitle + " " + City + " " + Email + " " + Phone + " " + Cellphone;
        }
    }
}
