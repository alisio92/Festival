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

        private ObservableCollection<LineUp> _lineUpSelected = new ObservableCollection<LineUp>();
        public ObservableCollection<LineUp> LineUpSelected 
        {
            get { return _lineUpSelected; }
            set { _lineUpSelected = value; OnPropertyChanged("LineUpSelected"); }
        }

        private LineUp _selectedLineUp;
        public LineUp SelectedLineUp
        {
            get { return _selectedLineUp; }
            set { _selectedLineUp = value; }
        }
        
        private ObservableCollection<clock> _urenList;
        public ObservableCollection<clock> UrenList
        {
            get { return _urenList; }
            set { _urenList = value; OnPropertyChanged("UrenList"); }
        }

        public ICommand Clickcommand
        {
            get { return new RelayCommand(ClickEvent); }
        }

        private void ClickEvent()
        {
            LineUpSelected[0] = LineUp.lineUp[LineUp.index-1]; 
        }

        public LineUpOverviewVM()
        {
            if (!isRunning)
            {
                isRunning = true;
                LineUpList = LineUp.GetLineUp();
                UrenList = clock.GetUren();
            }
            UrenList = clock.uren;
            StagesList = Stage.stages;
            LineUpList = LineUp.lineUp;
            BandsList = LineUp.BandList;
            LineUpSelected.Add(LineUp.lineUp[LineUp.index - 1]);
        }
    }
}
