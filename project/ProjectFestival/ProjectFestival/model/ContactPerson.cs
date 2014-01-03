using ProjectFestival.database;
using ProjectFestival.viewmodel;
using ProjectFestival.writetofile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        public static int aantal = 1;
        public static ObservableCollection<ContactPerson> contactPersons = new ObservableCollection<ContactPerson>();
        public static ObservableCollection<ContactPerson> oContactPersons = new ObservableCollection<ContactPerson>();

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private int _IDDatabase;
        public int IDDatabase
        {
            get { return _IDDatabase; }
            set { _IDDatabase = value; }
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

        public static ObservableCollection<ContactPerson> GetContactPerson()
        {
            aantal = 1;
            JobRoleList = ContactPersonType.GetContactPersonType();
            JobTitleList = ContactPersonTitle.GetContactPersonTitle();

            try
            {
                string sql = "SELECT * FROM ContactPerson";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    contactPersons.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            oContactPersons = contactPersons;
            return contactPersons;
        }

        public static ContactPerson Create(IDataRecord record)
        {
            ContactPerson contactPerson = new ContactPerson();

            contactPerson.ID = aantal;
            contactPerson.IDDatabase = Convert.ToInt32(record["ID"]);
            contactPerson.Name = record["Name"].ToString();
            contactPerson.Company = record["Company"].ToString();

            foreach (ContactPersonType type in JobRoleList)
            {
                if (type.IDDatabase == (int)record["JobRole"])
                {
                    contactPerson.JobRole = new ContactPersonType()
                    {
                        IDDatabase = type.IDDatabase,
                        ID = type.ID,
                        Name = type.Name
                    };
                }
            }

            foreach (ContactPersonTitle title in JobTitleList)
            {
                if (title.IDDatabase == (int)record["JobTitle"])
                {
                    contactPerson.JobTitle = new ContactPersonTitle()
                    {
                        IDDatabase = title.IDDatabase,
                        ID = title.ID,
                        Name = title.Name
                    };
                }
            }

            contactPerson.City = record["City"].ToString();
            contactPerson.Email = record["Email"].ToString();
            contactPerson.Cellphone = record["Cellphone"].ToString();
            contactPerson.Phone = record["Phone"].ToString();

            return contactPerson;
        }

        public static int EditContact(ContactPerson contactPerson)
        {
            DbTransaction trans = null;

            if (contactPerson.Phone == null || contactPerson.Phone == "")
            {
                contactPerson.Phone = "N/A";
            }
            if (contactPerson.Cellphone == null || contactPerson.Cellphone == "")
            {
                contactPerson.Cellphone = "N/A";
            }

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Contactperson SET Name=@Name,Company=@Company,JobRole=@JobRole,Jobtitle=@JobTitle,City=@City,Email=@Email,Phone=@Phone,Cellphone=@Cellphone WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPerson.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPerson.IDDatabase);
                DbParameter par3 = Database.AddParameter("@Company", contactPerson.Company);
                DbParameter par4 = Database.AddParameter("@JobRole", JobRoleList[contactPerson.JobRole.ID-1].IDDatabase);
                DbParameter par5 = Database.AddParameter("@JobTitle", JobTitleList[contactPerson.JobTitle.ID-1].IDDatabase);
                DbParameter par6 = Database.AddParameter("@City", contactPerson.City);
                DbParameter par7 = Database.AddParameter("@Email", contactPerson.Email);
                DbParameter par8 = Database.AddParameter("@Phone", contactPerson.Phone);
                DbParameter par9 = Database.AddParameter("@Cellphone", contactPerson.Cellphone);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
                trans.Rollback();
                return 0;
            }
        }

        public static int AddContact(ContactPerson contactPerson)
        {
            DbTransaction trans = null;

            if (contactPerson.Phone == null || contactPerson.Phone=="")
            {
                contactPerson.Phone = "N/A";
            }
            if (contactPerson.Cellphone == null || contactPerson.Cellphone == "")
            {
                contactPerson.Cellphone = "N/A";
            }

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Contactperson VALUES(@Name,@Company,@JobRole,@JobTitle,@City,@Email,@Phone,@Cellphone)";
                DbParameter par1 = Database.AddParameter("@Name", contactPerson.Name);
                DbParameter par2 = Database.AddParameter("@Company", contactPerson.Company);
                DbParameter par3 = Database.AddParameter("@JobRole", JobRoleList[contactPerson.JobRole.ID-1].IDDatabase);
                DbParameter par4 = Database.AddParameter("@JobTitle", JobTitleList[contactPerson.JobTitle.ID-1].IDDatabase);
                DbParameter par5 = Database.AddParameter("@City", contactPerson.City);
                DbParameter par6 = Database.AddParameter("@Email", contactPerson.Email);
                DbParameter par7 = Database.AddParameter("@Phone", contactPerson.Phone);
                DbParameter par8 = Database.AddParameter("@Cellphone", contactPerson.Cellphone);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteContact(ContactPerson contactPerson)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM ContactPerson WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", contactPerson.IDDatabase);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
                trans.Rollback();
                return 0;
            }
        }

        public static void Zoeken(string parameter)
        {
            parameter = parameter.ToLower();
            contactPersons = new ObservableCollection<ContactPerson>();

            foreach (ContactPerson contactPerson in oContactPersons)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((contactPerson.Name.ToLower().Contains(parameter)) || (contactPerson.JobRole.Name.ToLower().Contains(parameter)) || (contactPerson.JobTitle.Name.ToLower().Contains(parameter)) || (contactPerson.Company.ToLower().Contains(parameter)) || (contactPerson.City.ToLower().Contains(parameter)) || (contactPerson.Email.ToLower().Contains(parameter)) || (contactPerson.Phone.ToLower().Contains(parameter)) || (contactPerson.Cellphone.ToLower().Contains(parameter)) || (contactPerson.ID.ToString().ToLower().Contains(parameter)))
                    {
                        contactPersons.Add(contactPerson);
                    }
                }
                else
                {
                    contactPersons.Add(contactPerson);
                }
            }
        }

        public override string ToString()
        {
            return ID + " " + Name + " " + Company + " " + JobRole + " " + JobTitle + " " + City + " " + Email + " " + Phone + " " + Cellphone;
        }
    }
}
