using ProjectFestival.database;
using ProjectFestival.viewmodel;
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
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private String _name;
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public static int aantal = 1;

        public override string ToString()
        {
            return Name;
        }

        public static ObservableCollection<ContactPersonType> GetContactPersonType()
        {
            ApplicationVM.Infotxt("Inladen contact types", "");
            ObservableCollection<ContactPersonType> contactType = new ObservableCollection<ContactPersonType>();
            try
            {
                string sql = "SELECT * FROM ContactPersonType";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    contactType.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("Contact types ingeladen", "Inladen contact types");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database ContactPersonType niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan ContactPersonType tabel niet inlezen", "");
            }
            return contactType;
        }

        private static ContactPersonType Create(IDataRecord record)
        {
            ContactPersonType contactPersonType = new ContactPersonType();

            contactPersonType.ID = Convert.ToInt32(record["ID"]);
            contactPersonType.Name = record["Name"].ToString();

            return contactPersonType;
        }

        public static int AddType(ContactPersonType contactPersoonType)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO ContactpersonType VALUES(@ID,@Name)";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoonType.Name);
                DbParameter par2 = Database.AddParameter("@ID", aantal);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int EditType(ContactPersonType contactPersoonType)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE ContactpersonType SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoonType.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPersoonType.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }
        
        public static int DeleteType(ContactPersonType contactPersoonType)
        {
            return DBConnection.DeleteItem("ContactPersonType", contactPersoonType.ID);
        }
    }
}
