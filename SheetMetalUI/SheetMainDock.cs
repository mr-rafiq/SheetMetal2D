using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;

namespace SheetMetal2D
{
    [Guid("213E4328-CA66-43C0-AC6A-EB8F3468552D")]
    [CommandStyle(Rhino.Commands.Style.ScriptRunner)]
    public partial class SheetMainDock : UserControl
    {
        public GlobalVar var = new GlobalVar();

        #region SheetCrossSection
        public InputUI Inputnew { get; set; }
        public string Materialname;
        public double Thickness;
        public double Filletrad;
        #endregion


        public RhinoObject selectedobject { get; set; }

        public static Guid PanelId => typeof(SheetMainDock).GUID;
        public  Curve PassCurve { get; set; }
        public  RhinoList<Point3d> DirectionPoints = new RhinoList<Point3d>();

        public SheetMainDock()
        {
            InitializeComponent();
            this.runUI1.Okaybtn.Visibility = Visibility.Hidden;
            this.runUI1.Cancelbtn.Content = "Close";
            Command.BeginCommand += Onresetcommand;
            RhinoDoc.SelectObjects += Updateobjdata;
            RhinoDoc.DeselectAllObjects += OnDeselectAllObjects;
            RhinoDoc.DeselectObjects += OnDeselectObjects;
            
        }


        private void SheetMainDock_Load(object sender, EventArgs e){ }
        public void Updateobjdata(object sender, RhinoObjectSelectionEventArgs ea)
        {
            
            var dc = Rhino.RhinoDoc.ActiveDoc;
            
            var selectedobj = ea.RhinoObjects.ToList();
            if (ea.Selected)
            {
                if (selectedobj[0].Name == "SheetCrossSection")
                {
                    SheetCrossSection sheetcross = new SheetCrossSection();
                    IEnumerator inputuip = sheetcross.InputUIPanel.Children.GetEnumerator();
                    inputuip.Reset();
                    _ = inputuip.MoveNext();
                    InputUI inputs = (InputUI)inputuip.Current;
                    Inputnew = inputs;
                    inputs.Material_combo.Text = selectedobj[0].Attributes.UserDictionary.Values[0].ToString();
                    Databasedql sqldb = new Databasedql(this.var);                    
                    inputs.MaterialThikness_combo.ItemsSource = sqldb.GetMaterialVal();
                    inputs.MaterialThikness_combo.DisplayMemberPath = "Thikness";
                    inputs.MaterialThikness_combo.Text = selectedobj[0].Attributes.UserDictionary.Values[1].ToString();
                    inputs.Radius_txtbox.Text = selectedobj[0].Attributes.UserDictionary.Values[2].ToString();
                    CustomUserData ud = selectedobj[0].Attributes.UserData.Find(typeof(CustomUserData)) as CustomUserData;
                    this.PassCurve = ud.BaseCurve;
                    this.DirectionPoints = ud.UpdatePoints;
                    selectedobject = selectedobj[0];
                    RunUI run = new RunUI();
                    run.Okaybtn.Content = "Update";
                    this.Materialname = inputs.Material_combo.Text;
                    this.Thickness = double.Parse(inputs.MaterialThikness_combo.Text);
                    this.Filletrad = double.Parse(inputs.Radius_txtbox.Text);
                    //run.RunUIinitialise(ref this.var);
                    inputs.Material_combo.DropDownClosed += NewInput;
                    inputs.MaterialThikness_combo.DropDownClosed += NewInput;
                    run.Okaybtn.Click += inputs.Input_values;
                    run.Okaybtn.Click += UpdateObj;
                    this.elementHost1.Child = run;
                    this.elementHost2.Child = sheetcross;
                    
                    Guid Panelid = PanelId;
                    Rhino.UI.Panels.OpenPanel(Panelid);
                    RhinoApp.WriteLine(selectedobj[0].Name +" - "+ selectedobj[0].Attributes.UserDictionary.Values[0].ToString()+ " Selected");
                }
            }
            else { }
        }



        //public void inputsui(RhinoObject selectedobj)
        //{
        //    SheetCrossSection sheetcross = new SheetCrossSection();
        //    Databasedql sqldb = new Databasedql(this.var);
        //    IEnumerator inputuip = sheetcross.InputUIPanel.Children.GetEnumerator();
        //    inputuip.Reset();
        //    _ = inputuip.MoveNext();
        //    InputUI inputs = (InputUI)inputuip.Current;
        //    inputs.Material_combo.Text = selectedobj.Attributes.UserDictionary.Values[0].ToString();            

        //    inputs.MaterialThikness_combo.ItemsSource = sqldb.GetMaterialVal();
        //    inputs.MaterialThikness_combo.DisplayMemberPath = "Thikness";
        //    inputs.MaterialThikness_combo.Text = selectedobj.Attributes.UserDictionary.Values[1].ToString();
        //    inputs.Radius_txtbox.Text = selectedobj.Attributes.UserDictionary.Values[2].ToString();
        //    this.var.GetselectedMaterialname = inputs.Material_combo.Text;
        //    this.Materialname = inputs.Material_combo.Text;
        //    this.Thickness = double.Parse(inputs.MaterialThikness_combo.Text);
        //    this.Filletrad = double.Parse(inputs.Radius_txtbox.Text);
        //    this.selectedcurve = (Curve)Rhino.Runtime.CommonObject.FromJSON((string)selectedobj.Attributes.UserDictionary.Values[7]);
        //    Point3d pt;
        //    foreach (var item in (RhinoList<string>)selectedobj.Attributes.UserDictionary.Values[8])
        //    {
        //        bool trypoint = Point3d.TryParse(item, out pt);
        //        if (trypoint) { offsetpoints.Add(pt); }
        //    }
        //    RunUI run = new RunUI();
            
        //    run.Okaybtn.Content = "Update";
        //    run.Okaybtn.Click += UpdateObj;
        //    this.elementHost1.Child = run;
        //    this.elementHost2.Child = sheetcross;
        //    Guid Panelid = SheetMainDock.PanelId;
        //    Rhino.UI.Panels.OpenPanel(Panelid);
        //    RhinoApp.WriteLine(selectedobj.Name + " - " + selectedobj.Attributes.UserDictionary.Values[0].ToString() + " Selected");

        //}



        #region Update Object
        public void UpdateObj(object sender, RoutedEventArgs e)
        {
            
            var doc = RhinoDoc.ActiveDoc;
            //Param param = new Param(this.selectedcurve,this.offsetpoints,Filletrad,Thickness,doc,this.var);
            Param param = new Param(this.PassCurve, this.DirectionPoints, this.Filletrad, this.Thickness, doc, this.var);
            AttributeIds attid = new AttributeIds("SheetCrossSection", this.Materialname, this.Thickness, this.Filletrad, this.var.StartType, this.var.StartSwitch, this.var.EndType, this.var.EndSwitch, System.Drawing.Color.DarkOrange);
            Curve paramcurv = param.SheetMetalCurve;
            CustomUserData csudata = new CustomUserData
            {
                BaseCurve = this.PassCurve,
                UpdatePoints = this.DirectionPoints,
                Name = "SheetCrossSection"
            };
            attid.SetSheetCrossSectionAttributes.UserData.Add(csudata);
            doc.Objects.AddCurve(paramcurv, attid.SetSheetCrossSectionAttributes);
            doc.Objects.Delete(selectedobject);
            doc.Views.Redraw();
        }
        #endregion

        #region All UI reset
        public void Onresetcommand(object sender, CommandEventArgs e)
        {
            if (e.CommandEnglishName == "SheetMetal2DCommand")
            { Resetpaneltype(); }
        }
        public  void OnDeselectObjects(object sender, Rhino.DocObjects.RhinoObjectSelectionEventArgs e)
        {
            EmptyPanel empty = new EmptyPanel();
            RunUI run = new RunUI();
            System.Drawing.Bitmap papel_icon = Properties.Resources.papel_icon;
            this.elementHost2.Child = empty;
            run.Okaybtn.Visibility = Visibility.Hidden;
            run.Cancelbtn.Content = "Close";
            this.elementHost1.Child = run;

        }
        public  void OnDeselectAllObjects(object sender, Rhino.DocObjects.RhinoDeselectAllObjectsEventArgs e)
        {
            EmptyPanel empty = new EmptyPanel();
            RunUI run = new RunUI();
            run.Okaybtn.Visibility = Visibility.Hidden;
            run.Cancelbtn.Content = "Close";
            this.elementHost2.Child = empty;
            this.elementHost1.Child = run;
            

        }
   
        public void Resetpaneltype()
        {
            var dc = Rhino.RhinoDoc.ActiveDoc;
            string str = String.Empty;
            RhinoGet.GetString("PanelName", true, ref str);
            var gt = new GetOption();
            gt.SetCommandPrompt("PanelName");
            int SheetMetal = gt.AddOption("SheetMetal");
            int Foiler = gt.AddOption("Foiler");
            int Welding = gt.AddOption("Welding");
            int index = gt.OptionIndex();

            switch (str)
            {
                case "SheetMetal":
                    SheetUireset(dc);
                    break;

                case "Foiler":
                    FoilerUireset(dc);
                    break;

                case "Welding":
                    WeldingUireset(dc);
                    break;
            }

        }
    
        public void SheetUireset (RhinoDoc dc)
        {
            var selectedobj = dc.Objects.UnselectAll();
            SheetCrossSection sh = new SheetCrossSection();
            RunUI run = new RunUI();
            IEnumerator shuc = sh.AbschlussPanel.Children.GetEnumerator();
            shuc.Reset();
            shuc.MoveNext();
            IEnumerator shinput = sh.InputUIPanel.Children.GetEnumerator();
            shinput.Reset();
            shinput.MoveNext();
            Abschluss abs = (Abschluss)shuc.Current;
            InputUI inpt = (InputUI)shinput.Current;
            this.elementHost2.Child = sh;
            this.elementHost1.Child = run;
            inpt.Initializ(ref this.var);
            abs.initalstatus(ref this.var);
            run.Okaybtn.Click += abs.Check_stat;
            run.Okaybtn.Click += inpt.Input_values;
            run.Okaybtn.Click += run.SheetSection;
            run.RunUIinitialise(ref this.var);
        }

        public void FoilerUireset(RhinoDoc dc)
        {
            var selectedobj = dc.Objects.UnselectAll();
            Foiler foil = new Foiler();
            RunUI run = new RunUI();
            IEnumerator shinput = foil.InputUIPanel.Children.GetEnumerator();
            shinput.Reset();
            shinput.MoveNext();
            InputUI inpt = (InputUI)shinput.Current;
            inpt.Title.Content = "Foiler";
            inpt.Distance.Content = "Division";
            this.elementHost2.Child = foil;
            this.elementHost1.Child = run;
            inpt.Initializ(ref this.var);
            run.Okaybtn.Click += inpt.Input_values;
            run.Okaybtn.Click += run.SheetFoiler;
            run.RunUIinitialise(ref this.var);
        }

        public void WeldingUireset(RhinoDoc dc)
        {
            var selectedobj = dc.Objects.UnselectAll();
            WeldingUI weld = new WeldingUI();
            RunUI run = new RunUI();
            IEnumerator shinput = weld.InputUIPanel.Children.GetEnumerator();
            shinput.Reset();
            shinput.MoveNext();
            InputUI inpt = (InputUI)shinput.Current;
            inpt.Distance_txtbox.Text = "0.5";
            inpt.Title.Content = "Welding";
            inpt.Distance.Content = "Division";
            this.elementHost2.Child = weld;
            this.elementHost1.Child = run;
            inpt.Initializ(ref this.var);
            run.Okaybtn.Click += inpt.Input_values;
            run.Okaybtn.Click += run.SheetWeld;
            run.RunUIinitialise(ref this.var);
        }
        #endregion


        public void NewInput(object sender, EventArgs e)
        {

            this.Materialname = Inputnew.Material_combo.Text;
            this.Thickness = double.Parse(Inputnew.MaterialThikness_combo.Text);
            this.Filletrad = double.Parse(Inputnew.Radius_txtbox.Text);

        }
        
    }







}
