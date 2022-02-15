using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        private DataTable mainTable = DBConnection.Basic("[dbo].[ProcSelectPracownicy]");
        public DataTable MainTable
        {
            get => mainTable;
            set
            {
                mainTable = value;
                OnPropertyChanged();
            }
        }
    }


}
