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
    /// Interaction logic for ZespołyView.xaml
    /// </summary>
    public partial class ZespołyView : UserControl
    {
        private string selectedId = "0";
        private string selectedColumnId = "1";
        public ZespołyView()
        {
            InitializeComponent();
            DataContext = new ZespołyViewModel();
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
                DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdZespoły]", int.Parse(selectedId));

                MNazwaSource.Text = temp.Rows[0][0].ToString();
                if (temp.Rows[0][1].ToString() == "")
                    MPSource.SelectedValue = -1;
                else
                    MPSource.SelectedValue = temp.Rows[0][1];
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

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[zespoły] WHERE Nazwa = '{NazwaSource.Text}'") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";
            if (NazwaSource.Text.Length == 0) errorString += "Podaj nazwę zespołu\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string nazwa = NazwaSource.Text;
                string kierownik;
                if (((KeyValuePair<int, string>)PSource.SelectedItem).Key != -1)
                    kierownik = ((KeyValuePair<int, string>)PSource.SelectedItem).Key.ToString();
                else
                    kierownik = "null";

                string temp = $"INSERT INTO [dbo].[Zespoły] VALUES ('{nazwa}', {kierownik})";
                MessageBox.Show("Dodano nowy zespół");
                DBConnection.SQLCommand(temp);
                DataContext = new ZespołyViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[zespoły] WHERE Nazwa = '{MNazwaSource.Text}' AND Id != {selectedId}") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";
            if (MNazwaSource.Text.Length == 0) errorString += "Podaj nazwę zespołu\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string nazwa = MNazwaSource.Text;
                string kierownik;
                if (((KeyValuePair<int, string>)MPSource.SelectedItem).Key != -1)
                    kierownik = ((KeyValuePair<int, string>)MPSource.SelectedItem).Key.ToString();
                else
                    kierownik = "null";

                string temp = $"UPDATE [dbo].[Zespoły] SET nazwa = '{nazwa}', Id_kierownika = {kierownik} WHERE Id = {selectedId}";
                MessageBox.Show("Zmieniono informacje o zespole");
                DBConnection.SQLCommand(temp);
                DataContext = new ZespołyViewModel();
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                string output = "";
                int test = DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Pracownicy] WHERE [Zespoły_Id] = {selectedId}");
                if (test > 0)
                    output += $"Nie można usunąc tego zespołu, ponieważ jest w nim {test} pracowników.\n";
                int test2 = DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Projekty] WHERE [ZespoLy_id] = {selectedId}");
                if (test2 > 0)
                    output += $"Nie można usunąc tego zespołu, ponieważ ma przypisane {test2} projektów.";

                if (test == 0 && test2 == 0)
                {
                    string temp = $"DELETE FROM [dbo].[zespoły] WHERE Id = {selectedId}";
                    try
                    {
                        DBConnection.SQLCommand(temp);
                        MessageBox.Show("Usunięto zespół");
                        DataContext = new ZespołyViewModel();
                        ModForm.Visibility = Visibility.Collapsed;
                        Filters.Visibility = Visibility.Visible;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Nie usunięto zespołu, ponieważ jest związany z innymi tablicami");
                    }
                }
                else MessageBox.Show(output);
            }

        }


        private void AddFilter(object sender, RoutedEventArgs e)
        {
            StackPanel temp = Globals.StackPanel;

            ComboBox tempCB = Globals.ComboBoxColumnChoose;
            tempCB.ItemsSource = ((ZespołyViewModel)this.DataContext).FilterInfo;
            temp.Children.Add(tempCB);

            temp.Children.Add(Globals.ComboBox);

            temp.Children.Add(Globals.Button);

            (temp.Children[0] as ComboBox).SelectedIndex = 0;

            FiltersList.Children.Add(temp);
        }

        private void UseFilters(object sender, RoutedEventArgs e)
        {
            (DataContext as ZespołyViewModel).MainTable.RowFilter = Globals.GetFilter(FiltersList);
        }
    }
}
