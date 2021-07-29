using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SheetMetal2D
{
    /// <summary>
    /// Interaction logic for WeldingUI.xaml
    /// </summary>
    public partial class WeldingUI : UserControl
    {
        public GlobalVar var = new GlobalVar();
        public WeldingUI()
        {
            InitializeComponent();
            InputUI inputui = new InputUI();
            GeometryParametersUI geom = new GeometryParametersUI();
            geom.Fillbtn.Visibility = Visibility.Hidden;
            inputui.Initializ(ref this.var);
            this.InputUIPanel.Children.Add(inputui);
            this.GeomparamPanel.Children.Add(geom);
        }
    }
}
