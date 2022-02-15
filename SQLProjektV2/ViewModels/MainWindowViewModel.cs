using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLProjektV2.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        static public List<string> TableNames { get; set; }
        =
            new List<string>()
            {
                "Pracownicy",
                "Zespoły",
                "Rodzaje Zatrudnienia",
                "Stanowiska",
                "Godziny Pracy",
                "Sprzęty",
                "Projekty",
                "Klienci",
                "Zadania",
                "Wpisy Pracy",
                "Miejsca"
            };

        private int actualTableNumber = 0;

        private object mainView = new Views.PracownicyView();
        public object MainView
        {
            get
            {
                switch (actualTableNumber)
                {
                    case 0:
                        return new Views.PracownicyView();
                    case 1:
                        return new Views.ZespołyView();
                    case 2:
                        return new Views.RodzajeZatrudnieniaView();
                    case 3:
                        return new Views.StanowiskaView();
                    case 4:
                        return new Views.GodzinyPracyView();
                    case 5:
                        return new Views.SprzętyView();
                    case 6:
                        return new Views.ProjektyView();
                    case 7:
                        return new Views.KlienciView();
                    case 8:
                        return new Views.ZadaniaView();
                    case 9:
                        return new Views.WpisyPracyView();
                    case 10:
                        return new Views.MiejscaView();
                    default:
                        return new Views.PracownicyView();
                };

            }
            set => OnPropertyChanged();
        }

        public string ActualTable
        {
            get => TableNames[actualTableNumber];
            set => OnPropertyChanged();
        }

        public void TableChange(int positionChange)
        {
            if(actualTableNumber + positionChange == -1)
            {
                actualTableNumber = TableNames.Count - 1;
            }
            else if (actualTableNumber + positionChange == TableNames.Count)
            {
                actualTableNumber = 0;
            }
            else
            {
                actualTableNumber += positionChange;
            }
            ActualTable = "";
            MainView = "";
        }


    }
}
