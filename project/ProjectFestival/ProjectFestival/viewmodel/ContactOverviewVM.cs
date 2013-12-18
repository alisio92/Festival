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
    class ContactOverviewVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Contact"; }
        }

        private static Boolean isRunning = false;

        private ObservableCollection<ContactPerson> _contactList;
        public ObservableCollection<ContactPerson> ContactList
        {
            get { return _contactList; }
            set { _contactList = value; OnPropertyChanged("ContactList"); }
        }

        private ContactPerson _selectedContact;
        public ContactPerson SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                OnPropertyChanged("SelectedContact");
                ApplicationVM.SelectedItem = SelectedContact;
            }
        }

        public ContactOverviewVM()
        {
            if (!isRunning)
            {
                isRunning = true;
                ContactPerson.GetContactPerson();
            }
            _contactList = ContactPerson.contactPersons;
        }
    }
}
