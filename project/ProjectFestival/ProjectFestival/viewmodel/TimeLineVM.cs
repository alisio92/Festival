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
    class TimeLineVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "TimeLine"; }
        }

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

        private ObservableCollection<clock> _urenList;
        public ObservableCollection<clock> UrenList
        {
            get { return _urenList; }
            set { _urenList = value; OnPropertyChanged("UrenList"); }
        }

        public TimeLineVM()
        {
            UrenList = clock.GetUren();
            StagesList = Stage.stages;
            LineUpList = LineUp.sLineUp;
            BandsList = LineUp.BandList;
        }
    }
}
