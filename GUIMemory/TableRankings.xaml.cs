using BusinessMemory;
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
using System.Windows.Forms;
using BusinessMemory;
using MemoryDataAccess;

namespace GUIMemory
{
    /// <summary>
    /// Interaction logic for TableRankings.xaml
    /// </summary>
    public partial class TableRankings : Window
    {
        private MemoryRecordRepository repository = new MemoryRecordRepository();
        private MemoryRecord? MemoryRecord { get; set; } = null;
        private int Cards { get; set; }
        private int Seconds { get; set; }
        private int Attempts { get; set; }
        private DateTime DateTime { get; set; }
        private string PlayerName { get; set; } = null;

        public TableRankings(int amountOfCards, int seconds, int attempts, DateTime dateTime)
        {
            Cards = amountOfCards;
            Seconds = seconds;
            Attempts = attempts;
            DateTime = dateTime;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text;
            name = name.Trim();
            if (name.Length <= 0) {
                System.Windows.MessageBox.Show("Please enter a name");
            }
            PlayerName = name;
            MemoryRecord = new MemoryRecord(Cards, Seconds, Attempts, PlayerName);
            MemoryRecord = repository.CreateReadOrUpdate(MemoryRecord);
            label.Visibility = Visibility.Hidden;
            button.Visibility = Visibility.Hidden;
            NameBox.Visibility = Visibility.Hidden;
            CreateTable();
        }

        private void CreateTable()
        {
            string query = "SELECT PlayerName as Name, ROUND(((AmountOfCards*AmountOfCards) / (Seconds*Attempts)) *1000,2) AS Score, AmountOfCards as Cards,  Seconds, Attempts, DateTime AS Date FROM MemoryRecords ORDER by Score DESC";
            Dictionary<string, List<object>> tablelist = repository.ReadAllWithColumns(query);
            //add place
            tablelist["Place"] = SetPlaces(tablelist);
            tablelist = SortTablePosition(new List<string> {"Place"}, tablelist);

            // Maak een FlowDocument aan
            FlowDocument flowDocument = new FlowDocument();
            // Creeer de Table
            Table table = new Table();
            table.CellSpacing = 5;

            // Definieer de kolommen
            for (int i = 0; i < 4; i++)
            {
                table.Columns.Add(new TableColumn());
            }

            // Maak de TableRowGroup aan
            TableRowGroup tableRowGroup = new TableRowGroup();

            // Voeg de rijen toe aan de TableRowGroup
            // Voeg de titelrij toe
            TableRow titleRow = new TableRow();
            titleRow.Background = Brushes.SkyBlue;
            titleRow.Cells.Add(new TableCell(new Paragraph(new Run("Player Rankings")) { FontSize = 24, FontWeight = FontWeights.Bold, }) { ColumnSpan = 4, TextAlignment = TextAlignment.Center });
            tableRowGroup.Rows.Add(titleRow);

            // Voeg de headerrij toe
            TableRow headerRow = new TableRow();
            headerRow.Background = Brushes.LightGoldenrodYellow;
            foreach(string key in tablelist.Keys)
            {
               headerRow.Cells.Add(new TableCell(new Paragraph(new Run(key)) { FontSize = 14, FontWeight = FontWeights.Bold }));
            }
            tableRowGroup.Rows.Add(headerRow);

            for (int i = 0; i < tablelist[tablelist.Keys.First()].Count; i++)
            {
                TableRow itemRow = new TableRow();
                itemRow.Background = Brushes.White;
                foreach(string key in tablelist.Keys)
                {
                    itemRow.Cells.Add(new TableCell(new Paragraph(new Run(tablelist[key][i].ToString())) { FontSize = 12, FontWeight = FontWeights.Black }));
                    
                }
                tableRowGroup.Rows.Add(itemRow);
            }

            // Voeg de TableRowGroup toe aan de Table
            table.RowGroups.Add(tableRowGroup);

            // Voeg de Table toe aan het FlowDocument
            flowDocument.Blocks.Add(table);

            // Maak een FlowDocumentScrollViewer aan en voeg het FlowDocument toe
            FlowDocumentScrollViewer flowDocumentScrollViewer = new FlowDocumentScrollViewer();
            flowDocumentScrollViewer.Document = flowDocument;

            // Voeg de FlowDocumentScrollViewer toe aan de Grid genaamd "TableGrid"
            TableGrid.Children.Add(flowDocumentScrollViewer);

            NewGame.Visibility = Visibility.Visible;
            ViewImagesBtn.Visibility = Visibility.Visible;
        }

        private static Dictionary<string, List<object>> SortTablePosition(List<string> list, Dictionary<string, List<object>> rawTableOld)
        {
            Dictionary<string, List<object>> newTable = new Dictionary<string, List<object>>();
            foreach (string key in list)
            {
                newTable.Add(key, rawTableOld[key]);
            }

            foreach (string key in rawTableOld.Keys)
            {
                if (!newTable.ContainsKey(key))
                {
                    newTable.Add(key, rawTableOld[key]);
                }
            }
            return newTable;
        }

        private List<object> SetPlaces(Dictionary<string, List<object>> table)
        {
            List<object> placings = new List<object>();
            for (int i = 1; i <= table["Score"].Count; i++)
            {
                placings.Add('#' + i.ToString());
            }
            return placings;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            ChooseCardsAmount menu = new ChooseCardsAmount();
            menu.Show();
            this.Close();
        }

        private void ViewImagesBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewImages viewImages = new ViewImages();
            viewImages.Show();
            this.Close();
        }
    }
}
