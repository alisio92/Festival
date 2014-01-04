using GalaSoft.MvvmLight.Command;
using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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
            set { _lineUpList = value; OnPropertyChanged("LineUpList"); }
        }

        private ObservableCollection<Stage> _stagesList;
        public ObservableCollection<Stage> StagesList
        {
            get { return _stagesList; }
            set { _stagesList = value; OnPropertyChanged("StagesList"); }
        }

        private ObservableCollection<Band> _bandList;
        public ObservableCollection<Band> BandsList
        {
            get { return _bandList; }
            set { _bandList = value; OnPropertyChanged("BandsList"); }
        }

        private ObservableCollection<Festival> _dateList;
        public ObservableCollection<Festival> DateList
        {
            get { return _dateList; }
            set { _dateList = value; OnPropertyChanged("DateList"); }
        }

        private LineUp _selectedLineUp;
        public LineUp SelectedLineUp
        {
            get { return _selectedLineUp; }
            set 
            {
                _selectedLineUp = value;
                OnPropertyChanged("SelectedLineUp");
                ApplicationVM.SelectedItem = SelectedLineUp;
            }
        }

        public LineUpOverviewVM()
        {
            if (!isRunning)
            {
                isRunning = true;
                LineUpList = LineUp.GetLineUp();
            }
            StagesList = Stage.stages;
            LineUpList = LineUp.lineUp;
            BandsList = LineUp.BandList;
            DateList = Festival.festivals;
        }
    }
}
