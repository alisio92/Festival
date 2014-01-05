using GalaSoft.MvvmLight.Command;
using ProjectFestival.model;
using ProjectFestival.writetofile;
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
            PagesMainNav.Add(new ContactOverviewVM());
            PagesMainNav.Add(new LineUpOverviewVM());
            PagesMainNav.Add(new TicketOverviewVM());

            CurrentPage = PagesMainNav[0];
            subNav();
            FileWriter.JsonWegschrijven();
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
                PagesSubNav.Add(new TimeLineVM());
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

        private static BandGenre _bandGenre;
        public static BandGenre BandGenre
        {
            get { return _bandGenre; }
            set { _bandGenre = value; }
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

        public static DialogResult MessageQuestion()
        {
            return MessageBox.Show("Wilt u alles opslaan?", "Aanpassen", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public static void MessageInfo()
        {
            MessageBox.Show("Het laatste item mag niet leeg zijn.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MessageDelete()
        {
            MessageBox.Show("Je moet een item selecteren om te wissen.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MessageTickets()
        {
            MessageBox.Show("Er zijn geen genoeg tickets beschikbaar.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddItem()
        {
            if (CurrentPage.Name == "Personeel")
            {
                AddPersoneel();
            }
            if (CurrentPage.Name == "Contact")
            {
                AddContact();
            }
            if (CurrentPage.Name == "Tickets")
            {
                AddTicket();
            }
            if (CurrentPage.Name == "Verkoop")
            {
                AddTicketType();
            }
            if (CurrentPage.Name == "Genre & Stage")
            {
                AddGenreStage();
            }
            if (CurrentPage.Name == "Bands")
            {
                AddBand();
            }
            if (CurrentPage.Name == "Info Bands")
            {
                AddInfoBand();
            }
            if (CurrentPage.Name == "Line-Up")
            {
                AddLineUp();
            }
        }

        private void AddLineUp()
        {
            bool isMessage = false;
            foreach (LineUp lineUp in LineUp.lineUp)
            {
                if (lineUp.Band.Name == null || lineUp.Stage.Name == null || lineUp.From == null || lineUp.Until == null || lineUp.Date == null)
                {
                    isMessage = true;
                }
            }
            if (isMessage)
            {
                MessageInfo();
            }
            else
            {
                LineUp lineUp = new LineUp();
                lineUp.Band = new Band();
                lineUp.Stage = new Stage();
                lineUp.Date = new DateTime();
                lineUp.Date = DateTime.Today;
                lineUp.ID = LineUp.aantal;
                LineUp.lineUp.Add(lineUp);
                LineUp.aantal++;
            }
        }

        private void AddInfoBand()
        {
            bool isMessage = false;
            foreach (Band band in Band.bands)
            {
                if (band.GenreListBand[band.GenreListBand.Count - 1].Name == null)
                {
                    isMessage = true;
                }
            }
            if (isMessage)
            {
                MessageInfo();
            }
            else
            {
                BandGenre bandGenre = new BandGenre();
                bandGenre.GenreBand = new Genre();
                bandGenre.ID = BandGenre.aantal;
                BandGenre.bandGenre.Add(bandGenre);
                BandGenre.aantal++;
            }
        }

        private void AddBand()
        {
            bool isMessage = false;
            foreach (Band band in Band.bands)
            {
                if (band.Name == null || band.Facebook == null || band.Twitter == null)
                {
                    isMessage = true;
                }
            }
            if (isMessage)
            {
                MessageInfo();
            }
            else
            {
                Band b = new Band();
                b.ID = Band.aantal;
                b.GenreListBand = new ObservableCollection<Genre>();
                Genre g = new Genre();
                g.ID = Genre.aantal;
                b.GenreListBand.Add(g);
                Band.bands.Add(b);
                Band.aantal++;
                BandGenre.aantal++;
            }
        }

        private void AddGenreStage()
        {
            bool isMessageGenre = false;
            bool isMessageStage = false;
            bool isMessageFestival = false;
            foreach (Genre genre in Genre.genres)
            {
                if (genre.Name == null)
                {
                    isMessageGenre = true;
                }
            }
            foreach (Stage stage in Stage.stages)
            {
                if (stage.Name == null)
                {
                    isMessageStage = true;
                }
            }
            foreach (Festival festival in Festival.festivals)
            {
                if (festival.StartDate == null || festival.EndDate == null)
                {
                    isMessageFestival = true;
                }
            }
            if (isMessageGenre && isMessageStage && isMessageFestival)
            {
                MessageInfo();
            }
            if (!isMessageGenre)
            {
                Genre genre = new Genre();
                genre.ID = Genre.aantal;
                Band.GenreList.Add(genre);
                Genre.aantal++;
            }
            if (!isMessageStage)
            {
                Stage stage = new Stage();
                stage.ID = Stage.aantal;
                LineUp.StageList.Add(stage);
                Stage.aantal++;
            }
            if (!isMessageFestival)
            {
                Festival festval = new Festival();
                festval.ID = Stage.aantal;
                festval.StartDate = new DateTime();
                festval.StartDate = DateTime.Today;
                festval.EndDate = new DateTime();
                festval.EndDate = DateTime.Today;
                Festival.festivals.Add(festval);
                Festival.aantal++;
            }
        }

        private void AddTicket()
        {
            bool isMessage = false;
            foreach (Ticket ticket in Ticket.tickets)
            {
                if (ticket.TicketHolder == null || ticket.TicketHolderEmail == null || ticket.TicketType.Name == null || ticket.Amount == 0)
                {
                    isMessage = true;
                }
            }
            if (isMessage)
            {
                MessageInfo();
            }
            else
            {
                Ticket t = new Ticket();
                t.TicketType = new TicketType();
                t.ID = Ticket.aantal;
                Ticket.tickets.Add(t);
                Ticket.aantal++;
                CurrentPage = new TicketOverviewVM();
            }
        }

        private void AddTicketType()
        {
            bool isMessage = false;
            foreach (TicketType tType in TicketType.ticketTypes)
            {
                if (tType.Name == null || tType.Price == 0 || tType.AvailableTickets == 0)
                {
                    isMessage = true;
                }
            }
            if (isMessage)
            {
                MessageInfo();
            }
            else
            {
                TicketType tType = new TicketType();
                tType.ID = TicketType.aantal;
                Ticket.TicketTypeList.Add(tType);
                TicketType.aantal++;
            }
        }

        private void AddContact()
        {
            bool isMessage = false;
            foreach (ContactPerson cp in ContactPerson.contactPersons)
            {
                if (cp.Name == null || cp.JobRole.Name == null || cp.JobTitle.Name == null)
                {
                    isMessage = true;
                }
            }
            if (isMessage)
            {
                MessageInfo();
            }
            else
            {
                ContactPerson cp = new ContactPerson();
                cp.JobRole = new ContactPersonType();
                cp.JobTitle = new ContactPersonTitle();
                cp.ID = ContactPerson.aantal;
                ContactPerson.contactPersons.Add(cp);
                ContactPerson.aantal++;
            }
        }

        private void AddPersoneel()
        {
            bool isMessageType = false;
            bool isMessageTitle = false;
            foreach (ContactPersonType cpType in ContactPersonType.contactTypes)
            {
                if (cpType.Name == null)
                {
                    isMessageType = true;
                }
            }
            foreach (ContactPersonTitle cpTitle in ContactPersonTitle.contactTitles)
            {
                if (cpTitle.Name == null)
                {
                    isMessageTitle = true;
                }
            }
            if (isMessageType && isMessageTitle)
            {
                MessageInfo();
            }
            if (!isMessageType)
            {
                ContactPersonType cpType = new ContactPersonType();
                cpType.ID = ContactPersonType.aantal;
                ContactPerson.JobRoleList.Add(cpType);
                ContactPersonType.aantal++;
            }
            if (!isMessageTitle)
            {
                ContactPersonTitle cpTitle = new ContactPersonTitle();
                cpTitle.ID = ContactPersonTitle.aantal;
                ContactPerson.JobTitleList.Add(cpTitle);
                ContactPersonTitle.aantal++;
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
                genrestageSaveItem();
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
            if (CurrentPage.Name == "TimeLine")
            {
                if (SelectedItem!=null)
                {
                    TimeLineVM.newDate = (DateTime)SelectedItem;
                    CurrentPage = new TimeLineVM();
                }
            }
        }

        private void BandsSaveItem()
        {
            if (SelectedItem != null)
            {
                CurrentPage = new LineUpInfoVM();
            }
            else
            {
                MessageBox.Show("Je moet een item selecteren.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LineUpSaveItem()
        {
            LineUp lineUp = (LineUp)SelectedItem;

            if (lineUp != null)
            {
                String[] from = lineUp.From.Split(new Char[] { ':' });
                String[] until = lineUp.Until.Split(new Char[] { ':' });
                int uurUntil = Convert.ToInt32(until[0]);
                int uurFrom = Convert.ToInt32(from[0]);
                double minutenUntil = Convert.ToDouble(until[1]);
                double minutenFrom = Convert.ToDouble(from[1]);
                double uur = (uurUntil + minutenUntil / 60) - (uurFrom + minutenFrom / 60);

                if (uur >= 0.50)
                {
                    int id = LineUp.lineUp.IndexOf(lineUp);
                    id = LineUp.lineUp[id].IDDatabase;

                    if (id != 0)
                    {
                        LineUp.EditLineUp(lineUp);
                    }
                    else
                    {
                        LineUp.AddLineUp(lineUp);
                    }
                }
                else
                {
                    MessageBox.Show("Het verschil tss einduur en beginuur kan niet minder dan 30 min zijn.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                DialogResult result = MessageQuestion();
                if (result == DialogResult.Yes)
                {
                    foreach (LineUp lineup in LineUp.lineUp)
                    {
                        LineUp.EditLineUp(lineup);
                    }
                }
            }
        }

        private void InfoBandSaveItem()
        {
            Band band = (Band)SelectedItem;
            if (band != null)
            {
                int id = Band.bands.IndexOf(band);
                id = Band.bands[id].IDDatabase;

                if (id != 0)
                {
                    Band.EditBand(band);
                }
                else
                {
                    Band.AddBand(band);
                }
            }
            else
            {
                DialogResult result = MessageQuestion();
                if (result == DialogResult.Yes)
                {
                    foreach (Band b in Band.bands)
                    {
                        Band.EditBand(b);
                    }
                }
            }
        }

        private void genrestageSaveItem()
        {
            int id = 0;
            Stage stage = null;
            Genre genre = null;
            Festival festival = null;

            if (SelectedItem != null)
            {
                if (SelectedItem.GetType() == typeof(Stage))
                {
                    stage = (Stage)SelectedItem;
                    id = Stage.stages.IndexOf(stage);
                    id = Stage.stages[id].IDDatabase;

                    if (id != 0)
                    {
                        Stage.EditStage(stage);
                    }
                    else
                    {
                        Stage.AddStage(stage);
                    }
                }
                else if (SelectedItem.GetType() == typeof(Genre))
                {
                    genre = (Genre)SelectedItem;
                    id = Genre.genres.IndexOf(genre);
                    id = Genre.genres[id].IDDatabase;

                    if (id != 0)
                    {
                        Genre.EditGenre(genre);
                    }
                    else
                    {
                        Genre.AddGenre(genre);
                    }
                }
                else if (SelectedItem.GetType() == typeof(Festival))
                {
                    festival = (Festival)SelectedItem;
                    id = Festival.festivals.IndexOf(festival);
                    id = Festival.festivals[id].IDDatabase;

                    if (id != 0)
                    {
                        Festival.EditFestival(festival);
                    }
                    else
                    {
                        Festival.AddFestival(festival);
                    }
                }
            }
            else
            {
                DialogResult result = MessageQuestion();
                if (result == DialogResult.Yes)
                {
                    foreach (Stage s in Stage.stages)
                    {
                        Stage.EditStage(s);
                    }
                    foreach (Genre g in Genre.genres)
                    {
                        Genre.EditGenre(g);
                    }
                    foreach (Festival f in Festival.festivals)
                    {
                        Festival.EditFestival(f);
                    }
                }
            }
        }

        private void VerkoopSaveItem()
        {
            TicketType ticketType = (TicketType)SelectedItem;
            if (ticketType != null)
            {
                int id = TicketType.ticketTypes.IndexOf(ticketType);
                id = TicketType.ticketTypes[id].IDDatabase;

                if (id != 0)
                {
                    TicketType.EditType(ticketType);
                }
                else
                {
                    TicketType.AddType(ticketType);
                }
            }
            else
            {
                DialogResult result = MessageQuestion();
                if (result == DialogResult.Yes)
                {
                    foreach (TicketType c in TicketType.ticketTypes)
                    {
                        TicketType.EditType(c);
                    }
                }
            }
        }

        private void TicketsSaveItem()
        {
            Ticket ticket = (Ticket)SelectedItem;
            if (ticket != null)
            {
                int id = 0;
                for (int i = 0; i < Ticket.tickets.Count(); i++)
                {
                    if (Ticket.tickets[i].TicketHolder == ticket.TicketHolder && Ticket.tickets[i].TicketHolderEmail == ticket.TicketHolderEmail)
                    {
                        id = i;
                    }
                }
                id = Ticket.tickets[id].IDDatabase;

                if (id != 0)
                {
                    Ticket.EditTicket(ticket);
                }
                else
                {
                    Ticket.AddTicket(ticket);
                }
            }
            else
            {
                DialogResult result = MessageQuestion();
                if (result == DialogResult.Yes)
                {
                    foreach (Ticket c in Ticket.tickets)
                    {
                        Ticket.EditTicket(c);
                    }
                }
            }
        }

        private void PersoneelSaveItem()
        {
            int id = 0;
            ContactPersonType contactPersonType = null;
            ContactPersonTitle contactPersonTitle = null;

            if (SelectedItem != null)
            {
                if (SelectedItem.GetType() == typeof(ContactPersonType))
                {
                    contactPersonType = (ContactPersonType)SelectedItem;
                    id = ContactPersonType.contactTypes.IndexOf(contactPersonType);
                    id = ContactPersonType.contactTypes[id].IDDatabase;

                    if (id != 0)
                    {
                        ContactPersonType.EditType(contactPersonType);
                    }
                    else
                    {
                        ContactPersonType.AddType(contactPersonType);
                    }
                }
                else if (SelectedItem.GetType() == typeof(ContactPersonTitle))
                {
                    contactPersonTitle = (ContactPersonTitle)SelectedItem;
                    id = ContactPersonTitle.contactTitles.IndexOf(contactPersonTitle);
                    id = ContactPersonTitle.contactTitles[id].IDDatabase;

                    if (id != 0)
                    {
                        ContactPersonTitle.EditTitle(contactPersonTitle);
                    }
                    else
                    {
                        ContactPersonTitle.AddTitle(contactPersonTitle);
                    }
                }
            }
            else
            {
                DialogResult result = MessageQuestion();
                if (result == DialogResult.Yes)
                {
                    foreach (ContactPersonTitle cpTitle in ContactPersonTitle.contactTitles)
                    {
                        ContactPersonTitle.EditTitle(cpTitle);
                    }
                    foreach (ContactPersonType cpType in ContactPersonType.contactTypes)
                    {
                        ContactPersonType.EditType(cpType);
                    }
                }
            }
        }

        private void ContactSaveItem()
        {
            ContactPerson contactPersoon = (ContactPerson)SelectedItem;
            if (contactPersoon != null)
            {
                int id = ContactPerson.contactPersons.IndexOf(contactPersoon);
                id = ContactPerson.contactPersons[id].IDDatabase;

                if (id != 0)
                {
                    ContactPerson.EditContact(contactPersoon);
                }
                else
                {
                    ContactPerson.AddContact(contactPersoon);
                }
            }
            else
            {
                DialogResult result = MessageQuestion();
                if (result == DialogResult.Yes)
                {
                    foreach (ContactPerson cp in ContactPerson.contactPersons)
                    {
                        ContactPerson.EditContact(cp);
                    }
                }
            }
        }

        public void DeleteItem()
        {
            if (SelectedItem != null)
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
                    genrestageDeleteItem();
                }
                if (CurrentPage.Name == "Bands")
                {
                    BandDeleteItem();
                }
                if (CurrentPage.Name == "Info Bands")
                {
                    GenreDeleteItem();
                }
                if (CurrentPage.Name == "Line-Up")
                {
                    LineUpDeleteItem();
                }
            }
            else
            {
                MessageDelete();
            }
        }

        private void LineUpDeleteItem()
        {
            LineUp lineUp = (LineUp)SelectedItem;
            LineUp.DeleteLineUp(lineUp);
            LineUp.lineUp.Remove(lineUp);
        }

        private void GenreDeleteItem()
        {
            Band band = (Band)SelectedItem;
            band.GenreListBand.Remove(BandGenre.GenreBand);
            CurrentPage = new LineUpInfoVM();
        }

        private void BandDeleteItem()
        {
            Band band = (Band)SelectedItem;
            Band.DeleteBand(band);
            Band.bands.Remove(band);
        }

        private void genrestageDeleteItem()
        {
            if (SelectedItem.GetType() == typeof(Stage))
            {
                Stage stage = (Stage)SelectedItem;
                Stage.DeleteStage(stage);
                LineUp.StageList.Remove(stage);
            }
            else if (SelectedItem.GetType() == typeof(Genre))
            {
                Genre genre = (Genre)SelectedItem;
                Genre.DeleteGenre(genre);
                Band.GenreList.Remove(genre);
            }
            else if (SelectedItem.GetType() == typeof(Festival))
            {
                Festival festival = (Festival)SelectedItem;
                Festival.DeleteFestival(festival);
                Festival.festivals.Remove(festival);
            }
        }

        private void VerkoopDeleteItem()
        {
            TicketType ticket = (TicketType)SelectedItem;
            TicketType.DeleteType(ticket);
            Ticket.TicketTypeList.Remove(ticket);
        }

        private void TicketsDeleteItem()
        {
            Ticket ticket = (Ticket)SelectedItem;
            Ticket.DeleteTicket(ticket);
            Ticket.tickets.Remove(ticket);
            CurrentPage = new TicketOverviewVM();
        }

        private void PersoneelDeleteItem()
        {
            if (SelectedItem.GetType() == typeof(ContactPersonType))
            {
                ContactPersonType contactPersonType = (ContactPersonType)SelectedItem;
                ContactPersonType.DeleteType(contactPersonType);
                ContactPerson.JobRoleList.Remove(contactPersonType);
            }
            else if (SelectedItem.GetType() == typeof(ContactPersonTitle))
            {
                ContactPersonTitle contactPersonTitle = (ContactPersonTitle)SelectedItem;
                ContactPersonTitle.DeleteTitle(contactPersonTitle);
                ContactPerson.JobTitleList.Remove(contactPersonTitle);
            }
        }

        private void ContactDeleteItem()
        {
            ContactPerson contactPersoon = (ContactPerson)SelectedItem;
            ContactPerson.DeleteContact(contactPersoon);
            ContactPerson.contactPersons.Remove(contactPersoon);
        }
    }
}
