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
    /// Interaction logic for MiejscaView.xaml
    /// </summary>
    public partial class MiejscaView : UserControl
    {
        private string selectedId;
        public MiejscaView()
        {
            InitializeComponent();
            DataContext = new MiejscaViewModel();
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
                OptionChoose.Visibility = Visibility.Visible;
                AddForm.Visibility = Visibility.Collapsed;
                ModForm.Visibility = Visibility.Collapsed;
            }
        }

        private void MainTable_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Id")
                (e.Column as DataGridTextColumn).MaxWidth = 0;
        }

        private void AddFormVisible(object sender, RoutedEventArgs e)
        {
            OptionChoose.Visibility = Visibility.Collapsed;
            AddForm.Visibility = Visibility.Visible;
            ModForm.Visibility = Visibility.Collapsed;
        }

        private void ModFormVisible(object sender, RoutedEventArgs e)
        {
            AddForm.Visibility = Visibility.Collapsed;
            OptionChoose.Visibility = Visibility.Collapsed;
            ModForm.Visibility = Visibility.Visible;

            DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdMiejsca]", int.Parse(selectedId));

            MAdresSource.Text = temp.Rows[0][0].ToString();
            MNumerSource.Text = temp.Rows[0][1].ToString();
        }

        private void AddNewRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (AdresSource.Text.Length == 0)
            { errorString += "Podaj adres\n"; MessageBox.Show(errorString); }
            else if (NumerSource.Text.Length != 0)
            {
                if (!int.TryParse(NumerSource.Text, out _)) errorString += "Numer budynku musi być liczbą całkowitą\n";
                else if (int.Parse(NumerSource.Text) < 1) errorString += "Numer budynku musi być wiekszy oo zera\n";
                else if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Miejsca] WHERE Adres = '{AdresSource.Text}' AND Nr_pokoju = {NumerSource.Text}") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";

                if (errorString.Length != 0) MessageBox.Show(errorString);
                else
                {

                    string adres = AdresSource.Text;
                    string numer = NumerSource.Text;

                    string temp = $"INSERT INTO [dbo].[Miejsca] VALUES ('{adres}', {numer})";
                    MessageBox.Show("Dodano nowe miejsce");
                    DBConnection.SQLCommand(temp);
                    DataContext = new MiejscaViewModel();
                }
            }
            else
            {
                if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Miejsca] WHERE Adres = '{AdresSource.Text}' AND Nr_pokoju IS NULL") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";

                if (errorString.Length != 0) MessageBox.Show(errorString);
                else
                {

                    string adres = AdresSource.Text;
                    string numer = NumerSource.Text;

                    string temp = $"INSERT INTO [dbo].[Miejsca] VALUES ('{adres}', null)";
                    MessageBox.Show("Dodano nowe miejsce");
                    DBConnection.SQLCommand(temp);
                    DataContext = new MiejscaViewModel();
                }
            }




        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {

            string errorString = "";

            if (MAdresSource.Text.Length == 0)
            { errorString += "Podaj adres\n"; MessageBox.Show(errorString); }
            else if (MNumerSource.Text.Length != 0)
            {
                if (!int.TryParse(MNumerSource.Text, out _)) errorString += "Numer budynku musi być liczbą całkowitą\n";
                else if (int.Parse(MNumerSource.Text) < 1) errorString += "Numer budynku musi być wiekszy oo zera\n";
                else if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Miejsca] WHERE Adres = '{MAdresSource.Text}' AND Nr_pokoju = {MNumerSource.Text} AND Id != {selectedId}") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";

                if (errorString.Length != 0) MessageBox.Show(errorString);
                else
                {

                    string adres = MAdresSource.Text;
                    string numer = MNumerSource.Text;

                    string temp = $"UPDATE [dbo].[Miejsca] SET adres = '{adres}', nr_pokoju = {numer} WHERE Id = {selectedId}";
                    MessageBox.Show("Zmieniono dane o lokalizacji");
                    DBConnection.SQLCommand(temp);
                    DataContext = new MiejscaViewModel();
                }
            }
            else
            {
                if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Miejsca] WHERE Adres = '{MAdresSource.Text}' AND Nr_pokoju IS NULL AND Id != {selectedId}") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";

                if (errorString.Length != 0) MessageBox.Show(errorString);
                else
                {

                    string adres = MAdresSource.Text;
                    string numer = MNumerSource.Text;

                    string temp = $"UPDATE [dbo].[Miejsca] SET adres = '{adres}', nr_pokoju = null WHERE Id = {selectedId}";
                    MessageBox.Show("Dodano nowe miejsce");
                    DBConnection.SQLCommand(temp);
                    DataContext = new MiejscaViewModel();
                }
            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            string temp = $"DELETE FROM [dbo].[Miejsca] WHERE Id = {selectedId}";
            try
            {
                DBConnection.SQLCommand(temp);
                MessageBox.Show("Usunięto miejsce");
                DataContext = new MiejscaViewModel();
                OptionChoose.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                MessageBox.Show("Nie usunięto miejsca, ponieważ jest związane z innymi tablicami");
            }

        }
    }
}
