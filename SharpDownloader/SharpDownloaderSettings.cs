using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpDownloader
{
    public class SharpDownloaderSettings
    {
        public int RefreshTime { get; set; }
        public int NumberOfProcessors { get; set; }
        public int TableCellWidth { get; set; }

        public SharpDownloaderSettings(int _RefreshTime=500,int _NumberOfProcessors=0,int TableCellWidth=77)
        {
            this.RefreshTime = _RefreshTime;
            if (_NumberOfProcessors < 0)
                throw new Exception("Number of processors can not be less than zero");
            this.NumberOfProcessors = _NumberOfProcessors == 0 ? Environment.ProcessorCount : _NumberOfProcessors;
            this.TableCellWidth = TableCellWidth;
        }


    }
}
