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
    class ContactPersoneelVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Personeel"; }
        }

        public ContactPersoneelVM()
        {
            TypeList = ContactPersonType.contactTypes;
            TitleList = ContactPersonTitle.contactTitles;
        }
        
        private ObservableCollection<ContactPersonType> _typeList;
        public ObservableCollection<ContactPersonType> TypeList
        {
            get { return _typeList; }
            set { _typeList = value; OnPropertyChanged("TypeList"); }
        }

        private ObservableCollection<ContactPersonTitle> _titleList;
        public ObservableCollection<ContactPersonTitle> TitleList
        {
            get { return _titleList; }
            set { _titleList = value; OnPropertyChanged("TitleList"); }
        }

        private ContactPersonType _selectedType;
        public ContactPersonType SelectedType
        {
            get { return _selectedType; }
            set { _selectedType = value; OnPropertyChanged("SelectedType"); ApplicationVM.SelectedItem = SelectedType; }
        }

        private ContactPersonTitle _selectedTitle;
        public ContactPersonTitle SelectedTitle
        {
            get { return _selectedTitle; }
            set { _selectedTitle = value; OnPropertyChanged("SelectedTitle"); ApplicationVM.SelectedItem = SelectedTitle; }
        }
    }
}
