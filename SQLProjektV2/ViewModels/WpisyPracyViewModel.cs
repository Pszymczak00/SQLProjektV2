using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class WpisyPracyViewModel : ViewModelBase
    {
        public WpisyPracyViewModel()
        {
            Z = DBConnection.GetDict("[dbo].[ProcForeignKeyZadania]");
            M = DBConnection.GetDict("[dbo].[ProcForeignKeyMiejsca]");
        }

        public Dictionary<int, string> Z { get; set; }
        public Dictionary<int, string> M { get; set; }


        private DataView mainTable = new DataView(DBConnection.Basic("[dbo].[ProcSelectWpisy Pracy]"));
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
            {"Czas Rozpoczecia", "DateLong"},
            {"Czas Zakonczenia", "DateLong"},
            {"Opis", "String"},
            {"Adres", "String"},
            {"Zadanie", "String"},
            {"Pracownik", "String"},
            {"Kontakt", "String" },
        };
    }
}
