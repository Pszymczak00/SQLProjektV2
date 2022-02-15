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


        private DataTable mainTable = DBConnection.Basic("[dbo].[ProcSelectWpisy Pracy]");
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
