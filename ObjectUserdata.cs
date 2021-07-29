using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.FileIO;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SheetMetal2D
{
    public static class ObjectUserdata
    {
        public static RhinoList<Guid> SheetCrossSectionID = new RhinoList<Guid>();
        public static RhinoList<Guid> SheetCrossSectionOffsetID = new RhinoList<Guid>();
        public static RhinoList<Guid> FoilerID = new RhinoList<Guid>();
    }

    [Guid("DAAA9791-01DB-4F5F-B89B-4AE46767C783")]
    public class CustomUserData : UserData
    {
        #region Private constants

        /// <summary>
        /// The major and minor version number of this data.
        /// </summary>
        private const int MAJOR_VERSION = 1;
        private const int MINOR_VERSION = 0;

        #endregion

        #region Public properties
        /// <summary>
        /// The notes field
        /// </summary>
        public string Name { get; set; }
        public Curve BaseCurve { get; set; }
        public RhinoList<Point3d> UpdatePoints { get; set; }

        /// <summary>
        /// Returns true of our user data is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return !string.IsNullOrEmpty(this.Name) && this.UpdatePoints.Count > 0 && this.BaseCurve.GetLength() > 0;
            }
        }
        #endregion

        #region Userdata overrides

        /// <summary>
        /// Descriptive name of the user data.
        /// </summary>
        public override string Description => this.Name;

        public override string ToString() => String.Format("BaseCurve={0}, UpdatePoints={1}, Name={2}", this.BaseCurve.ToString(), this.UpdatePoints.Count().ToString(), this.Name);

        protected override void OnDuplicate(UserData source)
        {
            if (source is CustomUserData src)
            {
                Name = src.Name;
                UpdatePoints = src.UpdatePoints;
                BaseCurve = src.BaseCurve;
            }

        }


        protected override void OnTransform(Transform transformation)
        {
           

            if (this.UpdatePoints != null)
            {
                var doc = Rhino.RhinoDoc.ActiveDoc;
                RhinoList<Point3d> pts = new RhinoList<Point3d>();
                Rhino.RhinoApp.WriteLine(transformation.ToString());
                foreach (Point3d pt in this.UpdatePoints)
                {
                    pt.Transform(transformation);
                    pts.Add(pt);
                }
                this.UpdatePoints.Clear();
                this.UpdatePoints.AddRange(pts);
                Curve basecrv = this.BaseCurve.DuplicateCurve();
                basecrv.Transform(transformation);
                BaseCurve = basecrv;
                base.OnTransform(transformation);
                doc.Objects.UnselectAll();
            }    

        }

        public override bool ShouldWrite
        {
            get
            {
                return IsValid;
            }
        }
        protected override bool Read(BinaryArchiveReader archive)
        {
            // Read the chuck version
            archive.Read3dmChunkVersion(out var major, out var minor);
            if (major == MAJOR_VERSION)
            {
                // Read 1.0 fields  here
                if (minor >= MINOR_VERSION)
                {
                    this.Name = archive.ReadString();
                    int nPoints = archive.ReadInt();
                    RhinoList<Point3d> points = new RhinoList<Point3d>(nPoints);
                    for (int i = 0; i < nPoints; ++i)
                    { points.Add(archive.ReadPoint3d()); }
                    this.UpdatePoints = points;
                    this.BaseCurve = (Curve)archive.ReadGeometry();
                }
                // Note, if you every roll the minor version number,
                // then read those fields here.
            }
            return !archive.ReadErrorOccured;
        }

        /// <summary>
        /// Writes the content of this data to a stream archive.
        /// </summary>
        protected override bool Write(BinaryArchiveWriter archive)
        {
            // Write the chuck version
            archive.Write3dmChunkVersion(MAJOR_VERSION, MINOR_VERSION);

            // Write 1.0 fields
            archive.WriteString(this.Name);
            // defined somewhere else
            int nPoints = this.UpdatePoints.Count;
            archive.WriteInt(nPoints);
            for (int i = 0; i < nPoints; ++i)
            { archive.WritePoint3d(this.UpdatePoints[i]); }
            archive.WriteGeometry(this.BaseCurve);
            // Note, if you every roll the minor version number,
            // then write those fields here.
            return !archive.WriteErrorOccured;
        }

        #endregion
    }

    public class AttributeIds
    {
        public string CategoryName;
        public string MaterialName;
        public double Thickness;
        public double FilletRadius;
        public double FoilerDivision;
        public bool StartType;
        public bool EndType;
        public string SType;
        public string EType;
        public System.Drawing.Color Objcolor;
        public ObjectAttributes SetSheetCrossSectionAttributes;
        public ObjectAttributes SetFoilerAttributes;

        public AttributeIds() { }
        public AttributeIds(string CategoryName, string MaterialName, double Thickness, double FilletRadius, bool StartType, string SType, bool EndType, string EType, System.Drawing.Color Objcolor)
        {
            this.CategoryName = CategoryName;
            this.MaterialName = MaterialName;
            this.Thickness = Thickness;
            this.FilletRadius = FilletRadius;
            this.StartType = StartType;
            this.SType = SType;
            this.EndType = EndType;
            this.EType = EType;
            this.Objcolor = Objcolor;
            this.SetSheetCrossSectionAttributes = SheetCrossSectionDefaultribute();

        }

        public AttributeIds(string CategoryName, string MaterialName, double Thickness, double FilletRadius, double FoilerDivision, System.Drawing.Color Objcolor)
        {
            this.CategoryName = CategoryName;
            this.MaterialName = MaterialName;
            this.Thickness = Thickness;
            this.FilletRadius = FilletRadius;
            this.FoilerDivision = FoilerDivision;
            this.Objcolor = Objcolor;
            this.SetFoilerAttributes = FoilerDefaultribute();

        }

        public ObjectAttributes SheetCrossSectionDefaultribute()
        {
            var doc = RhinoDoc.ActiveDoc;

            ObjectAttributes dc = Rhino.RhinoDoc.ActiveDoc.CreateDefaultAttributes();

            //dc.ObjectColor = Rhino.RhinoDoc.ActiveDoc.Layers.CurrentLayer.Color;
            dc.Name = CategoryName;
            dc.UserDictionary.Set("Name", this.MaterialName);
            dc.UserDictionary.Set("Thickness", this.Thickness);
            dc.UserDictionary.Set("FilletRadius", this.FilletRadius);
            if (this.StartType == false) { this.StartType = false; }
            if (this.SType == null) { this.SType = "null"; }
            if (this.EndType == false) { this.EndType = false; }
            if (this.EType == null) { this.EType = "null"; }

            dc.UserDictionary.Set("StartType", this.StartType);
            dc.UserDictionary.Set("SType_Name", this.SType);
            dc.UserDictionary.Set("EndType", this.EndType);
            dc.UserDictionary.Set("EType_Name", this.EType);
            dc.ObjectColor = Objcolor;
            dc.PlotColor = System.Drawing.Color.Black;
            ObjectLayers layer = new ObjectLayers(this.CategoryName, this.Objcolor);
            dc.LayerIndex = layer.Check_AddLayer();
            return dc;
        }

        public ObjectAttributes FoilerDefaultribute()
        {
            var doc = RhinoDoc.ActiveDoc;

            ObjectAttributes dc = Rhino.RhinoDoc.ActiveDoc.CreateDefaultAttributes();
            dc.Name = CategoryName;
            dc.UserDictionary.Set("Name", this.MaterialName);
            dc.UserDictionary.Set("Thickness", this.Thickness);
            dc.UserDictionary.Set("FilletRadius", this.FilletRadius);
            dc.UserDictionary.Set("HatchDivision", this.FoilerDivision);
            dc.ObjectColor = Objcolor;
            dc.PlotColor = System.Drawing.Color.Black;
            ObjectLayers layer = new ObjectLayers(this.CategoryName, this.Objcolor);
            dc.LayerIndex = layer.Check_AddLayer();
            return dc;
        }
    }

    public class ObjectLayers
    {
        private RhinoDoc doc = RhinoDoc.ActiveDoc;

        public string Layername;
        public System.Drawing.Color Layercolor;
        public ObjectLayers(string Layername, System.Drawing.Color Layercolor)
        {
            this.Layername = Layername;
            this.Layercolor = Layercolor;

        }

        public int Check_AddLayer()
        {
            //layer name
            string layer_name = this.Layername;
            // Does a layer with the same name already exist?
            int layer_index = this.doc.Layers.Find(layer_name, true);
            if (layer_index >= 0)  {  return layer_index; }
            else
            {
                layer_index = this.doc.Layers.Add(layer_name, Layercolor);
                return layer_index;
            }
        }
    }

}
