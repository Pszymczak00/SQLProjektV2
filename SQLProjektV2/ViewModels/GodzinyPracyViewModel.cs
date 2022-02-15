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

        private DataTable mainTable = DBConnection.Basic("[dbo].[ProcSelectGodziny Pracy]");
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
