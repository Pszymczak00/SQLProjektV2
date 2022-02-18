using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels 
{
    class GodzinyPracyViewModel : ViewModelBase
    {
        public GodzinyPracyViewModel()
        {
            P = DBConnection.GetDict("[dbo].[ProcForeignKeyPracownicy]");
        }

        public Dictionary<int, string> P { get; set; }

        private DataView mainTable = new DataView(DBConnection.Basic("[dbo].[ProcSelectGodziny Pracy]"));
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
            {"Pracownik", "String"},
            {"Kontakt", "String"},
            {"Miesiąc", "DateMonth"},
            {"Zadeklarowane Godziny", "Number"},
        };
    }
}
