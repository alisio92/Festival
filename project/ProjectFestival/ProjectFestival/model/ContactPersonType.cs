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
    public class ContactPersonType
    {
        public static int aantal = 1;
        public static ObservableCollection<ContactPersonType> contactType = new ObservableCollection<ContactPersonType>();
        public static ObservableCollection<ContactPersonType> oContactType = new ObservableCollection<ContactPersonType>();

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

        public static ObservableCollection<ContactPersonType> GetContactPersonType()
        {
            aantal = 1;
            try
            {
                string sql = "SELECT * FROM ContactPersonType";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    contactType.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            oContactType = contactType;
            return contactType;
        }

        public static ContactPersonType Create(IDataRecord record)
        {
            ContactPersonType contactPersonType = new ContactPersonType();

            contactPersonType.ID = aantal;
            contactPersonType.IDDatabase = Convert.ToInt32(record["ID"]);
            contactPersonType.Name = record["Name"].ToString();

            return contactPersonType;
        }

        public static int AddType(ContactPersonType contactPersonType)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO ContactpersonType VALUES(@Name)";
                DbParameter par1 = Database.AddParameter("@Name", contactPersonType.Name);

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

        public static int EditType(ContactPersonType contactPersonType)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE ContactpersonType SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPersonType.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPersonType.IDDatabase);

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
        
        public static int DeleteType(ContactPersonType contactPersonType)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM ContactPersonType WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", contactPersonType.IDDatabase);

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
            contactType = new ObservableCollection<ContactPersonType>();

            foreach (ContactPersonType c in oContactType)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((c.Name.ToLower().Contains(parameter)) || (c.ID.ToString().ToLower().Contains(parameter)))
                    {
                        contactType.Add(c);
                    }
                }
                else
                {
                    contactType.Add(c);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
