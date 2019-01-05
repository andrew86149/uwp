using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace SpecApp
{
    public class FolderItem
    {
        public StorageFolder StorageFolder { set; get; }

        public int Level { set; get; }

        public string Indent
        {
            get { return new string('\x00A0', this.Level * 4); }
        }

        public Grid DisplayGrid { set; get; }
    }
}
