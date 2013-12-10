using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TimeLineTool;

namespace ProjectFestival.view
{
    /// <summary>
    /// Interaction logic for LineUpOverview.xaml
    /// </summary>
    public partial class LineUpOverview : UserControl
    {
        ObservableCollection<ITimeLineDataItem> data = new ObservableCollection<ITimeLineDataItem>();
        public ObservableCollection<ITimeLineDataItem> t2Data = new ObservableCollection<ITimeLineDataItem>();
        public ObservableCollection<ITimeLineDataItem> t3Data = new ObservableCollection<ITimeLineDataItem>();
        ObservableCollection<ITimeLineDataItem> listboxData = new ObservableCollection<ITimeLineDataItem>();

        public LineUpOverview()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.

            var tmp1 = new TempDataType()
            {
                StartTime = DateTime.Now.AddHours(3),
                EndTime = DateTime.Now.AddHours(18),
                Name = "Temp 1"
            };
            var tmp2 = new TempDataType()
            {
                StartTime = DateTime.Now.AddHours(18),
                EndTime = DateTime.Now.AddHours(33),
                Name = "Temp 2"
            };
            var temp3 = new TempDataType()
            {
                StartTime = DateTime.Now.AddHours(44),
                EndTime = DateTime.Now.AddHours(60),
                Name = "Temp 3"
            };
            var temp4 = new TempDataType()
            {
                StartTime = DateTime.Now.AddHours(60),
                EndTime = DateTime.Now.AddHours(70),
                Name = "Temp 4"
            };

            data.Add(tmp1);
            data.Add(tmp2);
            data.Add(temp3);
            data.Add(temp4);

            t2Data.Add(tmp1);
            t3Data.Add(temp3);

            //TimeLine2.Items = data;
            TimeLine2.StartDate = DateTime.Now;

            TimeLine3.StartDate = DateTime.Now;
            TimeLine2.Items = t2Data;
            TimeLine3.Items = t3Data;

            var lb1 = new TempDataType()
            {
                Name = "ListBox 1"
            };
            var lb2 = new TempDataType()
            {
                Name = "ListBox 2"
            };
            var lb3 = new TempDataType()
            {
                Name = "ListBox 3"
            };
            var lb4 = new TempDataType()
            {
                Name = "ListBox 4"
            };
            listboxData.Add(lb1);
            listboxData.Add(lb2);
            listboxData.Add(lb3);
            listboxData.Add(lb4);
            ListSrc.ItemsSource = listboxData;
        }

        private void Slider_Scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TimeLine3.UnitSize = Slider_Scale.Value;
            TimeLine2.UnitSize = Slider_Scale.Value;
        }
    }
}
