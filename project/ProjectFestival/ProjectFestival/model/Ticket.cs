using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ProjectFestival.database;
using ProjectFestival.viewmodel;
using ProjectFestival.writetofile;
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
using System.Windows;
using System.Windows.Forms;
using System.Xml;

namespace ProjectFestival.model
{
    public class Ticket
    {
        public static int aantal = 1;
        public static ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();
        public static ObservableCollection<Ticket> oTickets = new ObservableCollection<Ticket>();

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

        private String _ticketHolder;
        public String TicketHolder
        {
            get { return _ticketHolder; }
            set { _ticketHolder = value; }
        }

        private String _ticketHolderEmail;
        public String TicketHolderEmail
        {
            get { return _ticketHolderEmail; }
            set { _ticketHolderEmail = value; }
        }

        private TicketType _ticketType;
        public TicketType TicketType
        {
            get { return _ticketType; }
            set { _ticketType = value; }
        }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set { _amount = value; }
        }

        private static ObservableCollection<TicketType> _ticketTypeList;
        public static ObservableCollection<TicketType> TicketTypeList
        {
            get { return _ticketTypeList; }
            set { _ticketTypeList = value; }
        }

        public static ObservableCollection<Ticket> GetTickets()
        {
            aantal = 1;
            tickets = new ObservableCollection<Ticket>();
            TicketTypeList = TicketType.GetTicketTypes();

            try
            {
                string sql = "SELECT * FROM Ticket";
                DbDataReader reader = Database.GetData(sql);

                while (reader.Read())
                {
                    tickets.Add(Create(reader));
                    aantal++;
                }
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
            }
            oTickets = tickets;
            return tickets;
        }

        private static Ticket Create(IDataRecord record)
        {
            Ticket ticket = new Ticket();

            ticket.ID = aantal;
            ticket.IDDatabase = Convert.ToInt32(record["ID"]);
            ticket.TicketHolder = record["TicketHolder"].ToString();
            ticket.TicketHolderEmail = record["TicketHolderEmail"].ToString();
            ticket.Amount = (int)record["Amount"];
            foreach (TicketType type in TicketTypeList)
            {
                if (type.IDDatabase == (int)record["TicketType"])
                {
                    ticket.TicketType = new TicketType()
                    {
                        IDDatabase = type.IDDatabase,
                        ID = type.ID,
                        Name = type.Name,
                        AvailableTickets = type.AvailableTickets,
                        Price = type.Price
                    };
                }
            }

            return ticket;
        }

        public static int EditTicket(Ticket ticket)
        {
            DbTransaction trans = null;

            ObservableCollection<TicketType> ticketType = TicketType.GetTicketTypes();
            ObservableCollection<Ticket> ticketVorig = Ticket.GetTickets();
            int index = 0;
            int index2 = 0;
            for (int i = 0; i < ticketType.Count(); i++)
            {
                if (ticketType[i].Name == ticket.TicketType.Name)
                {
                    index = i;
                }
            }
            for (int i2 = 0; i2 < tickets.Count(); i2++)
            {
                if (tickets[i2].TicketHolder == ticket.TicketHolder)
                {
                    index2 = i2;
                }
            }
            int aantaltickets = ticketType[index].AvailableTickets;
            int aantalNu = ticket.Amount;
            int aantalVorig = ticketVorig[index2].Amount;

            if (ticket.TicketType.Name == ticketVorig[index2].TicketType.Name)
            {
                if (aantalNu > aantalVorig)
                {
                    if (aantaltickets - (aantalNu - aantalVorig) >= 0)
                    {
                        return EditTicket(trans, ticket, aantaltickets, index2);
                    }
                    else
                    {
                        ApplicationVM.MessageTickets();
                        return 0;
                    }
                }
                else if (aantalNu < aantalVorig)
                {
                    if (aantalNu >= 0)
                    {
                        return EditTicket(trans, ticket, aantaltickets, index2);
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return DatabaseConnectieTicket(trans, ticket, 0);
                }
            }
            else
            {
                int idVorig = ticketVorig[index2].TicketType.ID-1;
                if (aantaltickets >= aantalNu)
                {
                    int rowsaffected = 0;
                    rowsaffected += EditTicketType(ticket.TicketType.IDDatabase, -aantalVorig, ticketType[idVorig].AvailableTickets, idVorig);
                    rowsaffected += EditTicketType(TicketTypeList[ticket.TicketType.ID - 1].IDDatabase, aantalNu, aantaltickets, index);
                    rowsaffected += DatabaseConnectieTicket(trans, ticket, rowsaffected);
                }
            }
            return 0;
        }

        public static int DatabaseConnectieTicket(DbTransaction trans, Ticket ticket, int rowsaffected)
        {
            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Ticket SET TicketHolder=@TicketHolder,TicketHolderEmail=@TicketHolderEmail,TicketType=@TicketType,Amount=@Amount WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@TicketHolder", ticket.TicketHolder);
                DbParameter par2 = Database.AddParameter("@ID", ticket.IDDatabase);
                DbParameter par3 = Database.AddParameter("@TicketHolderEmail", ticket.TicketHolderEmail);
                DbParameter par4 = Database.AddParameter("@TicketType", TicketTypeList[ticket.TicketType.ID - 1].IDDatabase);
                DbParameter par5 = Database.AddParameter("@Amount", ticket.Amount);

                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5);

                trans.Commit();
                tickets = GetTickets();
                FileWriter.PrintTicket(ticket);
                return rowsaffected;
            }
            catch (Exception)
            {
                trans.Rollback();
                return 0;
            }
        }

        public static int EditTicket(DbTransaction trans, Ticket ticket, int aantaltickets, int index)
        {
            ObservableCollection<TicketType> types = new ObservableCollection<TicketType>();
            types = TicketType.GetTicketTypes();
            int vorig = tickets[index].Amount;
            int rowsaffected = 0;

            rowsaffected += EditTicketType(ticket.TicketType.IDDatabase, ticket.Amount - vorig, aantaltickets, ticket.TicketType.ID);
            rowsaffected += DatabaseConnectieTicket(trans, ticket, rowsaffected);
            return rowsaffected;
        }

        public static int AddTicket(Ticket ticket)
        {
            DbTransaction trans = null;
            int index = 0;
            for (int i = 0; i < TicketTypeList.Count(); i++)
            {
                if (ticket.TicketType.Name == TicketTypeList[i].Name)
                {
                    index = i;
                }
            }

            int aantaltickets = TicketType.ticketTypes[index].AvailableTickets;
            ObservableCollection<TicketType> types = TicketType.GetTicketTypes();
            int vorig = types[index].AvailableTickets;

            if (vorig - ticket.Amount >= 0)
            {
                EditTicketType(TicketTypeList[index].IDDatabase, ticket.Amount, vorig, index);

                try
                {
                    trans = Database.BeginTransaction();

                    string sql = "INSERT INTO Ticket VALUES(@TicketHolder,@TicketHolderEmail,@TicketType,@Amount)";
                    DbParameter par1 = Database.AddParameter("@TicketHolder", ticket.TicketHolder);
                    DbParameter par2 = Database.AddParameter("@TicketHolderEmail", ticket.TicketHolderEmail);
                    DbParameter par3 = Database.AddParameter("@TicketType", TicketTypeList[index].IDDatabase);
                    DbParameter par4 = Database.AddParameter("@Amount", ticket.Amount);

                    int rowsaffected = 0;
                    rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4);

                    trans.Commit();
                    tickets = GetTickets();
                    FileWriter.PrintTicket(ticket);
                    return rowsaffected;
                }
                catch (Exception e)
                {
                    FileWriter.WriteToFile(e.Message);
                    trans.Rollback();
                    return 0;
                }
            }
            else
            {
                ApplicationVM.MessageTickets();
                return 0;
            }
        }

        public static int EditTicketType(int idDatabase, int tickets, int vorig, int id)
        {
            DbTransaction trans = null;

            TicketType.ticketTypes[id].AvailableTickets = vorig - tickets;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE TicketType SET AvailableTickets=@AvailableTickets WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@ID", idDatabase);
                DbParameter par2 = Database.AddParameter("@AvailableTickets", vorig - tickets);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

                trans.Commit();
                TicketTypeList = TicketType.GetTicketTypes();
                return rowsaffected;
            }
            catch (Exception e)
            {
                FileWriter.WriteToFile(e.Message);
                trans.Rollback();
                return 0;
            }
        }

        public static int DeleteTicket(Ticket ticket)
        {
            DbTransaction trans = null;

            ObservableCollection<TicketType> types = new ObservableCollection<TicketType>();
            types = TicketType.GetTicketTypes();

            int index = 0;
            for (int i = 0; i < types.Count(); i++)
            {
                if (types[i].Name == ticket.TicketType.Name)
                {
                    index = i;
                }
            }
            int vorig = TicketTypeList[index].AvailableTickets;

            EditTicketType(ticket.TicketType.IDDatabase, -ticket.Amount, vorig, index);

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Ticket WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", ticket.IDDatabase);

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
            tickets = new ObservableCollection<Ticket>();

            foreach (Ticket ticket in oTickets)
            {
                if (parameter != "" && parameter != "Zoeken")
                {
                    if ((ticket.TicketHolder.ToLower().Contains(parameter)) || (ticket.TicketHolderEmail.ToLower().Contains(parameter)) || (ticket.TicketType.Name.ToLower().Contains(parameter)) || (ticket.Amount.ToString().ToLower().Contains(parameter)) || (ticket.ID.ToString().ToLower().Contains(parameter)))
                    {
                        tickets.Add(ticket);
                    }
                }
                else
                {
                    tickets.Add(ticket);
                }
            }
        }

        public override string ToString()
        {
            return ID + " " + TicketHolder + " " + TicketType + " " + TicketHolderEmail + " " + Amount;
        }
    }
}
