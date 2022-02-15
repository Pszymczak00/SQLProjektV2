using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class RodzajeZatrudnieniaViewModel : ViewModelBase
    {
        private DataTable mainTable = DBConnection.Basic($"[dbo].[ProcSelectRodzaje Zatrudnienia]");
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
