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
    /// Interaction logic for StanowiskaView.xaml
    /// </summary>
    public partial class StanowiskaView : UserControl
    {

        private string selectedId;
        public StanowiskaView()
        {
            InitializeComponent();
            DataContext = new StanowiskaViewModel();
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

            DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdStanowiska]", int.Parse(selectedId));

            MNazwaSource.Text = temp.Rows[0][0].ToString();
            MStazSource.Text = temp.Rows[0][1].ToString();
            MStawkaSource.Text = temp.Rows[0][2].ToString();
        }

        private void AddNewRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[stanowiska] WHERE Nazwa = '{NazwaSource.Text}'") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";
            if (NazwaSource.Text.Length == 0) errorString += "Podaj nazwę zespołu\n";

            if (StazSource.Text.Length == 0) errorString += "Podaj stawkę godzinową\n";
            else if (!int.TryParse(StazSource.Text, out _)) errorString += "Stawka godzinowa musi być liczbą całkowitą\n";
            else if (int.Parse(StazSource.Text) < 0) errorString += "Stawka godzinowa musi być większa lub równy 0\n";

            if (StawkaSource.Text.Length == 0) errorString += "Podaj stawkę godzinową\n";
            else if (!int.TryParse(StawkaSource.Text, out _)) errorString += "Stawka godzinowa musi być liczbą całkowitą\n";
            else if ( int.Parse(StawkaSource.Text ) < 20) errorString += "Stawka godzinowa musi być większa lub równy 20\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string nazwa = NazwaSource.Text;
                string staz = StazSource.Text;
                string stawka = StawkaSource.Text;

                string temp = $"INSERT INTO [dbo].[Stanowiska] VALUES ('{nazwa}', {staz}, {stawka})";
                MessageBox.Show("Dodano nowe stanowisko");
                DBConnection.SQLCommand(temp);
                DataContext = new StanowiskaViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {

            string errorString = "";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[stanowiska] WHERE Nazwa = '{NazwaSource.Text}' AND Id != {selectedId}") > 0) errorString += "Ten nazwa jest już używana przez inny zespół\n";
            if (MNazwaSource.Text.Length == 0) errorString += "Podaj nazwę zespołu\n";

            if (MStazSource.Text.Length == 0) errorString += "Podaj stawkę godzinową\n";
            else if (!int.TryParse(MStazSource.Text, out _)) errorString += "Stawka godzinowa musi być liczbą całkowitą\n";
            else if (int.Parse(MStazSource.Text) < 0) errorString += "Stawka godzinowa musi być większa lub równy 0\n";

            if (MStawkaSource.Text.Length == 0) errorString += "Podaj stawkę godzinową\n";
            else if (!int.TryParse(MStawkaSource.Text, out _)) errorString += "Stawka godzinowa musi być liczbą całkowitą\n";
            else if (int.Parse(MStawkaSource.Text) < 20) errorString += "Stawka godzinowa musi być większa lub równy 20\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string nazwa = MNazwaSource.Text;
                string staz = MStazSource.Text;
                string stawka = MStawkaSource.Text;

                string temp = $"UPDATE [dbo].[Stanowiska] SET nazwa = '{nazwa}', [Wymagany_staż] = {staz}, [Stawka_godzinowa] = {stawka} WHERE Id = {selectedId}";
                MessageBox.Show("Zmieniono dane o stanowisku");
                DBConnection.SQLCommand(temp);
                DataContext = new StanowiskaViewModel();
            }

        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            string temp = $"DELETE FROM [dbo].[stanowiska] WHERE Id = {selectedId}";
            try
            {
                DBConnection.SQLCommand(temp);
                MessageBox.Show("Usunięto stanowisko");
                DataContext = new StanowiskaViewModel();
                OptionChoose.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                MessageBox.Show("Nie usunięto stanowiska, ponieważ jest związany z innymi tablicami");
            }

        }

    }
}
