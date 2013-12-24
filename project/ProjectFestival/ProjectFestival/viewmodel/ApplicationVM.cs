using GalaSoft.MvvmLight.Command;
using ProjectFestival.model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ProjectFestival.viewmodel
{
    class ApplicationVM : ObservableObject
    {
        public ApplicationVM()
        {
            Infotxt("Applicatie starten...", "");
            PagesMainNav.Add(new ContactOverviewVM());
            PagesMainNav.Add(new LineUpOverviewVM());
            PagesMainNav.Add(new TicketOverviewVM());

            CurrentPage = PagesMainNav[0];
            subNav();
            Infotxt("Applicatie starten klaar", "Applicatie starten...");
        }

        public static void Infotxt(string infoNew, string infoOld)
        {
            if (infoOld == "")
            {
                Info += infoNew + "\n";
            }
            else
            {
                Info = Info.Replace(infoOld, infoNew);
            }
        }

        private String _search = "Zoeken";
        public String Search
        {
            get { return _search; }
            set
            {
                _search = value;
                OnPropertyChanged("Search");
            }
        }

        private static String _info;
        public static String Info
        {
            get { return _info; }
            set { _info = value; }
        }
        private void subNav()
        {
            PagesSubNav.Clear();

            if (CurrentPage.Name == "Contact")
            {
                PagesSubNav.Add(new ContactOverviewVM());
                PagesSubNav.Add(new ContactPersoneelVM());
            }
            else if (CurrentPage.Name == "Tickets")
            {
                PagesSubNav.Add(new TicketOverviewVM());
                PagesSubNav.Add(new TicketVerkoopVM());
            }
            else if (CurrentPage.Name == "Line-Up")
            {
                PagesSubNav.Add(new LineUpOverviewVM());
                PagesSubNav.Add(new LineUpBandsVM());
                PagesSubNav.Add(new LineUpGenreVM());
            }
        }

        private IPage _currentpage;
        public IPage CurrentPage
        {
            get { return _currentpage; }
            set
            {
                _currentpage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        private ObservableCollection<IPage> _pagesMainNav;
        public ObservableCollection<IPage> PagesMainNav
        {
            get
            {
                if (_pagesMainNav == null)
                    _pagesMainNav = new ObservableCollection<IPage>();
                return _pagesMainNav;
            }
        }

        private ObservableCollection<IPage> _pagesSubNav;
        public ObservableCollection<IPage> PagesSubNav
        {
            get
            {
                if (_pagesSubNav == null)
                    _pagesSubNav = new ObservableCollection<IPage>();
                return _pagesSubNav;
            }
        }

        public ICommand ChangePageCommand
        {
            get { return new RelayCommand<IPage>(ChangePage); }
        }

        private void ChangePage(IPage page)
        {
            CurrentPage = page;
            if ((CurrentPage.Name == "Contact") || (CurrentPage.Name == "Line-Up") || (CurrentPage.Name == "Tickets"))
            {
                subNav();
            }
            try
            {
                if (CurrentPage.Name == "Contact")
                {
                    if (ContactPerson.JobRoleList[ContactPersonType.aantal - 1].Name == null)
                    {
                        ContactPerson.JobRoleList.RemoveAt(ContactPersonType.aantal - 1);
                    }
                    if (ContactPerson.JobTitleList[ContactPersonTitle.aantal - 1].Name == null)
                    {
                        ContactPerson.JobTitleList.RemoveAt(ContactPersonTitle.aantal - 1);
                    }
                }
                if (CurrentPage.Name == "Tickets")
                {
                    if (Ticket.TicketTypeList[TicketType.aantal - 1].Name == null)
                    {
                        Ticket.TicketTypeList.RemoveAt(TicketType.aantal - 1);
                    }
                }
                if (CurrentPage.Name == "Verkoop")
                {
                    CurrentPage = new TicketVerkoopVM();
                }
            }
            catch (Exception)
            {
            }
        }

        private static object _selectedItem;
        public static object SelectedItem
        {
            get { return _selectedItem; }
            set { _selectedItem = value; }
        }

        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }

        public ICommand SaveItemCommand
        {
            get { return new RelayCommand(SaveItem); }
        }

        public ICommand DeleteItemCommand
        {
            get { return new RelayCommand(DeleteItem); }
        }

        public ICommand AddItemCommand
        {
            get { return new RelayCommand(AddItem); }
        }

        public ICommand SearchCommand
        {
            get { return new RelayCommand(SearchItem); }
        }

        public ICommand FocusCommand
        {
            get { return new RelayCommand(Focus); }
        }

        public ICommand LostFocusCommand
        {
            get { return new RelayCommand(LostFocus); }
        }

        private void LostFocus()
        {
            if (Search == "")
            {
                Search = "Zoeken";
            }
        }

        private void Focus()
        {
            if (Search == "Zoeken")
            {
                Search = "";
            }
        }

        private void SearchItem()
        {
            Zoeken();
        }

        private void Zoeken()
        {

            if (CurrentPage.Name == "Contact")
            {
                ContactPerson.Zoeken(Search);
                CurrentPage = new ContactOverviewVM();
            }
            if (CurrentPage.Name == "Personeel")
            {
                ContactPersonType.Zoeken(Search);
                ContactPersonTitle.Zoeken(Search);
                CurrentPage = new ContactPersoneelVM();
            }
            if (CurrentPage.Name == "Verkoop")
            {
                TicketType.Zoeken(Search);
                CurrentPage = new TicketVerkoopVM();
            }
            if (CurrentPage.Name == "Tickets")
            {
                Ticket.Zoeken(Search);
                CurrentPage = new TicketOverviewVM();
            }
            if (CurrentPage.Name == "Genre & Stage")
            {
                Genre.Zoeken(Search);
                Stage.Zoeken(Search);
                CurrentPage = new LineUpGenreVM();
            }
            if (CurrentPage.Name == "Bands")
            {
                Band.Zoeken(Search);
                CurrentPage = new LineUpBandsVM();
            }
            if (CurrentPage.Name == "Line-Up")
            {
                LineUp.Zoeken(Search);
                CurrentPage = new LineUpOverviewVM();
            }
        }

        private void AddItem()
        {
            if (CurrentPage.Name == "Personeel")
            {
                ContactPersonType cp = new ContactPersonType();
                cp.ID = ContactPersonType.aantal;
                ContactPerson.JobRoleList.Add(cp);
                ContactPersonTitle cpt = new ContactPersonTitle();
                cpt.ID = ContactPersonTitle.aantal;
                ContactPerson.JobTitleList.Add(cpt);
                //ContactPersonTitle.aantal++;
                //ContactPersonType.aantal++;
            }
            if (CurrentPage.Name == "Contact")
            {
                ContactPerson cp = new ContactPerson();
                cp.JobRole = new ContactPersonType();
                cp.JobTitle = new ContactPersonTitle();
                cp.ID = ContactPerson.aantal;
                ContactPerson.contactPersons.Add(cp);
                //ContactPerson.aantal++;
            }
            if (CurrentPage.Name == "Tickets")
            {
                Ticket t = new Ticket();
                t.TicketType = new TicketType();
                t.ID = Ticket.aantal;
                Ticket.tickets.Add(t);
                //Ticket.aantal++;
            }
            if (CurrentPage.Name == "Verkoop")
            {
                TicketType t = new TicketType();
                t.ID = TicketType.aantal;
                Ticket.TicketTypeList.Add(t);
                //TicketType.aantal++;
            }

            if (CurrentPage.Name == "Genre & Stage")
            {
                Genre g = new Genre();
                g.ID = Genre.aantal;
                Band.GenreList.Add(g);
                Stage s = new Stage();
                s.ID = Stage.aantal;
                LineUp.StageList.Add(s);
                //TicketType.aantal++;
            }
            if (CurrentPage.Name == "Bands")
            {
                Band b = new Band();
                b.ID = Band.aantal;
                Band.bands.Add(b);
            }
            if (CurrentPage.Name == "Info Bands")
            {
                BandGenre g2 = new BandGenre();
                g2.GenreBand = new Genre();
                g2.ID = BandGenre.aantal;
                BandGenre.bandGenre.Add(g2);
            }
        }

        public void SaveItem()
        {
            if (CurrentPage.Name == "Contact")
            {
                ContactSaveItem();
            }
            if (CurrentPage.Name == "Personeel")
            {
                PersoneelSaveItem();
            }
            if (CurrentPage.Name == "Tickets")
            {
                TicketsSaveItem();
            }
            if (CurrentPage.Name == "Verkoop")
            {
                VerkoopSaveItem();
            }
            if (CurrentPage.Name == "Genre & Stage")
            {
                GenreStageSaveItem();
            }
            if (CurrentPage.Name == "Info Bands")
            {
                InfoBandSaveItem();
                CurrentPage = new LineUpInfoVM();
            }
            if (CurrentPage.Name == "Line-Up")
            {
                LineUpSaveItem();
            }
            if (CurrentPage.Name == "Bands")
            {
                BandsSaveItem();
            }
        }

        private void BandsSaveItem()
        {
            CurrentPage = new LineUpInfoVM();
        }

        private void LineUpSaveItem()
        {
            LineUp.AddLineUp((LineUp)SelectedItem);
            //LineUp.JsonWegschrijven();
        }

        private void InfoBandSaveItem()
        {
            Band band = (Band)SelectedItem;
            int id = Convert.ToInt32(band.ID);

            if (id != Band.aantal)
            {
                Band.EditBand(band);
                Band.bands[id - 1] = band;
            }
            else
            {
                Band.AddBand(band);
                id = Band.aantal;
                band.ID = id;
                Band.bands[id - 1] = new Band();
                Band.bands[id - 1] = band;
            }
        }

        private void GenreStageSaveItem()
        {
            int id = 0;
            Stage stage = null;
            Genre genre = null;

            if (SelectedItem.GetType() == typeof(Stage))
            {

                stage = (Stage)SelectedItem;
                id = Convert.ToInt32(stage.ID);

                if (id != Stage.aantal)
                {
                    Stage.EditStage(stage);
                    LineUp.StageList[id - 1] = stage;
                }
                else
                {
                    Stage.AddStage(stage);
                    id = Stage.aantal;
                    stage.ID = id;
                    LineUp.StageList[id - 1] = new Stage();
                    LineUp.StageList[id - 1] = stage;
                }
            }
            else if (SelectedItem.GetType() == typeof(Genre))
            {
                genre = (Genre)SelectedItem;
                id = Convert.ToInt32(genre.ID);

                if (id != Genre.aantal)
                {
                    Genre.EditGenre(genre);
                    Band.GenreList[id - 1] = genre;
                }
                else
                {
                    Genre.AddGenre(genre);
                    id = Genre.aantal;
                    genre.ID = id;
                    Band.GenreList[id - 1] = new Genre();
                    Band.GenreList[id - 1] = genre;
                }
            }
        }

        private void VerkoopSaveItem()
        {
            TicketType ticketType = (TicketType)SelectedItem;
            int id = Convert.ToInt32(ticketType.ID);

            if (id != TicketType.aantal)
            {
                TicketType.EditType(ticketType);
                Ticket.TicketTypeList[id - 1] = ticketType;
            }
            else
            {
                TicketType.AddType(ticketType);
                id = TicketType.aantal;
                ticketType.ID = id;
                Ticket.TicketTypeList[id - 1] = new TicketType();
                Ticket.TicketTypeList[id - 1] = ticketType;
            }
        }

        private void TicketsSaveItem()
        {
            Ticket ticket = (Ticket)SelectedItem;
            int id = Convert.ToInt32(ticket.ID);

            if (id != Ticket.aantal)
            {
                Ticket.EditTicket(ticket);
                Ticket.tickets[id - 1] = ticket;
            }
            else
            {
                Ticket.AddTicket(ticket);
                id = Ticket.aantal;
                ticket.ID = id;
                Ticket.tickets[id - 1] = new Ticket();
                Ticket.tickets[id - 1] = ticket;
            }
        }

        private void PersoneelSaveItem()
        {
            int id = 0;
            ContactPersonType contactPersonType = null;
            ContactPersonTitle contactPersonTitle = null;

            if (SelectedItem.GetType() == typeof(ContactPersonType))
            {

                contactPersonType = (ContactPersonType)SelectedItem;
                id = Convert.ToInt32(contactPersonType.ID);

                if (id != ContactPersonType.aantal)
                {
                    ContactPersonType.EditType(contactPersonType);
                    ContactPerson.JobRoleList[id - 1] = contactPersonType;
                }
                else
                {
                    ContactPersonType.AddType(contactPersonType);
                    id = ContactPersonType.aantal;
                    contactPersonType.ID = id;
                    ContactPerson.JobRoleList[id - 1] = new ContactPersonType();
                    ContactPerson.JobRoleList[id - 1] = contactPersonType;
                }
            }
            else if (SelectedItem.GetType() == typeof(ContactPersonTitle))
            {
                contactPersonTitle = (ContactPersonTitle)SelectedItem;
                id = Convert.ToInt32(contactPersonTitle.ID);

                if (id != ContactPersonTitle.aantal)
                {
                    ContactPersonTitle.EditTitle(contactPersonTitle);
                    ContactPerson.JobTitleList[id - 1] = contactPersonTitle;
                }
                else
                {
                    ContactPersonTitle.AddTitle(contactPersonTitle);
                    id = ContactPersonTitle.aantal;
                    contactPersonTitle.ID = id;
                    ContactPerson.JobTitleList[id - 1] = new ContactPersonTitle();
                    ContactPerson.JobTitleList[id - 1] = contactPersonTitle;
                }
            }
        }

        private void ContactSaveItem()
        {
            ContactPerson contactPersoon = (ContactPerson)SelectedItem;
            int id = Convert.ToInt32(contactPersoon.ID);

            if (id != ContactPerson.aantal)
            {
                ContactPerson.EditContact(contactPersoon);
                ContactPerson.contactPersons[id - 1] = contactPersoon;
            }
            else
            {
                ContactPerson.AddContact(contactPersoon);
                id = ContactPerson.aantal;
                contactPersoon.ID = id;
                ContactPerson.contactPersons[id - 1] = new ContactPerson();
                ContactPerson.contactPersons[id - 1] = contactPersoon;
            }
        }

        public void DeleteItem()
        {
            if (CurrentPage.Name == "Contact")
            {
                ContactDeleteItem();
            }
            if (CurrentPage.Name == "Personeel")
            {
                PersoneelDeleteItem();
            }
            if (CurrentPage.Name == "Tickets")
            {
                TicketsDeleteItem();
            }
            if (CurrentPage.Name == "Verkoop")
            {
                VerkoopDeleteItem();
            }
            if (CurrentPage.Name == "Genre & Stage")
            {
                GenreStageDeleteItem();
            }
            if (CurrentPage.Name == "Bands")
            {
                BandDeleteItem();
            }
            if (CurrentPage.Name == "Info Bands")
            {
                GenreDeleteItem();
            }
        }

        private void GenreDeleteItem()
        {
            BandGenre bandGenre = (BandGenre)SelectedItem;
            BandGenre.bandGenre.Remove(bandGenre);
        }

        private void BandDeleteItem()
        {
            Band band = (Band)SelectedItem;
            int id = Convert.ToInt32(band.ID);
            Band.DeleteBand(band);
            Band.bands.Remove(band);
        }

        private void GenreStageDeleteItem()
        {
            if (SelectedItem.GetType() == typeof(Stage))
            {
                Stage stage = (Stage)SelectedItem;
                int id = Convert.ToInt32(stage.ID);
                Stage.DeleteStage(stage);
                LineUp.StageList.Remove(stage);
            }
            else if (SelectedItem.GetType() == typeof(Genre))
            {
                Genre genre = (Genre)SelectedItem;
                int id = Convert.ToInt32(genre.ID);
                Genre.DeleteGenre(genre);
                Band.GenreList.Remove(genre);
            }
        }

        private void VerkoopDeleteItem()
        {
            TicketType ticket = (TicketType)SelectedItem;
            int id = Convert.ToInt32(ticket.ID);
            TicketType.DeleteType(ticket);
            Ticket.TicketTypeList.Remove(ticket);
        }

        private void TicketsDeleteItem()
        {
            Ticket ticket = (Ticket)SelectedItem;
            int id = Convert.ToInt32(ticket.ID);
            Ticket.DeleteTicket(ticket);
            Ticket.tickets.Remove(ticket);
        }

        private void PersoneelDeleteItem()
        {
            if (SelectedItem.GetType() == typeof(ContactPersonType))
            {
                ContactPersonType contactPersonType = (ContactPersonType)SelectedItem;
                int id = Convert.ToInt32(contactPersonType.ID);
                ContactPersonType.DeleteType(contactPersonType);
                ContactPerson.JobRoleList.Remove(contactPersonType);
            }
            else if (SelectedItem.GetType() == typeof(ContactPersonTitle))
            {
                ContactPersonTitle contactPersonTitle = (ContactPersonTitle)SelectedItem;
                int id = Convert.ToInt32(contactPersonTitle.ID);
                ContactPersonTitle.DeleteTitle(contactPersonTitle);
                ContactPerson.JobTitleList.Remove(contactPersonTitle);
            }
        }

        private void ContactDeleteItem()
        {
            ContactPerson contactPersoon = (ContactPerson)SelectedItem;
            int id = Convert.ToInt32(contactPersoon.ID);
            ContactPerson.DeleteContact(contactPersoon);
            ContactPerson.contactPersons.Remove(contactPersoon);
        }
    }
}
