using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using MoonPdfLib.MuPdf;
using Newtonsoft.Json;
using RestSharp;
using System.Windows.Forms;
using System.Windows.Media;
using MoonPdfLib;


namespace eTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int _currentId;
        //read from app.config
        private readonly string _key;
        private System.Timers.Timer _timer;
        private DateTime _currentTime;
        private DateTime _startTime;

        private string _currentTimeSpan;
        private byte[] _fileBytes;
        private MoonPdfPanel _moonPanel;
        private List<string> _currentSynoList;
        private DetailRecord _currentDetailRecord;

        public string CurrentTimeSpan
        {
            get => _currentTimeSpan;
            set
            {
                if (_currentTimeSpan == value)
                    return;
                _currentTimeSpan = value;
                OnPropertyChanged(nameof(CurrentTimeSpan));
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _key = ConfigurationManager.AppSettings["apiKey"];
            this.DataContext = this;
            CurrentTimeSpan = "0:00:00";
            CreatePdfVwr();
        }

        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            var modal = new ModalWindow();
            modal.Owner = this;
            modal.ShowDialog();

            if (modal.Started)
            {
                var l1 = modal.L1Combo.SelectedValue;
                var age = modal.AgeCombo.SelectedValue;

                var newRecord = new Record
                {
                    Age = (int)age,
                    L1 = (string)l1,
                    StartDate = DateTime.Now,
                    PDFFile = _fileBytes
                };

                using (var db = new DatabaseEntities())
                {
                    db.Records.Add(newRecord);
                    db.SaveChanges();
                    _currentId = newRecord.Id;
                }

                translateTxt.Text = string.Empty;
                btnTranslate.IsEnabled = true;
                translateTxt.IsEnabled = true;
                StartBtn.IsEnabled = false;
                LoadFile.IsEnabled = false;
                StopBtn.IsEnabled = true;
                Start();

            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DatabaseEntities())
            {
                var mRecord = db.Records.First(f => f.Id == _currentId);
                mRecord.EndDate = DateTime.Now;
                mRecord.Duration = CurrentTimeSpan;
                db.SaveChanges();
            }

            translateTxt.Text = string.Empty;
            btnTranslate.IsEnabled = false;
            translateTxt.IsEnabled = false;
            StartBtn.IsEnabled = false;
            LoadFile.IsEnabled = true;
            StopBtn.IsEnabled = false;
            translatedTxtBlock.Text = string.Empty;
            synoLst.ItemsSource = null;
            Stop();
            LoadFile.IsEnabled = true;
            mainGrid.Children.Remove(_moonPanel);
            CreatePdfVwr();
            CurrentTimeSpan = "0:00:00";
            var resultWindow = new ResultsWindow {Owner = this};
            resultWindow.ShowDialog();

        }

        private void btnTranslate_Click(object sender, RoutedEventArgs e)
        {
            Translate();
        }

        private void CreatePdfVwr()
        {
            _moonPanel = new MoonPdfPanel
            {
                ViewType = ViewType.SinglePage,
                Background = Brushes.LightGray
            };
            Grid.SetColumn(_moonPanel, 0);
            Grid.SetRow(_moonPanel, 1);
            mainGrid.Children.Add(_moonPanel);
        }

        private void Translate()
        {
            using (var db = new DatabaseEntities())
            {
                var record = db.Records.First(f => f.Id == _currentId);
                Translator t = new Translator();
                try
                {
                    var txtToTranslate = this.translateTxt.Text.Trim();
                    var trnsTxt = t.Translate(txtToTranslate, "English", record.L1).Trim();
                    if (!string.IsNullOrEmpty(trnsTxt))
                    {
                        this.translatedTxtBlock.Text = $"{txtToTranslate} => {trnsTxt}";
                        var client = new RestClient("http://thesaurus.altervista.org");
                        var request = new RestRequest("thesaurus/v1", Method.GET);
                        request.AddParameter("key", _key);
                        request.AddParameter("word", txtToTranslate);
                        request.AddParameter("language", "en_US");
                        request.AddParameter("output", "json");
                        IRestResponse response = client.Execute(request);
                        var content = response.Content;
                        var objects = JsonConvert.DeserializeObject<Rootobject>(content);

                        var synonyms = objects
                            ?.response
                            ?.FirstOrDefault()
                            ?.list
                            .synonyms
                            .Split('|')
                            .ToList();

                        var synoList = new List<string>();
                        synonyms?.ForEach(f =>
                        {
                            var i = f.IndexOf('(');
                            if (i > 0)
                                synoList.Add(f.Substring(0, i).Trim());
                        });

                        if (!synoList.Any())
                            synoList.Add("Synonyms not found");

                        _currentSynoList = synoList;
                        this.synoLst.ItemsSource = synoList;

                        _currentDetailRecord = new DetailRecord()
                        {
                            CurrentPageNo = _moonPanel.GetCurrentPageNumber(),
                            SuggestedSynonisms = string.Join("|", synoList),
                            UnknownWord = txtToTranslate,
                            TranslatedWords = trnsTxt,
                            TimeStamp = DateTime.Now,
                            RecordId = record.Id,
                            SelectedSynonism = "Synonym not selected"

                        };

                        db.DetailRecords.Add(_currentDetailRecord);
                        db.SaveChanges();
                        translateTxt.Text = string.Empty;
                        synoLst.SelectedItem = null;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
            }
        }

        private void Start()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Elapsed;
            _timer.Enabled = true; // Enable it
            _startTime = DateTime.Now;
            _timer.Start();
        }

        private void Stop()
        {
            _timer.Stop();
        }

        private void Elapsed(object sender, ElapsedEventArgs e)
        {
            _currentTime = e.SignalTime;
            var duration = (_currentTime - _startTime);
            CurrentTimeSpan = duration.ToString("g").Split(',').First();
        }


        public class Rootobject
        {
            public Response[] response { get; set; }
        }

        public class Response
        {
            public List list { get; set; }
        }

        public class List
        {
            public string category { get; set; }
            public string synonyms { get; set; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog
            {
                Title = "Open PDF file",
                Filter = "PDF Files (*.pdf)|",
                RestoreDirectory = true,

            };

            if (fdlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                var filePath = fdlg.FileName;
                _fileBytes = File.ReadAllBytes(filePath);
                var source = new MemorySource(_fileBytes);
                _moonPanel.Open(source);
                StartBtn.IsEnabled = true;
            }
        }

        private void SynoLst_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                var selected = (string)e.AddedItems[0];
                //if (selected != "Synonyms not found")
                // {
                    using (var db = new DatabaseEntities())
                    {
                        var cDr = db.DetailRecords.First(f => f.Id == _currentDetailRecord.Id);
                        cDr.SelectedSynonism = selected;
                        _currentDetailRecord.SelectedSynonism = selected;
                        db.SaveChanges();

                    }
                //}
            }
        }
    }
}
