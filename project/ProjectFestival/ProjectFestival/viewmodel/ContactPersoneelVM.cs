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

        public ContactPersonType oSelectedType;
        public ContactPersonTitle oSelectedTitle;

        public ContactPersoneelVM()
        {
            TypeList = ContactPersonType.contactType;
            TitleList = ContactPersonTitle.contactTitle;
        }

        private ContactPersonType _selectedType;
        public ContactPersonType SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                SelectedChanged();
                OnPropertyChanged("SelectedType");
                ApplicationVM.SelectedItem = SelectedType;
            }
        }

        private ContactPersonTitle _selectedTitle;
        public ContactPersonTitle SelectedTitle
        {
            get { return _selectedTitle; }
            set
            {
                _selectedTitle = value;
                SelectedChanged();
                OnPropertyChanged("SelectedTitle");
                ApplicationVM.SelectedItem = SelectedTitle;
            }
        }

        public void SelectedChanged()
        {
            if (oSelectedType != null)
            {
                string sql = "SELECT * FROM ContactPersonType WHERE ID=@ID";
                DbParameter par = Database.AddParameter("@ID", oSelectedType.IDDatabase);
                DbDataReader reader = Database.GetData(sql, par);
                reader.Read();

                if (reader.HasRows)
                {
                    ContactPersonType c = ContactPersonType.Create(reader);
                    if ((c.Name != oSelectedType.Name))
                    {
                        DialogResult result = ApplicationVM.MessageQuestion();
                        if (result == DialogResult.Yes)
                        {
                            ContactPersonType.EditType(oSelectedType);
                        }
                        if (result == DialogResult.No)
                        {
                            int id = oSelectedType.ID;
                            c.ID = id;
                            oSelectedType = SelectedType;
                            ContactPersonType.contactType[id - 1] = c;
                        }
                    }
                }
                else
                {
                    if ((oSelectedType.Name != null))
                    {
                        DialogResult result = ApplicationVM.MessageQuestion();
                        if (result == DialogResult.Yes)
                        {
                            ContactPersonType.EditType(oSelectedType);
                        }
                        if (result == DialogResult.No)
                        {
                            ContactPersonType oldSelect = oSelectedType;
                            oSelectedType = SelectedType;
                            ContactPersonType.contactType.Remove(oldSelect);
                        }
                    }
                }
            }

            if (oSelectedTitle != null)
            {
                string sql = "SELECT * FROM ContactPersonTitle WHERE ID=@ID";
                DbParameter par = Database.AddParameter("@ID", oSelectedTitle.IDDatabase);
                DbDataReader reader = Database.GetData(sql, par);
                reader.Read();

                if (reader.HasRows)
                {
                    ContactPersonTitle c = ContactPersonTitle.Create(reader);
                    if ((c.Name != oSelectedTitle.Name))
                    {
                        DialogResult result = ApplicationVM.MessageQuestion();
                        if (result == DialogResult.Yes)
                        {
                            ContactPersonTitle.EditTitle(oSelectedTitle);
                        }
                        if (result == DialogResult.No)
                        {
                            int id = oSelectedTitle.ID;
                            c.ID = id;
                            oSelectedTitle = SelectedTitle;
                            ContactPersonTitle.contactTitle[id - 1] = c;
                        }
                    }
                }
                else
                {
                    if ((oSelectedTitle.Name != null))
                    {
                        DialogResult result = ApplicationVM.MessageQuestion();
                        if (result == DialogResult.Yes)
                        {
                            ContactPersonType.EditType(oSelectedType);
                        }
                        if (result == DialogResult.No)
                        {
                            ContactPersonTitle oldSelect = oSelectedTitle;
                            oSelectedTitle = SelectedTitle;
                            ContactPersonTitle.contactTitle.Remove(oldSelect);
                        }
                    }
                }
            }
            oSelectedTitle = SelectedTitle;
            oSelectedType = SelectedType;
        }
    }
}
