using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class ZadaniaViewModel :ViewModelBase
    {
        public ZadaniaViewModel()
        {
            P = DBConnection.GetDict("[dbo].[ProcForeignKeyPracownicy]");
            PROJ = DBConnection.GetDict("[dbo].[ProcForeignKeyProjekty]");
        }

        public Dictionary<int, string> P { get; set; }
        public Dictionary<int, string> PROJ { get; set; }


        private DataView mainTable = new DataView(DBConnection.Basic("[dbo].[ProcSelectZadania]"));
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
            {"Nazwa", "String"},
            {"Czy Zakończone", "Boolean"},
            {"Przeznaczony Czas", "Number"},
            {"Opis", "String"},
            {"Pracownik", "String"},
            {"Kontakt", "String"},
            {"Projekt", "String" },
        };
    }
}
