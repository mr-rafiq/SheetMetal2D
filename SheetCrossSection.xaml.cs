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
    /// Interaction logic for SheetCrossSection.xaml
    /// </summary>
    public partial class SheetCrossSection : UserControl
    {
        public GlobalVar var = new GlobalVar();
        public SheetCrossSection()
        {
            InitializeComponent();
            InputUI input = new InputUI();
            input.Initializ(ref var);            
            GeometryParametersUI geom = new GeometryParametersUI();
            Abschluss ab = new Abschluss();
            ab.initalstatus(ref var);
            this.InputUIPanel.Children.Add(input);
            this.AbschlussPanel.Children.Add(ab);
            this.GeomparamPanel.Children.Add(geom);
        }
    }
}
