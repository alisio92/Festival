using ProjectFestival.database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectFestival.model
{
    public class TicketType
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

        private double _price;
        public double Price
        {
            get { return _price; }
            set { _price = value; }
        }

        private int _availableTickets;
        public int AvailableTickets
        {
            get { return _availableTickets; }
            set { _availableTickets = value; }
        }

        public static int aantal = 1;
        
        public static ObservableCollection<TicketType> getTicketTypes()
        {
            ObservableCollection<TicketType> ticketType = new ObservableCollection<TicketType>();
            ticketType = DBConnection.GetDataOutDatabase<TicketType>("TicketType");

            foreach (TicketType t in ticketType)
            {
                aantal++;
            }
            return ticketType;
        }

        public static int EditType(TicketType ticketType)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE TicketType SET Name=@Name,Price=@Price,AvailableTickets=@AvailableTickets WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", ticketType.Name);
                DbParameter par2 = Database.AddParameter("@ID", ticketType.ID);
                DbParameter par3 = Database.AddParameter("@Price", ticketType.Price);
                DbParameter par4 = Database.AddParameter("@AvailableTickets", ticketType.AvailableTickets);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2,par3,par4);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }
        
        public static int AddType(TicketType ticketType)
        {
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO TicketType VALUES(@ID,@Name,@price,@AvailableTickets)";
                DbParameter par1 = Database.AddParameter("@Name", ticketType.Name);
                DbParameter par2 = Database.AddParameter("@ID", aantal);
                DbParameter par3 = Database.AddParameter("@Price", ticketType.Price);
                DbParameter par4 = Database.AddParameter("@AvailableTickets", ticketType.AvailableTickets);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2,par3,par4);

                trans.Commit();
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteType(TicketType ticketType)
        {
            return DBConnection.DeleteItem("TicketType", ticketType.ID);
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}
