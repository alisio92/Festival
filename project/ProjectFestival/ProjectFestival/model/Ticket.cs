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
    public class Ticket
    {
        private int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value; }
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

        public static ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();

        public static int aantal = 1;

        public static ObservableCollection<Ticket> GetTickets()
        {
            tickets = new ObservableCollection<Ticket>();
            ApplicationVM.Infotxt("Inladen klanten", "");
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
                ApplicationVM.Infotxt("Klanten ingeladen", "Inladen klanten");
            }
            catch (SqlException)
            {
                ApplicationVM.Infotxt("Kan database Ticket niet vinden", "");
            }
            catch (IndexOutOfRangeException)
            {
                ApplicationVM.Infotxt("Kolommen database hebben niet de juiste naam", "");
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Ticket tabel niet inlezen", "");
            }
            return tickets;
        }

        private static Ticket Create(IDataRecord record)
        {
            Ticket ticket = new Ticket();

            ticket.ID = Convert.ToInt32(record["ID"]);
            ticket.TicketHolder = record["TicketHolder"].ToString();
            ticket.TicketHolderEmail = record["TicketHolderEmail"].ToString();
            ticket.Amount = (int)record["Amount"];
            ticket.TicketType = new TicketType()
            {
                ID = (int)record["TicketType"],
                Name = TicketTypeList[(int)record["TicketType"] - 1].Name
            };

            return ticket;
        }

        public static int EditTicket(Ticket ticket)
        {
            ApplicationVM.Infotxt("Ticket aanpassen", "");
            DbTransaction trans = null;

            ObservableCollection<TicketType> ticketType = TicketType.GetTicketTypes();
            ObservableCollection<Ticket> ticketVorig = Ticket.GetTickets();
            int aantaltickets = ticketType[ticket.TicketType.ID - 1].AvailableTickets;
            int aantalNu = ticket.Amount;
            int aantalVorig = ticketVorig[ticket.ID - 1].Amount;

            if (aantalNu > aantalVorig)
            {
                if (aantaltickets - (aantalNu - aantalVorig) >= 0)
                {
                    return EditTicket(trans, ticket, aantaltickets);
                }
                else
                {
                    ApplicationVM.Infotxt("Er zijn niet genoeg tickets beschikbaar", "");
                    return 0;
                }
            }
            else if (aantalNu < aantalVorig)
            {
                if (aantalNu >= 0)
                {
                    return EditTicket(trans, ticket, aantaltickets);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                int idVorig = ticketVorig[ticket.ID - 1].TicketType.ID;
                if (idVorig != ticket.TicketType.ID)
                {
                    if (aantaltickets >= aantalNu)
                    {
                        int rowsaffected = 0;
                        rowsaffected += EditTicketType(idVorig, -aantalVorig, ticketType[idVorig - 1].AvailableTickets);
                        rowsaffected += EditTicketType(ticket.TicketType.ID, aantalVorig, aantaltickets);
                        try
                        {
                            trans = Database.BeginTransaction();

                            string sql = "UPDATE Ticket SET TicketHolder=@TicketHolder,TicketHolderEmail=@TicketHolderEmail,TicketType=@TicketType,Amount=@Amount WHERE ID=@ID";
                            DbParameter par1 = Database.AddParameter("@TicketHolder", ticket.TicketHolder);
                            DbParameter par2 = Database.AddParameter("@ID", ticket.ID);
                            DbParameter par3 = Database.AddParameter("@TicketHolderEmail", ticket.TicketHolderEmail);
                            DbParameter par4 = Database.AddParameter("@TicketType", ticket.TicketType.ID);
                            DbParameter par5 = Database.AddParameter("@Amount", ticket.Amount);

                            rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5);

                            trans.Commit();
                            ApplicationVM.Infotxt("Ticket aangepast", "Ticket aanpassen");
                            return rowsaffected;
                        }
                        catch (Exception)
                        {
                            ApplicationVM.Infotxt("Kan Ticket niet aanpassen", "");
                            trans.Rollback();
                            return 0;
                        }
                    }
                    else
                    {
                        ApplicationVM.Infotxt("Kan Ticket niet aanpassen", "");
                        return 0;
                    }
                }
                else
                {
                    ApplicationVM.Infotxt("Kan Ticket niet aanpassen", "");
                    return 0;
                }
            }
        }

        public static int EditTicket(DbTransaction trans, Ticket ticket, int aantaltickets)
        {
            ObservableCollection<TicketType> types = new ObservableCollection<TicketType>();
            types = TicketType.GetTicketTypes();
            int vorig = tickets[ticket.ID - 1].Amount;

            EditTicketType(ticket.TicketType.ID, ticket.Amount - vorig, aantaltickets);

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE Ticket SET TicketHolder=@TicketHolder,TicketHolderEmail=@TicketHolderEmail,TicketType=@TicketType,Amount=@Amount WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@TicketHolder", ticket.TicketHolder);
                DbParameter par2 = Database.AddParameter("@ID", ticket.ID);
                DbParameter par3 = Database.AddParameter("@TicketHolderEmail", ticket.TicketHolderEmail);
                DbParameter par4 = Database.AddParameter("@TicketType", ticket.TicketType.ID);
                DbParameter par5 = Database.AddParameter("@Amount", ticket.Amount);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5);

                trans.Commit();
                ApplicationVM.Infotxt("Ticket aangepast", "Ticket aanpassen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan Ticket niet aanpassen", "");
                trans.Rollback();
                return 0;
            }
        }

        public static int AddTicket(Ticket ticket)
        {
            ApplicationVM.Infotxt("Ticket toevoegen", "");
            DbTransaction trans = null;

            int aantaltickets = TicketType.ticketType[ticket.TicketType.ID - 1].AvailableTickets;
            ObservableCollection<TicketType> types = TicketType.GetTicketTypes();
            int vorig = types[ticket.TicketType.ID - 1].AvailableTickets;

            if (vorig - ticket.Amount >= 0)
            {
                EditTicketType(ticket.TicketType.ID, ticket.Amount, vorig);

                try
                {
                    trans = Database.BeginTransaction();

                    string sql = "INSERT INTO Ticket VALUES(@ID,@TicketHolder,@TicketHolderEmail,@TicketType,@Amount)";
                    DbParameter par1 = Database.AddParameter("@TicketHolder", ticket.TicketHolder);
                    DbParameter par2 = Database.AddParameter("@ID", aantal);
                    DbParameter par3 = Database.AddParameter("@TicketHolderEmail", ticket.TicketHolderEmail);
                    DbParameter par4 = Database.AddParameter("@TicketType", ticket.TicketType.ID);
                    DbParameter par5 = Database.AddParameter("@Amount", ticket.Amount);

                    int rowsaffected = 0;
                    rowsaffected += Database.ModifyData(trans, sql, par1, par2, par3, par4, par5);

                    trans.Commit();
                    ApplicationVM.Infotxt("Ticket toegevoegd", "Ticket aanpassen");
                    return rowsaffected;
                }
                catch (Exception)
                {
                    ApplicationVM.Infotxt("Kan ticket niet toevoegen", "");
                    trans.Rollback();
                    return 0;
                }
            }
            else
            {
                ApplicationVM.Infotxt("Er zijn niet genoeg tickets beschikbaar", "");
                return 0;
            }
        }

        public static int EditTicketType(int id, int tickets, int vorig)
        {
            ApplicationVM.Infotxt("TicketType aanpassen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "UPDATE TicketType SET AvailableTickets=@AvailableTickets WHERE ID=@ID";
                DbParameter par1 = Database.AddParameter("@ID", id);
                DbParameter par2 = Database.AddParameter("@AvailableTickets", vorig - tickets);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1, par2);

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

        public static int DeleteTicket(Ticket ticket)
        {
            ApplicationVM.Infotxt("Ticket wissen", "");
            DbTransaction trans = null;

            try
            {
                trans = Database.BeginTransaction();

                string sql = "DELETE FROM Ticket WHERE ID = @ID";
                DbParameter par1 = Database.AddParameter("@ID", ticket.ID);

                int rowsaffected = 0;
                rowsaffected += Database.ModifyData(trans, sql, par1);

                trans.Commit();
                ApplicationVM.Infotxt("Ticket gewist", "Ticket wissen");
                return rowsaffected;
            }
            catch (Exception)
            {
                ApplicationVM.Infotxt("Kan ticket niet wissen", "");
                trans.Rollback();
                return 0;
            }
        }

        public override string ToString()
        {
            return ID + " " + TicketHolder + " " + TicketType + " " + TicketHolderEmail + " " + Amount;
        }
    }
}
