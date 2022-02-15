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

        private DataTable mainTable = DBConnection.Basic("[dbo].[ProcSelectSprzęty]");
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
