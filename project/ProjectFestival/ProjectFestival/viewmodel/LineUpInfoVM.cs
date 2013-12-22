using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ProjectFestival.viewmodel
{
    class LineUpInfoVM : ObservableObject, IPage
    {
        public string Name
        {
            get { return "Info Bands"; }
        }

        private ObservableCollection<Genre> _genreList;
        public ObservableCollection<Genre> GenreList
        {
            get { return _genreList; }
            set { _genreList = value; OnPropertyChanged("GenreList"); }
        }

        private static Band _selectedBand;
        public static Band SelectedBand
        {
            get { return _selectedBand; }
            set { _selectedBand = value;}
        }

        public ICommand AddImageCommand
        {
            get
            {
                return new RelayCommand<DragEventArgs>(AddImage);
            }
        }

        public ICommand AddPlusImageCommand
        {
            get
            {
                return new RelayCommand<DragEventArgs>(AddPlusImage);
            }
        }

        private void AddPlusImage(DragEventArgs obj)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            ofd.Filter = "jpg Files (.jpg)|*.jpg|jpeg Files (.jpeg)|jpeg|png Files (.png)|*.png";

            if (ofd.ShowDialog() == true)
            {
                SelectedBand.Picture = GetPhoto(ofd.FileName);
                OnPropertyChanged("SelectedBand");
            }
        }

        private void AddPlusImage(DataObject e)
        {
            var data = e as DataObject;
            if (data.ContainsFileDropList())
            {
                var files = data.GetFileDropList();
                SelectedBand.Picture = GetPhoto(files[0]);
                OnPropertyChanged("SelectedBand");
            }
        }

        public ICommand DeleteImageCommand
        {
            get
            {
                return new RelayCommand<DragEventArgs>(DeleteImage);
            }
        }

        private void DeleteImage(DragEventArgs obj)
        {
            SelectedBand.Picture = null;
        }

        private Genre _selectedGenre;
        public Genre SelectedGenre
        {
            get { return _selectedGenre; }
            set
            {
                _selectedGenre = value;
                OnPropertyChanged("SelectedGenre");
                ApplicationVM.SelectedItem = SelectedGenre;
            }
        }

        private void AddImage(DragEventArgs e)
        {
            var data = e.Data as DataObject;
            if (data.ContainsFileDropList())
            {
                var files = data.GetFileDropList();
                SelectedBand.Picture = GetPhoto(files[0]);
                OnPropertyChanged("SelectedBand");
            }
        }

        private byte[] GetPhoto(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] data = new byte[fs.Length];
            fs.Read(data, 0, (int)fs.Length);
            fs.Close();
            return data;
        }

        public LineUpInfoVM()
        {
            _genreList = Band.GenreList;
        }
    }
}
