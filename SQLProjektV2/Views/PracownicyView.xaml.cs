using SQLProjektV2.ViewModels;
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
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.RegularExpressions;

namespace SQLProjektV2.Views
{
    /// <summary>
    /// Interaction logic for Pracownicy.xaml
    /// </summary>
    public partial class PracownicyView : UserControl
    {
        private string selectedId = "0";
        private string selectedColumnId = "1";
        public PracownicyView()
        {
            InitializeComponent();
            DataContext = new PracownicyViewModel();

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

                DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdPracownicy]", int.Parse(selectedId));

                MImieSource.Text = temp.Rows[0][0].ToString();
                MNazwiskoSource.Text = temp.Rows[0][1].ToString();
                MDatePicker1.SelectedDate = ((DateTime)temp.Rows[0][2]);
                MEmailSource.Text = temp.Rows[0][3].ToString();
                MNumerSource.Text = temp.Rows[0][4].ToString();
                if (temp.Rows[0][5].GetType().ToString() == "System.DateTime")
                    MDatePicker2.SelectedDate = ((DateTime)temp.Rows[0][5]);
                MRZSource.SelectedValue = temp.Rows[0][6];
                MSSource.SelectedValue = temp.Rows[0][7];
                MZSource.SelectedValue = temp.Rows[0][8];
            }
        }

        private void MainTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";

            if (e.PropertyName == "Id")
                (e.Column as DataGridTextColumn).MaxWidth = 0;

        }



        private void AddFormVisible(object sender, RoutedEventArgs e)
        {
            AddForm.Visibility = Visibility.Visible;
            ModForm.Visibility = Visibility.Collapsed;
            Filters.Visibility = Visibility.Collapsed;
            DataGridRow row = (DataGridRow)MainTable.ItemContainerGenerator.ContainerFromIndex(int.Parse(selectedColumnId));
            if(row != null)
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
            var foo = new EmailAddressAttribute();

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[pracownicy] WHERE Email = '{EmailSource.Text}'") > 0) errorString += "Ten email jest już używany przez innego pracownika\n";
            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[pracownicy] WHERE [Numer_telefonu] = '{NumerSource.Text}'") > 0) errorString += "Ten numer telefonu jest już używany przez innego pracownika\n";
            if (ImieSource.Text.Length == 0) errorString += "Podaj imię pracownika\n";
            if (NazwiskoSource.Text.Length == 0) errorString += "Podaj nazwisko pracownika\n";
            if (!DatePicker1.SelectedDate.HasValue) errorString += "Podaj datę zatrudnienia\n";
            if(!foo.IsValid(EmailSource.Text)) errorString += "Błędny format adresu email \n";
            if (!NumerSource.Text.All(char.IsDigit)) errorString += "Numer może zawierać wyłącznie cyfry\n";
            if (DatePicker1.SelectedDate.HasValue && DatePicker2.SelectedDate.HasValue && DatePicker1.SelectedDate.Value > DatePicker2.SelectedDate.Value) errorString += "Data zwolnienia jest wcześniejsza niż data zatrudnienia. \n";

            string number = NumerSource.Text.Length > 0 ? $"'{NumerSource.Text}'" : "null";
            string date1 = DatePicker1.SelectedDate.Value.ToString("yyyy-MM-dd");
            string date2 = DatePicker2.SelectedDate.HasValue ?  $"'{DatePicker2.SelectedDate.Value.ToString("yyyy-MM-dd")}'" : "null";
            string hasDate = DatePicker2.SelectedDate.HasValue ? "1" : "0";

            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string temp = $"INSERT INTO [dbo].[Pracownicy] VALUES ('{ImieSource.Text}', '{NazwiskoSource.Text}', '{date1}', '{EmailSource.Text}', {number}, {hasDate}, {date2}, {((KeyValuePair<int,string>)RZSource.SelectedItem).Key}, {((KeyValuePair<int, string>)SSource.SelectedItem).Key}, {((KeyValuePair<int, string>)ZSource.SelectedItem).Key})";
                MessageBox.Show("Dodano nowego pracownika");
                DBConnection.SQLCommand(temp);
                DataContext = new PracownicyViewModel();
            }

        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";
            var foo = new EmailAddressAttribute();

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[pracownicy] WHERE Email = '{EmailSource.Text}' AND Id_prac != {selectedId}") > 0) errorString += "Ten email jest już używany przez innego pracownika\n";
            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[pracownicy] WHERE [Numer_telefonu] = '{NumerSource.Text}' AND Id_prac != {selectedId}") > 0) errorString += "Ten numer telefonu jest już używany przez innego pracownika\n";

            if (MImieSource.Text.Length == 0) errorString += "Podaj imię pracownika\n";
            if (MNazwiskoSource.Text.Length == 0) errorString += "Podaj nazwisko pracownika\n";
            if (!MDatePicker1.SelectedDate.HasValue) errorString += "Podaj datę zatrudnienia\n";
            
            if (!foo.IsValid(MEmailSource.Text)) errorString += "Błędny format adresu email \n";
            if (!MNumerSource.Text.All(char.IsDigit)) errorString += "Numer może zawierać wyłącznie cyfry\n";
            if (MDatePicker1.SelectedDate.HasValue && MDatePicker2.SelectedDate.HasValue && MDatePicker1.SelectedDate.Value > MDatePicker2.SelectedDate.Value) errorString += "Data zwolnienia jest wcześniejsza niż data zatrudnienia. \n";

            string number = MNumerSource.Text.Length > 0 ? $"'{MNumerSource.Text}'" : "null";
            string date1 = MDatePicker1.SelectedDate.Value.ToString("yyyy-MM-dd");
            string date2 = MDatePicker2.SelectedDate.HasValue ? $"'{MDatePicker2.SelectedDate.Value.ToString("yyyy-MM-dd")}'" : "null";
            string hasDate = MDatePicker2.SelectedDate.HasValue ? "1" : "0";

            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string temp = $@"UPDATE [dbo].[Pracownicy] SET 
                            Imię = '{MImieSource.Text}', 
                            Nazwisko = '{MNazwiskoSource.Text}', 
                            Data_zatrudnienia = '{date1}',
                            [Email] = '{MEmailSource.Text}',
                            [Numer_telefonu] = {number},
                            [Czy_zwolniony] = {hasDate}, 
                            [Data_zwolnienia] = {date2},
                            [Rodzaje_zatrudnienia_Id] = {((KeyValuePair<int, string>)MRZSource.SelectedItem).Key},
                            [Stanowiska_Id] = {((KeyValuePair<int, string>)MSSource.SelectedItem).Key}, 
                            [Zespoły_Id] = {((KeyValuePair<int, string>)MZSource.SelectedItem).Key}
                            WHERE Id_prac = {selectedId}";
                MessageBox.Show("Zaaktualizowano dane o pracowniku");
                DBConnection.SQLCommand(temp);
                DataContext = new PracownicyViewModel();
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Czy na pewno chcesz usunąć wpis?", "", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                string output = "";
                int test = DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Sprzęty] WHERE [Pracownicy_Id_prac] = {selectedId}");
                if (test > 0)
                    output += $"Nie można usunąc tego pracownika, ponieważ jest do niego przypisanych {test} sprzętów.\n";
                int test2 = DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Zadania] WHERE [Pracownicy_Id_prac] = {selectedId}");
                if(test2 > 0)
                    output += $"Nie można usunąc tego pracownika, ponieważ jest do niego przypisanych {test2} zadań.";
                int test3 = DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Godziny_pracy] WHERE [Zadeklarowane_godziny] = {selectedId}");
                if (test3 > 0)
                    output += $"Nie można usunąc tego pracownika, ponieważ ma on {test3} deklaracji godzin pracy.";

                if (test == 0 && test2 == 0 && test3 == 0)
                {
                    string temp = $"DELETE FROM [dbo].[pracownicy] WHERE Id_prac = {selectedId}";
                    try
                    {
                        DBConnection.SQLCommand(temp);
                        MessageBox.Show("Usunięto pracownika");
                        DataContext = new PracownicyViewModel();
                        ModForm.Visibility = Visibility.Collapsed;
                        Filters.Visibility = Visibility.Visible;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Nie usunięto pracownika, ponieważ jest związany z innymi tablicami");
                    }
                }
                else MessageBox.Show(output);

            }
        }



        private void AddFilter(object sender, RoutedEventArgs e)
        {
            StackPanel temp = Globals.StackPanel;

            ComboBox tempCB = Globals.ComboBoxColumnChoose;
            tempCB.ItemsSource = ((PracownicyViewModel)this.DataContext).FilterInfo;
            temp.Children.Add(tempCB);

            temp.Children.Add(Globals.ComboBox);

            temp.Children.Add(Globals.Button);

            (temp.Children[0] as ComboBox).SelectedIndex = 0;

            FiltersList.Children.Add(temp);
        }

        private void UseFilters(object sender, RoutedEventArgs e)
        {
            (DataContext as PracownicyViewModel).MainTable.RowFilter = Globals.GetFilter(FiltersList);
        }


    }
}
