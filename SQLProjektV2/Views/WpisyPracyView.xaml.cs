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
    /// Interaction logic for WpisyPracyView.xaml
    /// </summary>
    public partial class WpisyPracyView : UserControl
    {
        private string selectedId = "0";
        private string selectedColumnId = "1";
        public WpisyPracyView()
        {
            InitializeComponent();
            DataContext = new WpisyPracyViewModel();
            DatePicker1.DefaultValue = DateTime.Now;
            DatePicker2.DefaultValue = DateTime.Now;
            DatePicker1.Value = DateTime.Now;
            DatePicker2.Value = DateTime.Now;

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
                DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdWpisy Pracy]", int.Parse(selectedId));

                MDatePicker1.Value = ((DateTime)temp.Rows[0][0]);
                MDatePicker2.Value = ((DateTime)temp.Rows[0][1]);
                MOpisSource.Text = temp.Rows[0][2].ToString();
                MMSource.SelectedValue = temp.Rows[0][3];
                MZSource.SelectedValue = temp.Rows[0][4];

            }
        }

        private void MainTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Id" || e.PropertyName == "PracId")
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
            string start = DatePicker1.Value.Value.ToString("yyyy-MM-dd HH:mm:ss");
            string stop = DatePicker2.Value.Value.ToString("yyyy-MM-dd HH:mm:ss");
             
            if(DatePicker1.Value.Value > DatePicker2.Value.Value) errorString += "Czas rozpoczęcia wpisu nie może być poźniejszy od czasu zakończenia wpisu\n";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Wpisy_Pracy] WHERE [dbo].FunGetPrac(Zadania_id) = [dbo].FunGetPrac({((KeyValuePair<int, string>)ZSource.SelectedItem).Key.ToString()}) AND (([Czas_rozpoczecia] < '{start}' AND [Czas_zakończenia] > '{start}') OR ([Czas_rozpoczecia] < '{stop}' AND [Czas_zakończenia] > '{stop}'))") > 0) errorString += "Pracownik w tym czasie ma już inny wpis pracy\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string opis = OpisSource.Text;
                string zadanie = ((KeyValuePair<int, string>)ZSource.SelectedItem).Key.ToString();
                string miejsce = ((KeyValuePair<int, string>)MSource.SelectedItem).Key.ToString();

                string temp = $"INSERT INTO [dbo].[Wpisy_Pracy] VALUES (1, '{start}', '{stop}', '{opis}', {miejsce}, {zadanie})";
                MessageBox.Show("Dodano nowy wpis pracy");
                DBConnection.SQLCommand(temp);
                DataContext = new WpisyPracyViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {

            string errorString = "";
            string start = MDatePicker1.Value.Value.ToString("yyyy-MM-dd HH:mm:ss");
            string stop = MDatePicker2.Value.Value.ToString("yyyy-MM-dd HH:mm:ss");

            if (MDatePicker1.Value.Value > MDatePicker2.Value.Value) errorString += "Czas rozpoczęcia wpisu nie może być poźniejszy od czasu zakończenia wpisu\n";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Wpisy_Pracy] WHERE Id != {selectedId} AND ([dbo].FunGetPrac(Zadania_id) = [dbo].FunGetPrac({((KeyValuePair<int, string>)MZSource.SelectedItem).Key.ToString()}) AND (([Czas_rozpoczecia] < '{start}' AND [Czas_zakończenia] > '{start}') OR ([Czas_rozpoczecia] < '{stop}' AND [Czas_zakończenia] > '{stop}')))") > 0) errorString += "Pracownik w tym czasie ma już inny wpis pracy\n";

            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string opis = MOpisSource.Text;
                string zadanie = ((KeyValuePair<int, string>)MZSource.SelectedItem).Key.ToString();
                string miejsce = ((KeyValuePair<int, string>)MMSource.SelectedItem).Key.ToString();

                string temp = $"UPDATE [dbo].[Wpisy_Pracy] SET [Czas_rozpoczecia] = '{start}', [Czas_zakończenia] = '{stop}', [Opis] = '{opis}', [Miejsca_id] = {miejsce}, [Zadania_id] = {zadanie} WHERE Id = {selectedId}";
                MessageBox.Show("Zmieniono treść wpisu pracy");
                DBConnection.SQLCommand(temp);
                DataContext = new WpisyPracyViewModel();
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                string temp = $"DELETE FROM [dbo].[Wpisy_Pracy] WHERE Id = {selectedId}";
                try
                {
                    DBConnection.SQLCommand(temp);
                    MessageBox.Show("Usunięto wpis pracy");
                    DataContext = new WpisyPracyViewModel();
                    ModForm.Visibility = Visibility.Collapsed;
                    Filters.Visibility = Visibility.Visible;
                }
                catch (Exception)
                {
                    MessageBox.Show("Nie usunięto wpisu, ponieważ jest związany z innymi tablicami");
                }
            }
        }


        private void AddFilter(object sender, RoutedEventArgs e)
        {
            StackPanel temp = Globals.StackPanel;

            ComboBox tempCB = Globals.ComboBoxColumnChoose;
            tempCB.ItemsSource = ((WpisyPracyViewModel)this.DataContext).FilterInfo;
            temp.Children.Add(tempCB);

            temp.Children.Add(Globals.ComboBox);

            temp.Children.Add(Globals.Button);

            (temp.Children[0] as ComboBox).SelectedIndex = 0;

            FiltersList.Children.Add(temp);
        }

        private void UseFilters(object sender, RoutedEventArgs e)
        {
            (DataContext as WpisyPracyViewModel).MainTable.RowFilter = Globals.GetFilter(FiltersList);
        }
    }
}
