using System;
using System.Collections.Generic;
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
using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Firebase_WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Readings> readings = new List<Readings>();
        public MainWindow()
        {
            InitializeComponent();
            fetchAndPlot();
        }

        public async void fetchAndPlot()
        {
            await querying();
            plot1.Model = CreatePlotModel();
            label1.Content ="Last update: " + DateTime.Now;
        }

        private async Task querying()
        {
            var firebase = new FirebaseClient("https://rpi-dht-fb.firebaseio.com/");
            var buffer = await firebase
              .Child("sensors")
              .Child("dht")
              .OrderByKey()
              .LimitToFirst(100)
              .OnceAsync<Readings>();

            foreach (var r in buffer)
            {
                var h = r.Object.humidity;
                var t = r.Object.temperature;
                var ts = r.Object.timestamp;
                readings.Add(new Readings { humidity = h, temperature = t, timestamp = ts });
                Console.WriteLine($"{h.ToString("0.00")} ,{t.ToString("0.00")}, {ts}");
            }
        }

        public PlotModel CreatePlotModel()
        {
            var plotModel = new PlotModel { Title = "Temperature / Humidity Chart" };


            var series1 = new LineSeries
            {
                Title = "Temperature (°C)",
                Color = OxyColor.FromRgb(255, 0, 0),
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.White,
                

            };

            var series2 = new LineSeries
            {
                Title = "Humidity (%)",
                Color = OxyColor.FromRgb(0, 0, 255),
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.White
            };

            for (int i = 0; i < readings.Count; i++)
            {
                series1.Points.Add(new DataPoint(i, readings[i].temperature));
                series2.Points.Add(new DataPoint(i, readings[i].humidity));
                var pointAnnotation = new PointAnnotation();
                pointAnnotation.X = Convert.ToDouble(i);
                pointAnnotation.Y = Convert.ToDouble(readings[i].humidity);
                pointAnnotation.Text = readings[i].timestamp;
                //plotModel.Annotations.Add(pointAnnotation);
            }

            for (int i = 0; i < series1.Points.Count; i++)
            {
                var points = series1.Points[i];

            }

            plotModel.Series.Add(series1);
            plotModel.Series.Add(series2);

            plotModel.LegendPlacement = LegendPlacement.Outside;
            plotModel.LegendPosition = LegendPosition.BottomCenter;
            plotModel.LegendOrientation = LegendOrientation.Horizontal;
            plotModel.LegendBorderThickness = 1;

            var axisX = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };
            var axisY = new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum =0,
                Maximum = 100,
                MajorGridlineStyle = LineStyle.Dot,
                MajorGridlineColor = OxyColors.Gray,
                MajorStep = 10,
                MinorStep = 10,
                IsZoomEnabled = false,
                IsPanEnabled = false
            };

            plotModel.Axes.Add(axisX);
            plotModel.Axes.Add(axisY);

            return plotModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            fetchAndPlot();
        }
    }




    internal class Readings
    {
        public long humidity { get; set; }
        public long temperature { get; set; }
        public string timestamp { get; set; }
    }
}
