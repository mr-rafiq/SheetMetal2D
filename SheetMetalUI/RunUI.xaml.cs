using Rhino;
using SheetMetal2D.Commands;
using System;
using System.Windows;
using System.Windows.Controls;


namespace SheetMetal2D
{
    /// <summary>
    /// Interaction logic for RunUI.xaml
    /// </summary>
    public partial class RunUI : UserControl
    {
        private Rhino.RhinoDoc doc;
        private GlobalVar var;

        public RunUI()
        {
            InitializeComponent();

        }

        public void RunUIinitialise(ref GlobalVar var)
        {
            this.var = var;
        }
        private void Okay_Click(object sender, RoutedEventArgs e)
        {            
            //CrimpedEdge.Type1(doc);
        }

        public void SheetSection(object sender, RoutedEventArgs e)
        {
            
            doc = Rhino.RhinoDoc.ActiveDoc;
            //if (CheckIntegrity(this.var))
            //{ 
            SheetMetalCrossSection sh = new SheetMetalCrossSection();
            sh.SheetCrossSection(this.doc, ref this.var);
            //}
        }

        public void SheetFoiler(object sender, RoutedEventArgs e)
        {

            doc = Rhino.RhinoDoc.ActiveDoc;
            //if (CheckIntegrity(this.var))
            //{ 
            SheetFoiler foil = new SheetFoiler();
            foil.SheetMetalFoiler(this.doc, ref this.var);
            //}
        }
        public void SheetWeld(object sender, RoutedEventArgs e)
        {

            this.doc = Rhino.RhinoDoc.ActiveDoc;
            //if (CheckIntegrity(this.var))
            //{ 
            Welding weld = new Welding();
            weld.SheetMetalWelding(this.doc, ref this.var);
            //}
        }
        //private bool CheckIntegrity(object ob) { }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var Panelid = SheetMainDock.PanelId;

            Rhino.UI.Panels.ClosePanel(Panelid);
        }
    }
}
