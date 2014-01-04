using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.viewmodel
{
    class LineUpGenreVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Genre & Stage"; }
        }

        public LineUpGenreVM()
        {
            _genreList = Genre.genres;
            _stageList = Stage.stages;
            _festivalList = Festival.festivals;
        }
        
        private ObservableCollection<Genre> _genreList;
        public ObservableCollection<Genre> GenreList
        {
            get { return _genreList; }
            set { _genreList = value; OnPropertyChanged("GenreList"); }
        }

        private ObservableCollection<Stage> _stageList;
        public ObservableCollection<Stage> StageList
        {
            get { return _stageList; }
            set { _stageList = value; OnPropertyChanged("StageList"); }
        }

        private ObservableCollection<Festival> _festivalList;
        public ObservableCollection<Festival> FestivalList
        {
            get { return _festivalList; }
            set { _festivalList = value; OnPropertyChanged("FestivalList"); }
        }
        
        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get { return _selectedGenre; }
            set { _selectedGenre = value; OnPropertyChanged("SelectedGenre"); ApplicationVM.SelectedItem = SelectedGenre; }
        }

        private Stage _selectedStage;
        public Stage SelectedStage
        {
            get { return _selectedStage; }
            set { _selectedStage = value; OnPropertyChanged("SelectedStage"); ApplicationVM.SelectedItem = SelectedStage; }
        }

        private Festival _selectedFestival;
        public Festival SelectedFestival
        {
            get { return _selectedFestival; }
            set { _selectedFestival = value; OnPropertyChanged("SelectedFestival"); ApplicationVM.SelectedItem = SelectedFestival; }
        }
    }
}
