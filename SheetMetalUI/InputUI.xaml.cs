using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace SheetMetal2D
{
    /// <summary>
    /// Interaction logic for InputUI.xaml
    /// </summary>
    public partial class InputUI : UserControl
    {
        private GlobalVar var;
        private Databasedql sqldb;
      

        public InputUI()
        {
            InitializeComponent();
        }
        
        public void Initializ(ref GlobalVar var)
        {
            this.var = var;
            this.sqldb = new Databasedql(this.var);
            //Units display
            unit.Content = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            unit1.Content = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            unit2.Content = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();

            //Databasedql.Loaddb();           
            Material_combo.ItemsSource = this.sqldb.GetMaterialBehaviour();             
            Material_combo.DisplayMemberPath = "MaterialName";
            Material_combo.SelectedIndex = 0;
            this.var.GetselectedMaterialname = Material_combo.Text;
            MaterialThikness_combo.ItemsSource = this.sqldb.GetMaterialVal();
            MaterialThikness_combo.DisplayMemberPath = "Thikness";
            MaterialThikness_combo.SelectedIndex = 0;
            if (Material_combo.Text != null && this.var != null)
            {
                this.var.Offsetthick = double.Parse(MaterialThikness_combo.Text);
            }
            if (this.Distance_txtbox.Text != "" && this.var != null)
            { this.var.HatchDivision = double.Parse(this.Distance_txtbox.Text); }
        }

        private void NumberValidationTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!isInputNumber(e))
            {
                MessageBox.Show("Please enter only numbers!");
            }
            if ((e.Key == Key.Decimal) && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        public static bool isInputNumber(KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal ||
               e.Key == Key.Delete || e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.OemPeriod)
            {
                //Pressed Alt, ctrl, shift and other modifier keys
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                { e.Handled = true; }
                else
                { return true; }
            }
            else//Press other function keys such as characters
            { e.Handled = true; }
            return false;
        }

        public void Material_combo_DropDownClosed(object sender, EventArgs e)
        {

            //Get Material Thickness
            if (Material_combo.Text != "")
            {
                this.var.GetselectedMaterialname = Material_combo.Text;
                MaterialThikness_combo.ItemsSource = this.sqldb.GetMaterialVal();
                MaterialThikness_combo.DisplayMemberPath = "Thikness";
                MaterialThikness_combo.SelectedIndex = 0;
                this.var.Offsetthick = double.Parse(MaterialThikness_combo.Text);
            }
        }

        public void Input_values(object sender, RoutedEventArgs e)
        {

            if (this.Radius_txtbox.Text != "" && this.var != null)
            { this.var.Filletradius = double.Parse(this.Radius_txtbox.Text); }


            if (this.Material_combo.Text != "" && this.var != null)
            {
                this.var.GetselectedMaterialname = this.Material_combo.Text;
                if (this.MaterialThikness_combo.Text != "" && this.var != null)
                {
                    this.var.Offsetthick = double.Parse(this.MaterialThikness_combo.Text);
                }
            }



        }

        //private void btn_Input_values(object sender, EventArgs e)
        //{
        //    if (MaterialThikness_combo != null)
        //    { this.var.Offsetthick = double.Parse(MaterialThikness_combo.Text); }
        //}


    }
}
