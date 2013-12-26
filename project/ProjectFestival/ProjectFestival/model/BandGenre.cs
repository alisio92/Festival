using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFestival.model
{
    public class BandGenre
    {
        private static ObservableCollection<Genre> _genreList;
        public static ObservableCollection<Genre> GenreList
        {
            get { return _genreList; }
            set { _genreList = value; }
        }

        private int _ID;
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private Genre _genreBand;
        public Genre GenreBand
        {
            get { return _genreBand; }
            set { _genreBand = value; }
        }

        public static int aantal = 1;

        public static ObservableCollection<BandGenre> bandGenre;

        public static ObservableCollection<BandGenre> GetBandgenres(ObservableCollection<Genre> genreListBand)
        {
            bandGenre = new ObservableCollection<BandGenre>();
            aantal = 1;
            GenreList = Band.GenreList;
            foreach (Genre g in genreListBand)
            {
                BandGenre b = new BandGenre();
                b.ID = aantal;
                b.GenreBand = g;
                bandGenre.Add(b);
                aantal++;
            }

            return bandGenre;
        }
    }
}
