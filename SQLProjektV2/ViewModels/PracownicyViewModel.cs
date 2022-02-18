using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SQLProjektV2.ViewModels
{
    public class PracownicyViewModel : ViewModelBase
    {
        public PracownicyViewModel()
        {
            RZ = DBConnection.GetDict("[dbo].[ProcForeignKeyRodzaje Zatrudnienia]");
            S = DBConnection.GetDict("[dbo].[ProcForeignKeyStanowiska]");
            Z = DBConnection.GetDict("[dbo].[ProcForeignKeyZespoły]");

        }

        public Dictionary<int, string> RZ { get; set; }
        public Dictionary<int, string> S { get; set; }
        public Dictionary<int, string> Z { get; set; }


        private DataView mainTable = new DataView( DBConnection.Basic("[dbo].[ProcSelectPracownicy]") );
        public DataView MainTable
        {
            get => mainTable;
            set
            {
                mainTable = value;
                OnPropertyChanged();
            }
        }

        public Dictionary<string, string> FilterInfo { get; set; } = new Dictionary<string, string>
        {
            {"Imię", "String"},
            {"Nazwisko", "String"},
            {"Data Zatrudnienia", "DateShort"},
            {"Email", "String"},
            {"Numer Telefonu", "StringNumbers"},
            {"Data Zwolnienia", "DateShort"},
            {"Rodzaj Zatrudnienia", "String" },
            {"Stanowisko", "String" },
            {"Zespół", "String" }
        };
            
    }


}
