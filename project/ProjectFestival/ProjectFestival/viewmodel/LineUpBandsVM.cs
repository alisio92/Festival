using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.viewmodel
{
    class LineUpBandsVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Bands"; }
        }

        private ObservableCollection<Band> _bandList;
        public ObservableCollection<Band> BandList
        {
            get { return _bandList; }
            set { _bandList = value; OnPropertyChanged("BandList"); }
        }

        private Band _selectedBand;
        public Band SelectedBand
        {
            get { return _selectedBand; }
            set
            {
                _selectedBand = value;
                OnPropertyChanged("SelectedBand");
                LineUpInfoVM.SelectedBand = SelectedBand;
                ApplicationVM.SelectedItem = SelectedBand;
            }
        }

        public LineUpBandsVM()
        {
            BandList = Band.GetBands();
        }
    }
}
