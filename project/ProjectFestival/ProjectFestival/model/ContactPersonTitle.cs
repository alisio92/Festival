using ProjectFestival.database;
using ProjectFestival.viewmodel;
using ProjectFestival.writetofile;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.model
{
    public class ContactPersonTitle
    {
        public static int aantal = 1;
        public static ObservableCollection<ContactPersonTitle> contactTitles = new ObservableCollection<ContactPersonTitle>();
        public static ObservableCollection<ContactPersonTitle> oContactTitles = new ObservableCollection<ContactPersonTitle>();
        
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
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

        public static ObservableCollection<ContactPersonTitle> GetContactPersonTitle()
        {
            aantal = 1;
            try
            {
                string sql = "SELECT * FROM ContactPersonTitle";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    contactTitles.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            oContactTitles = contactTitles;
            return contactTitles;
        }

        public static ContactPersonTitle Create(IDataRecord record)
        {
            ContactPersonTitle contactPersonTitle = new ContactPersonTitle();

            contactPersonTitle.ID = aantal;
            contactPersonTitle.IDDatabase = Convert.ToInt32(record["ID"]);
            contactPersonTitle.Name = record["Name"].ToString();

            return contactPersonTitle;
        }

        public static int AddTitle(ContactPersonTitle contactPersonTitle)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO ContactpersonTitle VALUES(@Name)";
                DbParameter par1 = Database.AddParameter("@Name", contactPersonTitle.Name);

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

        public static int EditTitle(ContactPersonTitle contactPersonTitle)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE ContactpersonTitle SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPersonTitle.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPersonTitle.IDDatabase);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

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

        public static int DeleteTitle(ContactPersonTitle contactPersonTitle)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM ContactPersonTitle WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", contactPersonTitle.IDDatabase);

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
            contactTitles = new ObservableCollection<ContactPersonTitle>();

            foreach (ContactPersonTitle contactPersonTitle in oContactTitles)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((contactPersonTitle.Name.ToLower().Contains(parameter)) || (contactPersonTitle.ID.ToString().ToLower().Contains(parameter)))
                    {
                        contactTitles.Add(contactPersonTitle);
                    }
                }
                else
                {
                    contactTitles.Add(contactPersonTitle);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
