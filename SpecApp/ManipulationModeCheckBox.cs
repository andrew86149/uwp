using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace SpecApp
{
    class ManipulationModeCheckBox : CheckBox
    {
        public ManipulationModes ManipulationModes { set; get; }
    }
}
