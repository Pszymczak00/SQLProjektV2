using SQLProjektV2.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace SQLProjektV2.Views
{
    /// <summary>
    /// Interaction logic for ProjektyView.xaml
    /// </summary>
    public partial class ProjektyView : UserControl
    {
        string selectedId;
        public ProjektyView()
        {
            InitializeComponent();
            DataContext = new ProjektyViewModel();
            DatePicker1.SelectedDate = DateTime.Today;
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            int index = MainTable.SelectedIndex;

            TextBlock x = null;
            try
            {
                x = MainTable.Columns[0].GetCellContent(MainTable.Items[index]) as TextBlock;
            }
            catch (Exception)
            {
            }
            if (x != null)
            {
                selectedId = x.Text;
                AddForm.Visibility = Visibility.Collapsed;
                Filters.Visibility = Visibility.Collapsed;
                ModForm.Visibility = Visibility.Visible;

                DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdProjekty]", int.Parse(selectedId));

                MDatePicker1.SelectedDate = ((DateTime)temp.Rows[0][0]);
                MOpisSource.Text = temp.Rows[0][1].ToString();
                MKSource.SelectedValue = temp.Rows[0][2];
                MZSource.SelectedValue = temp.Rows[0][3];
            }
        }

        private void MainTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Id")
                (e.Column as DataGridTextColumn).MaxWidth = 0;
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }



        private void AddFormVisible(object sender, RoutedEventArgs e)
        {
            AddForm.Visibility = Visibility.Visible;
            ModForm.Visibility = Visibility.Collapsed;
            Filters.Visibility = Visibility.Collapsed;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            AddForm.Visibility = Visibility.Collapsed;
            ModForm.Visibility = Visibility.Collapsed;
            Filters.Visibility = Visibility.Visible;
        }


        private void AddNewRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (!DatePicker1.SelectedDate.HasValue) errorString += "Podaj datę zakończenia projektu\n";
            if ((errorString.Length == 0) && DBConnection.SQLCommandRet($"select count(*) from [dbo].[Projekty] WHERE Opis = '{OpisSource.Text}' AND Klienci_Id = '{((KeyValuePair<int, string>)KSource.SelectedItem).Key}'") > 0) errorString += "Już jest taki projekt\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string terminOddania = DatePicker1.SelectedDate.Value.ToString("yyyy-MM-dd");
                string opis = OpisSource.Text;
                string klient = ((KeyValuePair<int, string>)KSource.SelectedItem).Key.ToString();
                string zespol = ((KeyValuePair<int, string>)ZSource.SelectedItem).Key.ToString();

                string temp = $"INSERT INTO [dbo].[Projekty] VALUES ('{terminOddania}', '{opis}', {klient}, {zespol})";
                MessageBox.Show("Dodano nowy projekt");
                DBConnection.SQLCommand(temp);
                DataContext = new ProjektyViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {

            string errorString = "";

            if (!MDatePicker1.SelectedDate.HasValue) errorString += "Podaj datę zakończenia projektu\n";
            if ((errorString.Length == 0) && DBConnection.SQLCommandRet($"select count(*) from [dbo].[Projekty] WHERE Opis = '{MOpisSource.Text}' AND Klienci_Id = '{((KeyValuePair<int, string>)MKSource.SelectedItem).Key}' AND Id != {selectedId}") > 0) errorString += "Już jest taki projekt\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string terminOddania = MDatePicker1.SelectedDate.Value.ToString("yyyy-MM-dd");
                string opis = MOpisSource.Text;
                string klient = ((KeyValuePair<int, string>)MKSource.SelectedItem).Key.ToString();
                string zespol = ((KeyValuePair<int, string>)MZSource.SelectedItem).Key.ToString();

                string temp = $"UPDATE [dbo].[Projekty] SET [Termin_oddania] = '{terminOddania}', [Opis] = '{opis}', [Klienci_Id] = {klient}, [ZespoLy_id] = {zespol} WHERE Id = {selectedId}";
                MessageBox.Show("Zmieniono dane o projekcie");
                DBConnection.SQLCommand(temp);
                DataContext = new ProjektyViewModel();
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                string temp = $"DELETE FROM [dbo].[Projekty] WHERE Id = {selectedId}";
                try
                {
                    DBConnection.SQLCommand(temp);
                    MessageBox.Show("Usunięto projekt");
                    DataContext = new ProjektyViewModel();
                    ModForm.Visibility = Visibility.Collapsed;
                    Filters.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    MessageBox.Show("Nie usunięto projektu, ponieważ jest związany z innymi tablicami");
                }
            }
        }


        private void AddFilter(object sender, RoutedEventArgs e)
        {
            StackPanel temp = Globals.StackPanel;

            ComboBox tempCB = Globals.ComboBoxColumnChoose;
            tempCB.ItemsSource = ((ProjektyViewModel)this.DataContext).FilterInfo;
            temp.Children.Add(tempCB);

            temp.Children.Add(Globals.ComboBox);

            temp.Children.Add(Globals.Button);

            (temp.Children[0] as ComboBox).SelectedIndex = 0;

            FiltersList.Children.Add(temp);
        }

        private void UseFilters(object sender, RoutedEventArgs e)
        {
            (DataContext as ProjektyViewModel).MainTable.RowFilter = Globals.GetFilter(FiltersList);
        }
    }
}
