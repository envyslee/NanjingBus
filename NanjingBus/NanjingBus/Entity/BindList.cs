using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NanjingBus.Entity
{
    public class BindList:INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string str)
        {
            if (PropertyChanged!=null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(str));
            }
        }

        private string busId;

        public string BusId
        {
            get
            {
                return busId;
            }
            set
            {
                busId = value;
                OnPropertyChanged("BusId");
            }
        }

        private string currentLevel;

        public string CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                currentLevel = value;
                OnPropertyChanged("CurrentLevel");
            }
        }

        private string busSpeed;

        public string BusSpeed
        {
            get
            {
                return busSpeed;
            }
            set
            {
                busSpeed = value;
                OnPropertyChanged("BusSpeed");
            }
        }

        private string stationName;

        public string StationName
        {
            get
            {
                return stationName;
            }
            set
            {
                stationName = value;
                OnPropertyChanged("StationName");
            }
        }

        private Visibility _visibility=Visibility.Collapsed;

        public Visibility Visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged("Visibility");
            }
        }
    }
}
