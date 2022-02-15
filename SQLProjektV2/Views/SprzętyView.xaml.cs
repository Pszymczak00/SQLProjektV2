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
    /// Interaction logic for SprzętyView.xaml
    /// </summary>
    public partial class SprzętyView : UserControl
    {
        string selectedId;
        public SprzętyView()
        {
            InitializeComponent();
            DataContext = new SprzętyViewModel();
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

        private void AddNewRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (TypSource.Text.Length == 0) errorString += "Podaj typ urządzenia\n";
            if (NazwaSource.Text.Length == 0) errorString += "Podaj nazwe urządzenia\n";
            string tempOpis = OpisSource.Text.Length != 0 ? $"'{OpisSource.Text}'" : "''";
            if ((errorString.Length == 0) &&  DBConnection.SQLCommandRet($"select count(*) from [dbo].[Sprzęty] WHERE Typ = '{TypSource.Text}' AND Nazwa = '{NazwaSource.Text}' AND Opis = {tempOpis} AND Pracownicy_Id_Prac = '{((KeyValuePair<int, string>)PSource.SelectedItem).Key}'") > 0) errorString += "Już jest taki sprzęt\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string typ = TypSource.Text;
                string nazwa = NazwaSource.Text;
                string opis = OpisSource.Text;
                string pracownik = ((KeyValuePair<int, string>)PSource.SelectedItem).Key.ToString();

                string temp = $"INSERT INTO [dbo].[Sprzęty] VALUES ('{typ}', '{nazwa}', '{opis}', {pracownik})";
                MessageBox.Show("Dodano informacje o nowym urządzeniu");
                DBConnection.SQLCommand(temp);
                DataContext = new SprzętyViewModel();
            }
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

            DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdSprzęty]", int.Parse(selectedId));

            MTypSource.Text = temp.Rows[0][0].ToString();
            MNazwaSource.Text = temp.Rows[0][1].ToString();
            MOpisSource.Text = temp.Rows[0][2].ToString();
            MPSource.SelectedValue = temp.Rows[0][3];
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {

            string errorString = "";

            if (MTypSource.Text.Length == 0) errorString += "Podaj typ urządzenia\n";
            if (MNazwaSource.Text.Length == 0) errorString += "Podaj nazwe urządzenia\n";
            string tempOpis = MOpisSource.Text.Length != 0 ? $"'{MOpisSource.Text}'" : "null";
            if ((errorString.Length == 0) && DBConnection.SQLCommandRet($"select count(*) from [dbo].[Sprzęty] WHERE Typ = '{MTypSource.Text}' AND Nazwa = '{MNazwaSource.Text}' AND Opis = {tempOpis} AND Pracownicy_Id_Prac = '{((KeyValuePair<int, string>)MPSource.SelectedItem).Key} 'AND Id != {selectedId}") > 0) errorString += "Już jest taki sprzęt\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {
                string typ = MTypSource.Text;
                string nazwa = MNazwaSource.Text;
                string opis = MOpisSource.Text;
                string pracownik = ((KeyValuePair<int, string>)MPSource.SelectedItem).Key.ToString();

                string temp = $"UPDATE [dbo].[Sprzęty] SET Typ = '{typ}', Nazwa = '{nazwa}', Opis = '{opis}', [Pracownicy_Id_prac] = {pracownik} WHERE Id = {selectedId}";
                MessageBox.Show("Zaaktualizowano informacje o użądzeniu");
                DBConnection.SQLCommand(temp);
                DataContext = new SprzętyViewModel();
              
            }
        }
        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            string temp = $"DELETE FROM [dbo].[Sprzęty] WHERE Id = {selectedId}";
            try
            {
                DBConnection.SQLCommand(temp);
                MessageBox.Show("Usunięto sprzęt");
                DataContext = new SprzętyViewModel();
                OptionChoose.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                MessageBox.Show("Nie usunięto sprzętu, ponieważ jest związany z innymi tablicami");
            }
        }
    }
}
