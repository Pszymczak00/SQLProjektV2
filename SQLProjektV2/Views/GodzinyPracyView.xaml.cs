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
    /// Interaction logic for GodzinyPracyView.xaml
    /// </summary>
    public partial class GodzinyPracyView : UserControl
    {
        private string selectedId = "0";
        private string selectedColumnId = "1"; public GodzinyPracyView()
        {
            InitializeComponent();
            DataContext = new GodzinyPracyViewModel();
            DatePicker1.SelectedDate = DateTime.Today;
        }

        private void MainTable_MouseDoubleClick(object sender, MouseButtonEventArgs e)
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
                DataGridRow row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
                if (row != null)
                {
                    row.BorderBrush = null;
                    row.BorderThickness = new Thickness(0);
                }
                selectedId = x.Text;
                selectedColumnId = index.ToString();
                AddForm.Visibility = Visibility.Collapsed;
                ModForm.Visibility = Visibility.Visible;
                Filters.Visibility = Visibility.Collapsed;
                row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
                row.BorderBrush = Brushes.White;
                row.BorderThickness = new Thickness(2);

                DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdGodziny Pracy]", int.Parse(selectedId));

                MPSource.SelectedValue = temp.Rows[0][0];
                MDatePicker1.SelectedDate = ((DateTime)temp.Rows[0][1]);
                MGodzinySource.Text = temp.Rows[0][2].ToString();
            }
        }

        private void MainTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Id")
                (e.Column as DataGridTextColumn).MaxWidth = 0;
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "MM/yyyy";
        }



        private void AddFormVisible(object sender, RoutedEventArgs e)
        {
            AddForm.Visibility = Visibility.Visible;
            ModForm.Visibility = Visibility.Collapsed;
            Filters.Visibility = Visibility.Collapsed;
            DataGridRow row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
            if (row != null)
            {
                row.BorderBrush = null;
                row.BorderThickness = new Thickness(0);
            }

        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            AddForm.Visibility = Visibility.Collapsed;
            ModForm.Visibility = Visibility.Collapsed;
            Filters.Visibility = Visibility.Visible;
            DataGridRow row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
            if (row != null)
            {
                row.BorderBrush = null;
                row.BorderThickness = new Thickness(0);
            }

        }


        private void AddNewRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";
            if (!DatePicker1.SelectedDate.HasValue) errorString += "Podaj miesiąc\n";
            else
            {
                string year = DatePicker1.SelectedDate.Value.Year.ToString();
                string month = DatePicker1.SelectedDate.Value.Month.ToString();
                string date = $"{year}-{month}-01";
                if (DBConnection.SQLCommandRet($"select count(*) from [dbo].[Godziny_Pracy] WHERE Pracownicy_Id_Prac = '{((KeyValuePair<int, string>)PSource.SelectedItem).Key}' AND miesiąc = '{DatePicker1.SelectedDate.Value.ToString(date)}'") > 0) errorString += "Już jest taki wpis pracy\n";
            }
            if (GodzinySource.Text.Length == 0) errorString += "Podaj liczbę godzin\n";
            else if (!int.TryParse(GodzinySource.Text, out _)) errorString += "Liczba godzin musi być liczbą\n";
            else if (int.Parse(GodzinySource.Text) < 0) errorString += "Liczba godzin musi być większa lub równa 0\n";

            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string prac = ((KeyValuePair<int, string>)PSource.SelectedItem).Key.ToString();
                string godziny = GodzinySource.Text;
                string year = DatePicker1.SelectedDate.Value.Year.ToString();
                string month = DatePicker1.SelectedDate.Value.Month.ToString();
                string date = $"{year}-{month}-01";

                string temp = $"insert into [dbo].[Godziny_Pracy] values ({prac}, '{date}', {godziny})";
                MessageBox.Show("Dodano nowe wpis deklaracji godzin pracy");
                DBConnection.SQLCommand(temp);
                DataContext = new GodzinyPracyViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (!MDatePicker1.SelectedDate.HasValue) errorString += "Podaj miesiąc\n";
            else
            {
                string year = MDatePicker1.SelectedDate.Value.Year.ToString();
                string month = MDatePicker1.SelectedDate.Value.Month.ToString();
                string date = $"{year}-{month}-01";
                if (DBConnection.SQLCommandRet($"select count(*) from [dbo].[Godziny_Pracy] WHERE Pracownicy_Id_Prac = '{((KeyValuePair<int, string>)MPSource.SelectedItem).Key}' AND miesiąc = '{MDatePicker1.SelectedDate.Value.ToString(date)}' AND Id != {selectedId}") > 0) errorString += "Już jest taki wpis pracy\n";
            }

            if (MGodzinySource.Text.Length == 0) errorString += "Podaj liczbę godzin\n";
            else if (!int.TryParse(MGodzinySource.Text, out _)) errorString += "Liczba godzin musi być liczbą\n";
            else if (int.Parse(MGodzinySource.Text) < 0) errorString += "Liczba godzin musi być większa lub równa 0\n";

            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string prac = ((KeyValuePair<int, string>)MPSource.SelectedItem).Key.ToString();
                string year = MDatePicker1.SelectedDate.Value.Year.ToString();
                string month = MDatePicker1.SelectedDate.Value.Month.ToString();
                string date = $"{year}-{month}-01";
                string godziny = MGodzinySource.Text;

                string temp = $"UPDATE [dbo].[Godziny_Pracy] SET [Pracownicy_Id_prac] = {prac}, [Miesiąc] = '{date}', [Zadeklarowane_godziny] = {godziny} WHERE Id = {selectedId}";
                MessageBox.Show("Zaktualizowano wpis deklaracji godzin pracy");
                DBConnection.SQLCommand(temp);
                DataContext = new GodzinyPracyViewModel();
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "",  MessageBoxButton.YesNo);
        
            if(result == MessageBoxResult.Yes)
            {
                string temp = $"DELETE FROM [dbo].[Godziny_pracy] WHERE Id = {selectedId}";
                try
                {
                    DBConnection.SQLCommand(temp);
                    MessageBox.Show("Usunięto wpis godzin pracy");
                    DataContext = new GodzinyPracyViewModel();
                    ModForm.Visibility = Visibility.Collapsed;
                    Filters.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    MessageBox.Show("Nie usunięto wpisu godzin pracy, ponieważ jest związany z innymi tablicami");
                }
            }

        }



        private void AddFilter(object sender, RoutedEventArgs e)
        {
            StackPanel temp = Globals.StackPanel;

            ComboBox tempCB = Globals.ComboBoxColumnChoose;
            tempCB.ItemsSource = ((GodzinyPracyViewModel)this.DataContext).FilterInfo;
            temp.Children.Add(tempCB);

            temp.Children.Add(Globals.ComboBox);

            temp.Children.Add(Globals.Button);

            (temp.Children[0] as ComboBox).SelectedIndex = 0;

            FiltersList.Children.Add(temp);
        }

        private void UseFilters(object sender, RoutedEventArgs e)
        {
            (DataContext as GodzinyPracyViewModel).MainTable.RowFilter = Globals.GetFilter(FiltersList);
        }

    }
}
