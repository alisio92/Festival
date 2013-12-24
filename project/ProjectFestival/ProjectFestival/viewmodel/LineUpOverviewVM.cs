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
            set { _lineUpList = value; OnPropertyChanged("LineUpList"); }
        }

        private ObservableCollection<Stage> _stagesList;
        public ObservableCollection<Stage> StagesList
        {
            get { return _stagesList; }
            set { _stagesList = value; OnPropertyChanged("StagesList"); }
        }

        private ObservableCollection<clock> _urenList;
        public ObservableCollection<clock> UrenList
        {
            get { return _urenList; }
            set { _urenList = value; OnPropertyChanged("UrenList"); }
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
        }
    }
}
