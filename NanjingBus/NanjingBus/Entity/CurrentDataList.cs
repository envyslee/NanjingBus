using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanjingBus.Entity
{
    public class CurrentDataList
    {
        public string Distance { get; set; }

        public string uploadTime { get; set; }

        public string currentLevel { get; set; }

        public string busId { get; set; }

        public string busLong { get; set; }

        public string busSpeed
        {
            get;
            set;

        }

        public string busLat { get; set; }

        public string reloadTime { get; set; }
    }
}
