using ProjectFestival.database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
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

        public static ObservableCollection<ContactPersonTitle> getContactPersonTitle()
        {
            ObservableCollection<ContactPersonTitle> contactTitle = new ObservableCollection<ContactPersonTitle>();
            contactTitle = DBConnection.GetDataOutDatabase<ContactPersonTitle>("ContactPersonTitle");

            foreach (ContactPersonTitle cp in contactTitle)
            {
                aantal++;
            }
            return contactTitle;
        }

        public static int EditTitle(ContactPersonTitle contactPersoonTitle)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE ContactpersoonTitle SET Name=@Name WHERE ID=@ID";
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

        public static int AddTitle(ContactPersonTitle contactPersoonTitle)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO ContactpersoonTitle VALUES(@ID,@Name)";
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

        public static int DeleteTitle(ContactPersonTitle contactPersoonTitle)
        {
            return DBConnection.DeleteItem("ContactPersonTitle", contactPersoonTitle.ID);
        }
    }
}
