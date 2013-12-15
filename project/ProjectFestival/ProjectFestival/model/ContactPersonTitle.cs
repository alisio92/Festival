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
    public class ContactPersonTitle
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

        public static ObservableCollection<ContactPersonTitle> GetContactPersonTitle()
        {
            ApplicationVM.Infotxt("Inladen contact titels", "");
            ObservableCollection<ContactPersonTitle> contactType = new ObservableCollection<ContactPersonTitle>();
            try
            {
                string sql = "SELECT * FROM ContactPersonTitle";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    contactType.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("Contact titels ingeladen", "Inladen contact titels");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database ContactPersonTitle niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan ContactPersonTitle tabel niet inlezen", "");
            }
            return contactType;
        }

        private static ContactPersonTitle Create(IDataRecord record)
        {
            ContactPersonTitle contactPersonTitle = new ContactPersonTitle();

            contactPersonTitle.ID = Convert.ToInt32(record["ID"]);
            contactPersonTitle.Name = record["Name"].ToString();

            return contactPersonTitle;
        }

        public static int AddTitle(ContactPersonTitle contactPersoonTitle)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO ContactpersonTitle VALUES(@ID,@Name)";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoonTitle.Name);
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

        public static int EditTitle(ContactPersonTitle contactPersoonTitle)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE ContactpersonTitle SET Name=@Name WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", contactPersoonTitle.Name);
                DbParameter par2 = Database.AddParameter("@ID", contactPersoonTitle.ID);

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

        public static int DeleteTitle(ContactPersonTitle contactPersoonTitle)
        {
            return DBConnection.DeleteItem("ContactPersonTitle", contactPersoonTitle.ID);
        }
    }
}
