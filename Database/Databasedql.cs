using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;
using System.Windows.Forms;

namespace SheetMetal2D
{
    public  class Databasedql
    {
        GlobalVar var;
        public Databasedql( GlobalVar var)
        {
            this.var = var;
        }
        public  List<MaterialBehaviour> GetMaterialBehaviour()
        {
            IDbConnection db = new SQLiteConnection("Data Source =" + SheetMetal2D.Properties.Resources.Databasepath);
            db.Open();
            //GlobalVar.MaterialNam = db.Query<String>("SELECT DISTINCT (MaterialName) from MaterialBehaviour").ToList();            
            return db.Query<MaterialBehaviour>("SELECT DISTINCT (MaterialName) from MaterialBehaviour").ToList();

        }

        public  List<MaterialVal> GetMaterialVal()
        {
            IDbConnection db = new SQLiteConnection("Data Source =" + SheetMetal2D.Properties.Resources.Databasepath);
            db.Open();
            return db.Query<MaterialVal>("SELECT Thikness from MaterialBehaviour WHERE MaterialBehaviour.MaterialName = " + "'"+ this.var.GetselectedMaterialname + "'").ToList();            
        }
    }





    public  class MaterialBehaviour
    {
                
        public string MaterialName { get; set; }
       
    }

    public class MaterialVal
    {
        //public int ID { get; set; }
        public int Thikness { get; set; }
    }



}
