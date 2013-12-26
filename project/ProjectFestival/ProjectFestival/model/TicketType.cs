using ProjectFestival.database;
using ProjectFestival.viewmodel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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

        public static ObservableCollection<TicketType> ticketType = new ObservableCollection<TicketType>();
        public static ObservableCollection<TicketType> oTicketType = new ObservableCollection<TicketType>();

        public static ObservableCollection<TicketType> GetTicketTypes()
        {
            aantal = 1;
            ticketType = new ObservableCollection<TicketType>();
            ApplicationVM.Infotxt("Inladen ticket types", "");
            try
            {
                string sql = "SELECT * FROM TicketType";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    ticketType.Add(Create(reader));
                    aantal++;
                }
                ApplicationVM.Infotxt("Ticket types ingeladen", "Inladen ticket types");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database TicketType niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan TicketType tabel niet inlezen", "");
            }
            oTicketType = ticketType;
            return ticketType;
        }

        private static TicketType Create(IDataRecord record)
        {
            TicketType ticketType = new TicketType();

            ticketType.ID = aantal;
            ticketType.IDDatabase = Convert.ToInt32(record["ID"]);
            ticketType.Name = record["Name"].ToString();
            ticketType.Price = Convert.ToDouble(record["Price"].ToString());
            ticketType.AvailableTickets = Convert.ToInt32(record["AvailableTickets"]);

            return ticketType;
        }

        public static int EditType(TicketType ticketType)
        {
            ApplicationVM.Infotxt("TicketType aanpassen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE TicketType SET Name=@Name,Price=@Price,AvailableTickets=@AvailableTickets WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@Name", ticketType.Name);
                DbParameter par2 = Database.AddParameter("@ID", ticketType.IDDatabase);
                DbParameter par3 = Database.AddParameter("@Price", ticketType.Price);
                DbParameter par4 = Database.AddParameter("@AvailableTickets", ticketType.AvailableTickets);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2,par3,par4);

                trans.Commit();
                ApplicationVM.Infotxt("TicketType aangepast", "TicketType aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan TicketType niet aanpassen", "");
                trans.Rollback();
                return 0;
            }
        }
        
        public static int AddType(TicketType ticketType)
        {
            ApplicationVM.Infotxt("TicketType toevoegen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "INSERT INTO TicketType VALUES(@Name,@price,@AvailableTickets)";
                DbParameter par1 = Database.AddParameter("@Name", ticketType.Name);
                DbParameter par2 = Database.AddParameter("@Price", ticketType.Price);
                DbParameter par3 = Database.AddParameter("@AvailableTickets", ticketType.AvailableTickets);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2,par3);

                trans.Commit();
                ApplicationVM.Infotxt("TicketType toegevoegd", "TicketType aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan TicketType niet toevoegen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteType(TicketType ticketType)
        {
            ApplicationVM.Infotxt("TicketType wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM TicketType WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", ticketType.IDDatabase);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("TicketType gewist", "TicketType wissen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan TicketType niet wissen", "");
                trans.Rollback();
                return 0;
            }
        }
        
        public override string ToString()
        {
            return Name;
        }

        public static void Zoeken(string parameter)
        {
            parameter = parameter.ToLower();
            ticketType = new ObservableCollection<TicketType>();

            foreach (TicketType c in oTicketType)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((c.Name.ToLower().Contains(parameter)) || (c.ID.ToString().ToLower().Contains(parameter)) || (c.Price.ToString().ToLower().Contains(parameter)) || (c.AvailableTickets.ToString().ToLower().Contains(parameter)))
                    {
                        ticketType.Add(c);
                    }
                }
                else
                {
                    ticketType.Add(c);
                }
            }
        }
    }
}
