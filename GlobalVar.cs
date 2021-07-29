using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetMetal2D
{

    #region Contains all the variable which are Global
    public class GlobalVar : object
    {
        public double Offsetthick { get; set; }
        

        //Change unit lable text
        public string Docunit { get; set; }
        public double Filletradius { get; set; }
        public double HatchDivision { get; set; }
        //Crimped Edge selection status
        public bool StartType { get; set; }
        public bool EndType { get; set; }
        public bool Starttype1 { get; set; }
        public  bool Endtype1 { get; set; }
        public  double StartHeight { get; set; }
        public  double StartLength { get; set; }
        public  double EndHeight { get; set; }
        public  double EndLength { get; set; }

        //Switch Toggle to flip sides of the edge

        public  double startflip { get; set; }
        public  double endflip { get; set; }

        public  string StartSwitch { get; set; }
        public  string EndSwitch { get; set; }


        //Database Values
        public  List<string> MaterialNam{ get; set; }
        public  List<string> MaterialVal { get; set; }
        public  string GetselectedMaterialname { get; set; }

    }
    #endregion


}
