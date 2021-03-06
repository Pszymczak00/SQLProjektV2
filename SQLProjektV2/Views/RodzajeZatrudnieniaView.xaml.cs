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
    /// Interaction logic for RodzajeZatrudnieniaView.xaml
    /// </summary>
    public partial class RodzajeZatrudnieniaView : UserControl
    {
        private string selectedId = "0";
        private string selectedColumnId = "1";
        public RodzajeZatrudnieniaView()
        {
            InitializeComponent();
            DataContext = new RodzajeZatrudnieniaViewModel();
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
                Filters.Visibility = Visibility.Collapsed;
                ModForm.Visibility = Visibility.Visible;
                row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
                row.BorderBrush = Brushes.White;
                row.BorderThickness = new Thickness(2);
                DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdRodzaje Zatrudnienia]", int.Parse(selectedId));

                MNazwaSource.Text = temp.Rows[0][0].ToString();
                MMinSource.Text = temp.Rows[0][1].ToString();
                MMaxSource.Text = temp.Rows[0][2].ToString();
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

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[rodzaje_zatrudnienia] WHERE Nazwa = '{NazwaSource.Text}'") > 0) errorString += "Ta nazwa jest już używana zajęta\n";
            if (NazwaSource.Text.Length == 0) errorString += "Podaj nazwę zespołu\n";
            if (MinSource.Text.Length == 0) errorString += "Podaj minimalną liczbę godzin\n";
            else if (!int.TryParse(MinSource.Text, out _)) errorString += "Liczba godzin musi być liczbą\n";
            if (MaxSource.Text.Length == 0) errorString += "Podaj maksymalną liczbę godzin\n";
            else if (!int.TryParse(MaxSource.Text, out _)) errorString += "Liczba godzin musi być liczbą\n";
            if (MinSource.Text.Length > 0 && MaxSource.Text.Length > 0 && int.TryParse(MinSource.Text, out _) && int.TryParse(MaxSource.Text, out _) && int.Parse(MinSource.Text) > int.Parse(MaxSource.Text)) errorString += "Minimalna ilość godzin musi być mniejsza niż maksymalna\n";
            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string nazwa = NazwaSource.Text;
                string minGodzin = MinSource.Text;
                string maxGodzin = MaxSource.Text;

                string temp = $"INSERT INTO [dbo].[Rodzaje_zatrudnienia] VALUES ('{nazwa}', {minGodzin}, {maxGodzin})";
                MessageBox.Show("Dodano informacje o nowym rodzaju zatrudnienia");
                DBConnection.SQLCommand(temp);
                DataContext = new RodzajeZatrudnieniaViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[rodzaje_zatrudnienia] WHERE Nazwa = '{NazwaSource.Text}' AND Id != {selectedId}") > 0) errorString += "Ta nazwa jest już używana zajęta\n";
            if (MNazwaSource.Text.Length == 0) errorString += "Podaj nazwę zespołu\n";
            if (MMinSource.Text.Length == 0) errorString += "Podaj minimalną liczbę godzin\n";
            else if (!int.TryParse(MMinSource.Text, out _)) errorString += "Liczba godzin musi być liczbą całkowitą\n";
            if (MMaxSource.Text.Length == 0) errorString += "Podaj maksymalną liczbę godzin\n";
            else if (!int.TryParse(MMaxSource.Text, out _)) errorString += "Liczba godzin musi być liczbą całkowitą\n";
            if (MMinSource.Text.Length > 0 && MMaxSource.Text.Length > 0 && int.TryParse(MMinSource.Text, out _) && int.TryParse(MMaxSource.Text, out _) && int.Parse(MMinSource.Text) > int.Parse(MMaxSource.Text)) errorString += "Minimalna ilość godzin musi być mniejsza niż maksymalna\n";
            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string nazwa = MNazwaSource.Text;
                string minGodzin = MMinSource.Text;
                string maxGodzin = MMaxSource.Text;

                string temp = $"UPDATE [dbo].[Rodzaje_zatrudnienia] SET nazwa = '{nazwa}', min_godzin = {minGodzin}, max_godzin = {maxGodzin} WHERE Id = {selectedId}";
                MessageBox.Show("Zaaktualizowano dane o rodzaju zatrudnienia");
                DBConnection.SQLCommand(temp);
                DataContext = new RodzajeZatrudnieniaViewModel();
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                int test = DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Pracownicy] WHERE [Rodzaje_zatrudnienia_Id] = {selectedId}");
                if (test > 0)
                    MessageBox.Show($"Nie można usunąc tego rodzaju zatrudnienia, ponieważ {test} pracowników pracuje na takiej umowie.");

                else
                {
                    string temp = $"DELETE FROM [dbo].[rodzaje_zatrudnienia] WHERE Id = {selectedId}";
                    try
                    {
                        DBConnection.SQLCommand(temp);
                        MessageBox.Show("Usunięto rodzaj zatrudnienia");
                        DataContext = new RodzajeZatrudnieniaViewModel();
                        ModForm.Visibility = Visibility.Collapsed;
                        Filters.Visibility = Visibility.Visible;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Nie usunięto rodzaju zatrudnienia, ponieważ jest związany z innymi tablicami");
                    }
                }
            }
        }


        private void AddFilter(object sender, RoutedEventArgs e)
        {
            StackPanel temp = Globals.StackPanel;

            ComboBox tempCB = Globals.ComboBoxColumnChoose;
            tempCB.ItemsSource = ((RodzajeZatrudnieniaViewModel)this.DataContext).FilterInfo;
            temp.Children.Add(tempCB);

            temp.Children.Add(Globals.ComboBox);

            temp.Children.Add(Globals.Button);

            (temp.Children[0] as ComboBox).SelectedIndex = 0;

            FiltersList.Children.Add(temp);
        }

        private void UseFilters(object sender, RoutedEventArgs e)
        {
            (DataContext as RodzajeZatrudnieniaViewModel).MainTable.RowFilter = Globals.GetFilter(FiltersList);
        }

    }
}
