using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class KlienciViewModel : ViewModelBase
    {
        private DataView mainTable = new DataView(DBConnection.Basic($"[dbo].[ProcSelectKlienci]"));
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
            {"Email Klienta", "String"},
            {"Numer Telefonu", "StringNumbers"},
     
        };
    }
}
