using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class ProjektyViewModel : ViewModelBase
    {
        public ProjektyViewModel()
        {
            Z = DBConnection.GetDict("[dbo].[ProcForeignKeyZespoły]");
            K = DBConnection.GetDict("[dbo].[ProcForeignKeyKlienci]");
        }

        public Dictionary<int, string> Z { get; set; }
        public Dictionary<int, string> K { get; set; }


        private DataView mainTable = new DataView(DBConnection.Basic("[dbo].[ProcSelectProjekty]"));
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
            {"Termin Oddania", "DateShort"},
            {"Opis", "String"},
            {"Klient", "String"},
            {"Kontakt", "String"},
            {"Zespół", "String"}
        };
    }
}
