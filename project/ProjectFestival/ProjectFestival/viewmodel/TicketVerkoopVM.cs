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
    class TicketVerkoopVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Verkoop"; }
        }

        public TicketVerkoopVM()
        {
            _tickeTypetList = TicketType.ticketTypes;
        }
        
        private ObservableCollection<TicketType> _tickeTypetList;
        public ObservableCollection<TicketType> TicketTypeList
        {
            get { return _tickeTypetList; }
            set { _tickeTypetList = value; OnPropertyChanged("TicketTypeList"); }
        }

        private TicketType _selectedTicketType;
        public TicketType SelectedTicketType
        {
            get { return _selectedTicketType; }
            set { _selectedTicketType = value; OnPropertyChanged("SelectedTicketType"); ApplicationVM.SelectedItem = SelectedTicketType; }
        }
    }
}
