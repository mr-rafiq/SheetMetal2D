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
    /// Interaction logic for Abschluss.xaml
    /// </summary>
    public partial class Abschluss : UserControl
    {
        GlobalVar var;
        public Abschluss()
        {
            InitializeComponent();
        }

        public void initalstatus(ref GlobalVar var)
        {
            this.var = var;
            unit1.Content = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            unit2.Content = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            Endunit1.Content = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            Endunit2.Content = Rhino.RhinoDoc.ActiveDoc.ModelUnitSystem.ToString();
            StartChkbox.IsChecked = false;
            EndChkbox.IsChecked = false;
            Starttype1.IsEnabled = false;
            Starttype2.IsEnabled = false;
            Starttype3.IsEnabled = false;
            Starttype4.IsEnabled = false;
            Endtype1.IsEnabled = false;
            Endtype2.IsEnabled = false;
            Endtype3.IsEnabled = false;
            Endtype4.IsEnabled = false;
        }
        public void Check_stat(object sender, RoutedEventArgs e)
        {            
            if (StartChkbox.IsChecked.Value)
            {
                this.var.StartType = StartChkbox.IsChecked.Value;
                this.var.Starttype1 = Starttype1.IsChecked.Value;

                if (Startbtntoggle.IsChecked.Value)
                { this.var.startflip = -1.0; }
                else
                {
                    this.var.startflip = 1.0;
                }

            }
            else
            {
                this.var.StartType = StartChkbox.IsChecked.Value;
            }
            if (EndChkbox.IsChecked.Value)
            {
                this.var.EndType = EndChkbox.IsChecked.Value;
                this.var.Endtype1 = Endtype1.IsChecked.Value;

                if (Endbtntoggle.IsChecked.Value)
                { this.var.endflip = 1.0; }
                else
                {
                    this.var.endflip = -1.0;
                }

            }
            else
            {
                this.var.EndType = EndChkbox.IsChecked.Value;
            }



        }

        //Number restriction for text box
        #region Number Restrictions for text box
        private void NumberValidationTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!isInputNumber(e))
            {
                MessageBox.Show("Please enter only numbers!");
            }
            if ((e.Key == Key.Decimal) && ((sender as TextBox).Text.IndexOf('.') > -1) || ((sender as TextBox).Text.IndexOf('.') == 0))
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
                {
                    e.Handled = true;
                }
                else
                {
                    return true;
                }
            }
            else //Press other function keys such as characters
            {
                e.Handled = true;
            }
            return false;
        }
        //Number restriction for text box
        #endregion

        #region Checkbox Status Events
        private void StartChkbox_Checked(object sender, RoutedEventArgs e)
        {
            Starttype1.IsEnabled = true;
            Starttype2.IsEnabled = true;
            Starttype3.IsEnabled = true;
            Starttype4.IsEnabled = true;
            Starttype1.IsChecked = true;


        }
        private void StartChkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Starttype1.IsChecked = false;
            Starttype2.IsChecked = false;
            Starttype3.IsChecked = false;
            Starttype4.IsChecked = false;
            Starttype1.IsEnabled = false;
            Starttype2.IsEnabled = false;
            Starttype3.IsEnabled = false;
            Starttype4.IsEnabled = false;


        }
        private void EndChkbox_Checked(object sender, RoutedEventArgs e)
        {


            Endtype1.IsEnabled = true;
            Endtype2.IsEnabled = true;
            Endtype3.IsEnabled = true;
            Endtype4.IsEnabled = true;
            Endtype1.IsChecked = true;
        }
        private void EndChkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            Endtype1.IsEnabled = false;
            Endtype2.IsEnabled = false;
            Endtype3.IsEnabled = false;
            Endtype4.IsEnabled = false;
            Endtype1.IsChecked = false;
            Endtype2.IsChecked = false;
            Endtype3.IsChecked = false;
            Endtype4.IsChecked = false;
        }
        #endregion

        #region TextBox text events
        private void StartHeight_txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (StartHeight_txtbox.Text != "" && this.var != null)
            {
                this.var.StartHeight = Convert.ToDouble(StartHeight_txtbox.Text);
            }

        }
        private void StartLength_txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox a = (TextBox)sender;
            string b = a.Text;
            if (StartLength_txtbox.Text != "" && this.var!= null)
            {
                this.var.StartLength = Convert.ToDouble(StartLength_txtbox.Text);
            }

        }
        private void EndHeight_txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EndHeight_txtbox.Text != "" && this.var != null)
            {
                this.var.EndHeight = Convert.ToDouble(EndHeight_txtbox.Text);
            }
        }
        private void EndLength_txtbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EndLength_txtbox.Text != "" && this.var != null)
            {
                this.var.EndLength = Convert.ToDouble(EndLength_txtbox.Text);
            }
        }
        #endregion

        #region Toggle Buttons
        private void Endbtntoggle_Unchecked(object sender, RoutedEventArgs e)    //Button Toggle
        {
            Endbtntoggle.Content = "Inside";



        }
        private void Startbtntoggle_Unchecked(object sender, RoutedEventArgs e)       //Button Toggle
        {
            Startbtntoggle.Content = "Inside";

        }
        private void Startbtntoggle_Checked(object sender, RoutedEventArgs e)       //Button Toggle
        {
            Startbtntoggle.Content = "Outside";

        }
        private void Endbtntoggle_Checked(object sender, RoutedEventArgs e)            //Button Toggle
        {
            Endbtntoggle.Content = "Outside";
        }
        #endregion

        #region Start/End Type Check
        private void Starttype_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton ob = (RadioButton)sender;
            this.var.StartSwitch = ob.Name;
            if (ob.Name == "Starttype2")
            {
                StartHeight_txtbox.Text = StartLength_txtbox.Text;
                StartHeight_txtbox.IsEnabled = false;
            }
            else
            { StartHeight_txtbox.IsEnabled = true; }
            if (ob.Name == "Starttype4") 
            { StartLength_txtbox.IsEnabled = false; }
            else 
            { StartLength_txtbox.IsEnabled = true; }
            //End
        }
        private void Endtype_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton oe = (RadioButton)sender;
            this.var.EndSwitch = oe.Name;
            if (oe.Name == "Endtype2")
            {
                EndHeight_txtbox.Text = EndLength_txtbox.Text;
                EndHeight_txtbox.IsEnabled = false;
            }
            else
            {
                EndHeight_txtbox.IsEnabled = true;

            }
            if (oe.Name == "Endtype4")
            {
                EndLength_txtbox.IsEnabled = false;
            }
            else
            {
                EndLength_txtbox.IsEnabled = true;
            }

        }
        #endregion

    }
}
