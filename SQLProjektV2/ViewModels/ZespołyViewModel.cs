﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class ZespołyViewModel : ViewModelBase
    {
        public ZespołyViewModel()
        {
            P.Add(-1, "Brak");
            Dictionary<int,string>  temp = DBConnection.GetDict("[dbo].[ProcForeignKeyPracownicy]");
            foreach (var v in temp)
                P.Add(v.Key, v.Value);
        }

        public Dictionary<int, string> P { get; set; } = new Dictionary<int, string>();

        private DataTable mainTable = DBConnection.Basic($"[dbo].[ProcSelectZespoły]");
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
