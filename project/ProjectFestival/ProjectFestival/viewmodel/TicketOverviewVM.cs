using GalaSoft.MvvmLight.Command;
using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectFestival.viewmodel
{
    class TicketOverviewVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Tickets"; }
        }

        private static Boolean isRunning = false;

        private ObservableCollection<Ticket> _ticketList;
        public ObservableCollection<Ticket> TicketList
        {
            get { return _ticketList; }
            set { _ticketList = value; OnPropertyChanged("TicketList"); }
        }

        private Ticket _selectedTicket;
        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set { _selectedTicket = value; OnPropertyChanged("SelectedTicket"); ApplicationVM.SelectedItem = SelectedTicket; }
        }

        public TicketOverviewVM()
        {
            if (!isRunning)
            {
                isRunning = true;
                Ticket.GetTickets();
            }
            _ticketList = Ticket.tickets;
        }
    }
}
