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


        private DataTable mainTable = DBConnection.Basic("[dbo].[ProcSelectProjekty]");
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
