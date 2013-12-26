using GalaSoft.MvvmLight.Command;
using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        
        public ContactPerson oSelected;

        private ContactPerson _selectedContact;
        public ContactPerson SelectedContact
        {
            get { return _selectedContact; }
            set
            {
                _selectedContact = value;
                //SelectedChanged();
                OnPropertyChanged("SelectedContact");
                ApplicationVM.SelectedItem = SelectedContact;
            }
        }

        public void SelectedChanged()
        {
            if (oSelected != null)
            {
                string sql = "SELECT * FROM ContactPerson WHERE ID=@ID";
                DbParameter par = Database.AddParameter("@ID", oSelected.IDDatabase);
                DbDataReader reader = Database.GetData(sql, par);
                reader.Read();

                if (reader.HasRows)
                {
                    ContactPerson c = ContactPerson.Create(reader);
                    if ((c.Name != oSelected.Name) || (c.JobRole.Name != oSelected.JobRole.Name) || (c.JobTitle.Name != oSelected.JobTitle.Name) || (c.Phone != oSelected.Phone) || (c.Cellphone != oSelected.Cellphone) || (c.Company != oSelected.Company) || (c.Email != oSelected.Email) || (c.City != oSelected.City))
                    {
                        DialogResult result = ApplicationVM.MessageQuestion();
                        if (result == DialogResult.Yes)
                        {
                            ContactPerson.EditContact(oSelected);
                        }
                        if (result == DialogResult.No)
                        {
                            int id = oSelected.ID;
                            c.ID = id;
                            oSelected = SelectedContact;
                            ContactPerson.contactPersons[id - 1] = c;
                        }
                    }
                }
                else
                {
                    if ((oSelected.Name!=null) || (oSelected.JobRole.Name!=null) || ( oSelected.JobTitle.Name!=null))
                    {
                        DialogResult result = ApplicationVM.MessageQuestion();
                        if (result == DialogResult.Yes)
                        {
                            ContactPerson.EditContact(oSelected);
                        }
                        if (result == DialogResult.No)
                        {
                            ContactPerson oldSelect = oSelected;
                            oSelected = SelectedContact;
                            ContactPerson.contactPersons.Remove(oldSelect);
                        }
                    }
                }
            }
            oSelected = SelectedContact;
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
