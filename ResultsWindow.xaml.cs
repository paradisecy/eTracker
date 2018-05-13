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
using System.Windows.Shapes;


namespace eTracker
{
    /// <summary>
    /// Interaction logic for ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        public class AllList
        {
            public Record Record { get; set; }
            public List<DetailRecord> DetailRecords { get; set; }
        }

        private Learn _learner; 
        private List<string> Queries = new List<string>();
        public ResultsWindow()
        {
            InitializeComponent();
            using (var db = new DatabaseEntities())
            {
                var records = db.Records.OrderByDescending(o => o.Id).ToList();
                var allList = new List<AllList>();
                records.ForEach(f =>
                {
                    allList.Add(new AllList(){Record = f,DetailRecords = f.DetailRecords.ToList()});
                });
                listAll.ItemsSource = allList;
            }
            _learner = new Learn();
            //LearnedTree.ItemsSource = _learner.Tree;
            txtCode.Text = _learner.Code;
            txtRules.Text = _learner.Rules;
        }

        private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch (e.Column.Header.ToString())
            {
                case "PDFFile":
                    e.Cancel = true;
                    break;
                case "RecordId":
                    e.Cancel = true;
                    break;
                case "Id":
                    e.Cancel = true;
                    break;
                case "Record":
                    e.Cancel = true;
                    break;
            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var rQ = txtQuery.Text;
            var q = txtQuery.Text.Split(',');
            var ans = _learner.Query(q);
            Queries.Add($"{rQ} => {ans}");
            qList.ItemsSource = null;
            qList.ItemsSource = Queries;
        }
    }
}
