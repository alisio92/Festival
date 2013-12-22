using ProjectFestival.database;
using ProjectFestival.viewmodel;
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
    public class ContactPerson : IDataErrorInfo
    {
        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        [Required(ErrorMessage = "Naam mag niet leeg zijn")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "De lengte moet tussen 2 en 50 karakters liggen")]
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

        [Required(ErrorMessage = "Type mag niet leeg zijn")]
        private ContactPersonType _jobRole;
        public ContactPersonType JobRole
        {
            get { return _jobRole; }
            set { _jobRole = value; }
        }

        [Required(ErrorMessage = "Titel mag niet leeg zijn")]
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

        [Phone(ErrorMessage = "Geef een correcte telefoon nummer in")]
        private String _phone;
        public String Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        [Phone(ErrorMessage = "Geef een correcte telefoon nummer in")]
        private String _cellphone;
        public String Cellphone
        {
            get { return _cellphone; }
            set { _cellphone = value; }
        }

        public static ObservableCollection<ContactPerson> contactPersons = new ObservableCollection<ContactPerson>();
        public static ObservableCollection<ContactPerson> oContactPersons = new ObservableCollection<ContactPerson>();

        public static int aantal = 1;

        public static ObservableCollection<ContactPerson> GetContactPerson()
        {
            ApplicationVM.Infotxt("Inladen contact personen", "");
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
                ApplicationVM.Infotxt("Contact personen ingeladen", "Inladen contact personen");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database ContactPerson niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan ContactPerson tabel niet inlezen", "");
            }
            oContactPersons = contactPersons;
            return contactPersons;
        }

        private static ContactPerson Create(IDataRecord record)
        {
            ContactPerson contactPerson = new ContactPerson();

            contactPerson.ID = Convert.ToInt32(record["ID"]);
            contactPerson.Name = record["Name"].ToString();
            contactPerson.Company = record["Company"].ToString();
            contactPerson.JobRole = new ContactPersonType()
            {
                ID = (int)record["JobRole"],
                Name = JobRoleList[(int)record["JobRole"]-1].Name
            };
            contactPerson.JobTitle = new ContactPersonTitle()
            {
                ID = (int)record["JobTitle"],
                Name = JobTitleList[(int)record["JobTitle"]-1].Name
            };
            contactPerson.City = record["City"].ToString();
            contactPerson.Email = record["Email"].ToString();
            if (record["Cellphone"].ToString() != "")
            {
                contactPerson.Cellphone = record["Cellphone"].ToString();
            }
            else
            {
                contactPerson.Cellphone = "N/A";
            }
            if (record["Phone"].ToString() != "")
            {
                contactPerson.Phone = record["Phone"].ToString();
            }
            else
            {
                contactPerson.Phone = "N/A";
            }

            return contactPerson;
        }

        public static int EditContact(ContactPerson contactPerson)
        {
            ApplicationVM.Infotxt("ContactPerson aanpassen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Contactperson SET Name=@Name,Company=@Company,JobRole=@JobRole,Jobtitle=@JobTitle,City=@City,Email=@Email,Phone=@Phone,Cellphone=@Cellphone WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPerson.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPerson.ID);
                DbParameter par3 = Database.AddParameter("@Company", contactPerson.Company);
                DbParameter par4 = Database.AddParameter("@JobRole", contactPerson.JobRole.ID);
                DbParameter par5 = Database.AddParameter("@JobTitle", contactPerson.JobTitle.ID);
                DbParameter par6 = Database.AddParameter("@City", contactPerson.City);
                DbParameter par7 = Database.AddParameter("@Email", contactPerson.Email);
                DbParameter par8 = Database.AddParameter("@Phone", contactPerson.Phone);
                DbParameter par9 = Database.AddParameter("@Cellphone", contactPerson.Cellphone);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);

                trans.Commit();
                ApplicationVM.Infotxt("ContactPerson aangepast", "ContactPerson aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan ContactPerson niet aanpassen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int AddContact(ContactPerson contactPerson)
        {
            ApplicationVM.Infotxt("ContactPerson toevoegen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO Contactperson VALUES(@ID,@Name,@Company,@JobRole,@JobTitle,@City,@Email,@Phone,@Cellphone)";
                DbParameter par1 = Database.AddParameter("@Name", contactPerson.Name);
                DbParameter par2 = Database.AddParameter("@Company", contactPerson.Company);
                DbParameter par3 = Database.AddParameter("@JobRole", contactPerson.JobRole.ID);
                DbParameter par4 = Database.AddParameter("@JobTitle", contactPerson.JobTitle.ID);
                DbParameter par5 = Database.AddParameter("@City", contactPerson.City);
                DbParameter par6 = Database.AddParameter("@Email", contactPerson.Email);
                DbParameter par7 = Database.AddParameter("@Phone", contactPerson.Phone);
                DbParameter par8 = Database.AddParameter("@Cellphone", contactPerson.Cellphone);
                DbParameter par9 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5, par6, par7, par8, par9);

                trans.Commit();
                ApplicationVM.Infotxt("ContactPerson toegevoegd", "ContactPerson aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan ContactPerson niet toevoegen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteContact(ContactPerson contactPerson)
        {
            ApplicationVM.Infotxt("ContactPerson wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM ContactPerson WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", contactPerson.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("ContactPerson gewist", "ContactPerson wissen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan ContactPerson niet wissen", "");
                trans.Rollback();
                return 0;
            }
        }

        public override string ToString()
        {
            return ID + " " + Name + " " + Company + " " + JobRole + " " + JobTitle + " " + City + " " + Email + " " + Phone + " " + Cellphone;
        }

        public string Error
        {
            get { return "Het object is niet valid"; }
        }
        public string this[string columnName]
        {
            get
            {
                try
                {
                    object value = this.GetType().GetProperty(columnName).GetValue(this);
                    Validator.ValidateProperty(value, new ValidationContext(this, null, null)
                    {
                        MemberName = columnName
                    });
                }
                catch (ValidationException ex)
                {
                    return ex.Message;
                }
                return String.Empty;
            }
        }

        public static void Zoeken(string parameter)
        {
            contactPersons = new ObservableCollection<ContactPerson>();
            foreach (ContactPerson c in oContactPersons)
            {
                if (c.Name.Contains(parameter))
                {
                    contactPersons.Add(c);
                }
            }
        }
    }
}
