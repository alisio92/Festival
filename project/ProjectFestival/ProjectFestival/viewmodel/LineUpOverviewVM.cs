using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.viewmodel
{
    class LineUpOverviewVM : ObservableObject, IPage
    {
        public string Name 
        {
            get { return "Line-Up"; }
        }

        private static Boolean isRunning = false;

        private ObservableCollection<LineUp> _lineUpList;
        public ObservableCollection<LineUp> LineUpList
        {
            get { return _lineUpList; }
            set { _lineUpList = value; OnPropertyChanged("BandList"); }
        }

        public LineUpOverviewVM()
        {
            if (!isRunning)
            {
                isRunning = true;
                LineUpList = LineUp.GetLineUp();
            }
        }
    }
}
