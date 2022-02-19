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
    /// Interaction logic for ZadaniaView.xaml
    /// </summary>
    public partial class ZadaniaView : UserControl
    {
        private string selectedId = "0";
        private string selectedColumnId = "1";
        public ZadaniaView()
        {
            InitializeComponent();
            DataContext = new ZadaniaViewModel();
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
                DataGridRow row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
                if (row != null)
                {
                    row.BorderBrush = null;
                    row.BorderThickness = new Thickness(0);
                }

                selectedId = x.Text;
                selectedColumnId = index.ToString();
                AddForm.Visibility = Visibility.Collapsed;
                Filters.Visibility = Visibility.Collapsed;
                ModForm.Visibility = Visibility.Visible;
                row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
                row.BorderBrush = Brushes.White;
                row.BorderThickness = new Thickness(2);
                DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdZadania]", int.Parse(selectedId));

                MNazwaSource.Text = temp.Rows[0][0].ToString();
                MUkonczoneSource.IsChecked = (bool)temp.Rows[0][1];
                MCzasSource.Text = temp.Rows[0][2].ToString();
                MOpisSource.Text = temp.Rows[0][3].ToString();
                MPSource.SelectedValue = temp.Rows[0][4];
                MPROJSource.SelectedValue = temp.Rows[0][5];
            }
        }

        private void MainTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Id")
                (e.Column as DataGridTextColumn).MaxWidth = 0;
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

            if(NazwaSource.Text.Length == 0) errorString += "Podaj nazwę \n";
            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Zadania] WHERE [Nazwa] = '{NazwaSource.Text}' AND [Pracownicy_Id_prac] = {((KeyValuePair<int, string>)PSource.SelectedItem).Key} AND [Projekty_Id] = {((KeyValuePair<int, string>)PROJSource.SelectedItem).Key}") > 0) errorString += "Jest już takie zadanie\n";
          
            if (CzasSource.Text.Length == 0) errorString += "Podaj przeznaczony czas\n";
            else if (!int.TryParse(CzasSource.Text, out _)) errorString += "Przeznaczony czas musi być liczbą całkowitą\n";
            else if (int.Parse(CzasSource.Text) < 0) errorString += "Przeznaczony czas musi być większy lub równy 0\n";

            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string nazwa = NazwaSource.Text;
                string czy;
                if (UkonczoneSource.IsChecked.Value) czy = "1";
                else czy = "0";
                string czas = CzasSource.Text;
                string opis = OpisSource.Text;
                string pracownik = ((KeyValuePair<int, string>)PSource.SelectedItem).Key.ToString();
                string projekt = ((KeyValuePair<int, string>)PROJSource.SelectedItem).Key.ToString();

                string temp = $"INSERT INTO [dbo].[Zadania] VALUES ('{nazwa}', {czy}, {czas}, '{opis}', {pracownik}, {projekt})";
                MessageBox.Show("Dodano nowe zadanie");
                DBConnection.SQLCommand(temp);
                DataContext = new ZadaniaViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (MNazwaSource.Text.Length == 0) errorString += "Podaj nazwę \n";
            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Zadania] WHERE [Nazwa] = '{MNazwaSource.Text}' AND [Pracownicy_Id_prac] = {((KeyValuePair<int, string>)MPSource.SelectedItem).Key} AND [Projekty_Id] = {((KeyValuePair<int, string>)MPROJSource.SelectedItem).Key} AND Id != {selectedId}") > 0) errorString += "Jest już takie zadanie\n";

            if (MCzasSource.Text.Length == 0) errorString += "Podaj przeznaczony czas\n";
            else if (!int.TryParse(MCzasSource.Text, out _)) errorString += "Przeznaczony czas musi być liczbą całkowitą\n";
            else if (int.Parse(MCzasSource.Text) < 0) errorString += "Przeznaczony czas musi być większy lub równy 0\n";

            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string nazwa = MNazwaSource.Text;
                string czy;
                if (MUkonczoneSource.IsChecked.Value) czy = "1";
                else czy = "0";
                string czas = MCzasSource.Text;
                string opis = MOpisSource.Text;
                string pracownik = ((KeyValuePair<int, string>)MPSource.SelectedItem).Key.ToString();
                string projekt = ((KeyValuePair<int, string>)MPROJSource.SelectedItem).Key.ToString();

                string temp = $"UPDATE [dbo].[Zadania] SET [Nazwa] = '{nazwa}', [Czy_zakończone] = {czy}, [Przeznaczony_czas] = {czas}, [Opis] = '{opis}', [Pracownicy_Id_prac] = {pracownik}, [Projekty_Id] = {projekt} WHERE Id = {selectedId}";
                MessageBox.Show("Zmieniono informacje o zadaniu");
                DBConnection.SQLCommand(temp);
                DataContext = new ZadaniaViewModel();
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                int test = DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Wpisy_pracy] WHERE [Zadania_id] = {selectedId}");
                if (test > 0)
                    MessageBox.Show($"Nie można usunąc tego zadania, ponieważ ma przypisane {test} wpisów pracy.");

                else
                {
                    string temp = $"DELETE FROM [dbo].[Zadania] WHERE Id = {selectedId}";
                    try
                    {
                        DBConnection.SQLCommand(temp);
                        MessageBox.Show("Usunięto zadanie");
                        DataContext = new ZadaniaViewModel();
                        ModForm.Visibility = Visibility.Collapsed;
                        Filters.Visibility = Visibility.Visible;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Nie usunięto zadania, ponieważ jest związane z innymi tablicami");
                    }
                }
            }

        }


        private void AddFilter(object sender, RoutedEventArgs e)
        {
            StackPanel temp = Globals.StackPanel;

            ComboBox tempCB = Globals.ComboBoxColumnChoose;
            tempCB.ItemsSource = ((ZadaniaViewModel)this.DataContext).FilterInfo;
            temp.Children.Add(tempCB);

            temp.Children.Add(Globals.ComboBox);

            temp.Children.Add(Globals.Button);

            (temp.Children[0] as ComboBox).SelectedIndex = 0;

            FiltersList.Children.Add(temp);
        }

        private void UseFilters(object sender, RoutedEventArgs e)
        {
            (DataContext as ZadaniaViewModel).MainTable.RowFilter = Globals.GetFilter(FiltersList);
        }
    }
}
