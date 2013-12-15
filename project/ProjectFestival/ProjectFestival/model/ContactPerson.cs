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
                Name = ContactPerson.JobRoleList[(int)record["JobRole"]-1].Name
            };
            contactPerson.JobTitle = new ContactPersonTitle()
            {
                ID = (int)record["JobTitle"],
                Name = ContactPerson.JobTitleList[(int)record["JobTitle"]-1].Name
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
                ApplicationVM.Infotxt("Kan ContactPerson tabel niet updaten", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int AddContact(ContactPerson contactPersoon)
        {
            ApplicationVM.Infotxt("Niewe lijn toevoegen aan ContactPerson", "");
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
                ApplicationVM.Infotxt("Niewe lijn toegevoegd aan ContactPerson", "Niewe lijn toevoegen aan ContactPerson");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan geen nieuwe lijn toevoegen aan ContactPerson", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteContact(ContactPerson contactPersoon)
        {
            ApplicationVM.Infotxt("Data verwijderen in contactPerson op lijn " + contactPersoon.ID, "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM ContactPerson WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", contactPersoon.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("Data verwijderen in contactPerson op lijn " + contactPersoon.ID, "Data verwijderen in contactPerson op lijn " + contactPersoon.ID);
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan geen rij verwijderen in ContactPerson", "");
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
    }
}
