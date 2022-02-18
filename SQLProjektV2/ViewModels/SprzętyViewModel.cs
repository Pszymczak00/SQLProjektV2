using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class SprzętyViewModel : ViewModelBase
    {
        public SprzętyViewModel()
        {
            P = DBConnection.GetDict("[dbo].[ProcForeignKeyPracownicy]");
        }

        public Dictionary<int, string> P { get; set; }

        private DataView mainTable = new DataView(DBConnection.Basic("[dbo].[ProcSelectSprzęty]"));
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
            {"Typ", "String"},
            {"Nazwa", "String"},
            {"Opis", "String"},
            {"Pracownik", "String"},
            {"Kontakt", "String"},
        };
    }
}
