using SQLProjektV2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    /// Interaction logic for KlienciView.xaml
    /// </summary>
    public partial class KlienciView : UserControl
    {
        private string selectedId;
        public KlienciView()
        {
            InitializeComponent();
            DataContext = new KlienciViewModel();
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

            DataTable temp = DBConnection.BasicId("[dbo].[ProcSelectIdKlienci]", int.Parse(selectedId));

            MImięSource.Text = temp.Rows[0][0].ToString();
            MNazwiskoSource.Text = temp.Rows[0][1].ToString();
            MEmailSource.Text = temp.Rows[0][2].ToString();
            MNumerSource.Text = temp.Rows[0][3].ToString();
        }



        private void AddNewRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";
            var foo = new EmailAddressAttribute();

            if (ImięSource.Text.Length == 0) errorString += "Podaj imię klienta\n";
            if (NazwiskoSource.Text.Length == 0) errorString += "Podaj nazwisko nazwisko\n";
            if (!foo.IsValid(EmailSource.Text)) errorString += "Błędny format adresu email \n";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Klienci] WHERE [Email_klienta] = '{EmailSource.Text}'") > 0) errorString += "Jest już klient o takim adresie email\n";
            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Klienci] WHERE numer_telefonu = '{NumerSource.Text}'") > 0) errorString += "Jest już klient o takim numerze telefonu\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string imie = ImięSource.Text;
                string nazwisko = NazwiskoSource.Text;
                string email = EmailSource.Text;
                string number = NumerSource.Text.Length > 0 ? $"'{NumerSource.Text}'" : "null";

                string temp = $"INSERT INTO [dbo].[Klienci] VALUES ('{imie}', '{nazwisko}', '{email}', {number})";
                MessageBox.Show("Dodano nowego kllient");
                DBConnection.SQLCommand(temp);
                DataContext = new KlienciViewModel();
            }
        }

        private void UpdateRecord(object sender, RoutedEventArgs e)
        {
            string errorString = "";
            var foo = new EmailAddressAttribute();

            if (MImięSource.Text.Length == 0) errorString += "Podaj imię klienta\n";
            if (MNazwiskoSource.Text.Length == 0) errorString += "Podaj nazwisko nazwisko\n";
            if (!foo.IsValid(MEmailSource.Text)) errorString += "Błędny format adresu email \n";

            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Klienci] WHERE [Email_klienta] = '{MEmailSource.Text}' AND Id != {selectedId}") > 0) errorString += "Jest już klient o takim adresie email\n";
            if (DBConnection.SQLCommandRet($"SELECT COUNT(*) FROM [dbo].[Klienci] WHERE numer_telefonu = '{MNumerSource.Text}' AND Id != {selectedId}") > 0) errorString += "Jest już klient o takim numerze telefonu\n";


            if (errorString.Length != 0) MessageBox.Show(errorString);
            else
            {

                string imie = MImięSource.Text;
                string nazwisko = MNazwiskoSource.Text;
                string email = MEmailSource.Text;
                string number = MNumerSource.Text.Length > 0 ? $"'{MNumerSource.Text}'" : "null";

                string temp = $"UPDATE [dbo].[Klienci] SET [Imię] = '{imie}', [Nazwisko] = '{nazwisko}', [Email_klienta] = '{email}', [Numer_telefonu] = {number} WHERE Id = {selectedId}";
                MessageBox.Show("Zmieniono dane klienta");
                DBConnection.SQLCommand(temp);
                DataContext = new KlienciViewModel();

            }
        }

        private void DeleteRecord(object sender, RoutedEventArgs e)
        {
            string temp = $"DELETE FROM [dbo].[Klienci] WHERE Id = {selectedId}";
            try
            {
                DBConnection.SQLCommand(temp);
                MessageBox.Show("Usunięto klienta");
                DataContext = new KlienciViewModel();
                OptionChoose.Visibility = Visibility.Collapsed;
            }
            catch (Exception)
            {
                MessageBox.Show("Nie usunięto klienta, ponieważ jest związany z innymi tablicami");
            }

        }

    }
}
