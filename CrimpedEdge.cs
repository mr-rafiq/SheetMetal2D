
using Newtonsoft.Json;
using Rhino;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SheetMetal2D
{
    //public class CrimpedEdge
    //{

    //    public static Curve Basecurve(double len, double width, Plane p)
    //    {
    //        Interval intwd = new Interval(width, 0);
    //        Interval intht = new Interval(len, 0);
    //        Rectangle3d rec = new Rectangle3d(p, intht, intwd);

    //        Curve ccx = rec.ToNurbsCurve();

    //        Curve[] c = ccx.DuplicateSegments();

    //        RhinoList<Point3d> pt = new RhinoList<Point3d>();
    //        foreach (Curve ln in c)
    //        {
    //            pt.Add(ln.PointAtStart);

    //        }
    //        var pl = new PolylineCurve(pt);
    //        return pl;
    //    }


    //    public static Curve Type1(Curve inputcurv,double framelen,double rotateplan, CurveEnd trimtype, double len, double width, double tol, double angltol, double flip)
    //    {

    //        var curve_copy = inputcurv.DuplicateCurve();
    //        curve_copy.Domain = new Interval(0, 1);

    //        curve_copy.PerpendicularFrameAt(framelen, out Plane pl);

    //        pl.Rotate(rotateplan, pl.YAxis);
    //        pl.Rotate(4.71239, flip * pl.XAxis);

    //        var poly = curve_copy.Trim(trimtype, len);


    //        var basecurve = Basecurve(len, width, pl);

    //        var fill = Curve.CreateFilletCornersCurve(basecurve, width / 2, tol, angltol);
    //        RhinoList<Curve> cr = new RhinoList<Curve>();
    //        cr.Add(poly);
    //        cr.Add(fill);
    //        var jn = Curve.JoinCurves(cr);
    //        return jn[0];

    //    }

    //    public static Curve[] Type2(Curve inputcurv, double framelen, double rotateplan, CurveEnd trimtype, double len, double width, double tol, double angltol, double flip)
    //    {
    //        var curve_copy = inputcurv.DuplicateCurve();
    //        curve_copy.Domain = new Interval(0, 1);

    //        curve_copy.PerpendicularFrameAt(framelen, out Plane pl);

    //        pl.Rotate(rotateplan, pl.YAxis);
    //        pl.Rotate(4.71239, flip * pl.XAxis);

    //        var poly = curve_copy.Trim(trimtype, len);
    //        var basecurve = Basecurve(len, width, pl);

    //        var lin = CurveManupulationClass.LinefromCrv(basecurve);

    //        RhinoList<Curve> typcrv = new RhinoList<Curve>();
    //        typcrv.Add(basecurve);
    //        typcrv.Add(lin.ToNurbsCurve());
    //        var basejoin = Curve.JoinCurves(typcrv);
    //        Curve cc = basejoin[0].ToNurbsCurve();            
    //        var trcrv = cc.Trim(len / 3, 0.0);

    //        var fill = Curve.CreateFilletCornersCurve(trcrv, width / 2, tol, angltol);


    //        RhinoList<Curve> cr = new RhinoList<Curve>();
    //        cr.Add(poly);
    //        cr.Add(fill);
    //        var jn = Curve.JoinCurves(cr);
    //        return jn;

    //    }

    //    public static Curve Type3(Curve inputcurv, double framelen, double rotateplan, CurveEnd trimtype, double len, double width, double tol, double angltol, double flip)
    //    {
    //        var curve_copy = inputcurv.DuplicateCurve();
    //        curve_copy.Domain = new Interval(0, 1);

    //        curve_copy.PerpendicularFrameAt(framelen, out Plane pl);

    //        pl.Rotate(rotateplan, pl.YAxis);
    //        pl.Rotate(4.71239, flip * pl.XAxis);

    //        Curve poly = curve_copy.Trim(trimtype, len);
    //        Curve basecurve = Basecurve(len, width, pl);

    //        var fill = Curve.CreateFilletCornersCurve(basecurve, len / 4, tol, angltol);
    //        //Rhino.RhinoDoc.ActiveDoc.Objects.Add(fill);
    //        RhinoList<Curve> cr = new RhinoList<Curve>();
    //        cr.Add(poly);
    //        cr.Add(fill);
    //        var jn = Curve.JoinCurves(cr);


    //        var addtyp1 = CrimpedEdge.Type1(jn[0], framelen, rotateplan, trimtype, len/2, width/2, tol, angltol, flip);

    //        //Rhino.RhinoDoc.ActiveDoc.Objects.Add(addtyp1);
    //        return addtyp1;

    //    }

    //    //public static void Type1(RhinoDoc doc)
    //    //{
    //    //    string myJsonResponse = Properties.Resources.ConfigJson;
    //    //    Root Termedge = JsonConvert.DeserializeObject<Root>(File.ReadAllText(myJsonResponse));

    //    //    var pt = new RhinoList<Point3d>();
    //    //    var wt = new RhinoList<Double>();
    //    //    var kt = new RhinoList<Double>();
    //    //    int dimension = 2;
    //    //    bool isRational = false;
    //    //    int order = 3;
    //    //    int cv_count = Termedge.Termination.Type1.Points.Count;
    //    //    Rhino.Geometry.NurbsCurve nc = new Rhino.Geometry.NurbsCurve(dimension, isRational, order, cv_count);

    //    //    for (int i = 0; i<Termedge.Termination.Type1.Points.Count;i++)
    //    //    {
    //    //       pt.Add(new Point3d(Termedge.Termination.Type1.Points[i][0], Termedge.Termination.Type1.Points[i][1], 0));
    //    //        nc.Points.SetPoint(i, new Point3d(Termedge.Termination.Type1.Points[i][0], Termedge.Termination.Type1.Points[i][1], 0));
    //    //    }

    //    //    for (int i = 0; i < Termedge.Termination.Type1.Weight.Count; i++)
    //    //    {
    //    //        wt.Add(Termedge.Termination.Type1.Weight[i]);
    //    //        nc.Points.SetWeight(i, Termedge.Termination.Type1.Weight[i]);

    //    //    }
    //    //    for (int i = 0; i < Termedge.Termination.Type1.Knots.Count; i++)
    //    //    {
    //    //        kt.Add(Termedge.Termination.Type1.Knots[i]);
    //    //        nc.Knots[i] = Termedge.Termination.Type1.Knots[i];

    //    //    }


    //    //    doc.Objects.AddCurve(nc);


    //    //}

    //    //    public static Surface termsurf(double len , double width, Plane p)
    //    //    {

    //    //        //Vector3d vec = ((Vector3d)pto);
    //    //        //Plane p = new Plane(pto, vec);
    //    //        Interval intwd = new Interval(width / 2, -width / 2);
    //    //        Interval intht = new Interval(0, len);
    //    //        Rectangle3d rec = new Rectangle3d(p, intht, intwd);
    //    //        Rhino.RhinoDoc.ActiveDoc.Objects.AddRectangle(rec);
    //    //        Brep[] surf = Brep.CreatePlanarBreps(rec.ToNurbsCurve(), RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);

    //    //        Interval invl = new Interval(0, 1);
    //    //        surf[0].Surfaces[0].SetDomain(0, invl);
    //    //        surf[0].Surfaces[0].SetDomain(1, invl);

    //    //        return surf[0].Surfaces[0];

    //    //    }

    //    //    public static RhinoList<Curve> Type1(Surface surf)
    //    //    {

    //    //        string myJsonResponse = Properties.Resources.ConfigJson;
    //    //        Jsonobject Termedge = JsonConvert.DeserializeObject<Jsonobject>(File.ReadAllText(myJsonResponse));

    //    //        RhinoList<Curve> curves = new RhinoList<Curve>();
    //    //        for (int i = 0; i < Termedge.Termination.Type1.Linept1.Length; i++)
    //    //        {
    //    //            Interval invl = new Interval(0, 1);
    //    //            surf.Evaluate(Termedge.Termination.Type1.Linept1[i][0], Termedge.Termination.Type1.Linept1[i][1],1, out Point3d ept1, out Vector3d[] vec);
    //    //            surf.Evaluate(Termedge.Termination.Type1.Linept2[i][0], Termedge.Termination.Type1.Linept2[i][1], 1, out Point3d ept2, out vec);

    //    //            Line line = new Line(ept1, ept2);
    //    //            //doc.Objects.AddLine(line);
    //    //            curves.Add(line.ToNurbsCurve());
    //    //        }

    //    //        for (int i = 0; i < Termedge.Termination.Type1.Arcpt1.Length; i++)
    //    //        {

    //    //            Point3d ept1;
    //    //            Point3d ept2;
    //    //            Point3d ept3;
    //    //            Vector3d[] vec;
    //    //            Interval invl = new Interval(0, 1);
    //    //            surf.Evaluate(Termedge.Termination.Type1.Arcpt1[i][0], Termedge.Termination.Type1.Arcpt1[i][1], 1, out ept1, out vec);
    //    //            surf.Evaluate(Termedge.Termination.Type1.Arcpt2[i][0], Termedge.Termination.Type1.Arcpt2[i][1], 1, out ept2, out vec);
    //    //            surf.Evaluate(Termedge.Termination.Type1.Arcpt3[i][0], Termedge.Termination.Type1.Arcpt3[i][1], 1, out ept3, out vec);

    //    //           //doc.Objects.AddArc(new Arc(ept1, ept2, ept3));
    //    //            var arcs = new Arc(ept1, ept2, ept3);
    //    //            curves.Add(arcs.ToNurbsCurve());
    //    //        }
    //    //        return curves;
    //    //    }


    //    //}




    //    //public class Jsonobject
    //    //{
    //    //    public Termination Termination { get; set; }
    //    //}

    //    //public class Termination
    //    //{
    //    //    public Type1 Type1 { get; set; }
    //    //}

    //    //public class Type1
    //    //{
    //    //    public double[][] Linept1 { get; set; }
    //    //    public double[][] Linept2 { get; set; }
    //    //    public double[][] Arcpt1 { get; set; }
    //    //    public double[][] Arcpt2 { get; set; }
    //    //    public double[][] Arcpt3 { get; set; }
    //    //}










    //    //public class Type1
    //    //{
    //    //    [JsonProperty("points")]
    //    //    public List<List<double>> Points { get; set; }

    //    //    [JsonProperty("weight")]
    //    //    public List<double> Weight { get; set; }

    //    //    [JsonProperty("knots")]
    //    //    public List<double> Knots { get; set; }
    //    //}

    //    //public class Termination
    //    //{
    //    //    [JsonProperty("Type1")]
    //    //    public Type1 Type1 { get; set; }
    //    //}

    //    //public class Root
    //    //{
    //    //    [JsonProperty("Termination")]
    //    //    public Termination Termination { get; set; }
    //    //}

    //}

    #region Bouding Box Class for the Crimped Edge
    public class Edgebase
    {
        public double len;
        public double width;
        public Plane pln { get; set; }
        public PolylineCurve plcrv { get; set; }

        public Curve inputcurv;
        public double framelen;
        public double rotateplane { get; set; }
        public CurveEnd trimtype;
        public double tolerance;
        public double angltolerance;
        public double flip;
        public double materialthickness;

        //Check For the End or Start type to get the default angle
        public double checktype()
        {
            if (this.framelen > 0) {
                return rotateplane = 1.5708;
            }
            else
            {
                return rotateplane = 4.71239;
            }

        }
        //Constructor
        public Edgebase(Curve inputcurve, double Bboxpos, CurveEnd trimend, double BBoxlength, double BBoxwidth, double modltolerance, double modlangltolerance, double flipside, double materialthickness)
        {
            inputcurv = inputcurve;
            framelen = Bboxpos;
            trimtype = trimend;
            len = BBoxlength;
            width = BBoxwidth;
            tolerance = modltolerance;
            angltolerance = modlangltolerance;
            flip = flipside;
            this.materialthickness = materialthickness;
        }

        //Base bounding box for the all types except type4
        public Curve Basecurve(double width, double len)
        {
            Interval intwd = new Interval(width, 0);
            Interval intht = new Interval(len, 0);
            Rectangle3d rec = new Rectangle3d(pln, intht, intwd);

            Curve ccx = rec.ToNurbsCurve();

            Curve[] c = ccx.DuplicateSegments();

            RhinoList<Point3d> pt = new RhinoList<Point3d>();
            foreach (Curve ln in c)
            {
                pt.Add(ln.PointAtStart);

            }
            plcrv = new PolylineCurve(pt);
            return plcrv;
        }

        //Base bounding box for type4
        public static Rectangle3d rect(double len, double width, Plane p)
        {
            Interval intwd = new Interval(width / 2, -width / 2);
            Interval intht = new Interval(len, 0);
            Rectangle3d rec = new Rectangle3d(p, intht, intwd);
            return rec;
        }

        //Create duplicate curve with domain(0 to 1)
        public Curve DupdomainCrv(Curve inputcurv)
        {
            var curve_copy = inputcurv.DuplicateCurve();
            curve_copy.Domain = new Interval(0, 1);
            return curve_copy;
        }

        //Orient the XY plane based on Start/End
        public Plane Orientplane(Curve curve_copy)
        {
            double plnangle = checktype();
            curve_copy.PerpendicularFrameAt(framelen, out Plane pl);
            pl.Rotate(plnangle, pl.YAxis);
            pl.Rotate(4.71239, flip * pl.XAxis);
            return pl;
        }

        /// <summary>
        /// Check the Curve to decide whether bothside of the curve or it's on one side when the segments of the curve is one. 
        /// Based on the condition the Base point will be placed on the Curve Start/End
        /// </summary>
        /// <param name="cur"></param>
        /// <returns></returns>
        public bool checkcurvesegments(Curve cur)
        {
            var seg = cur.DuplicateSegments();
            if (seg.Count() > 1)
                return true;
            else
                return false;
        }
        //CheckDefault Values for Type3
        public Curve Checkdefault3(Curve cur)
        {
           
            if(checkcurvesegments(cur))
            {
                if (width < 3 * materialthickness)
                {
                    width = 3 * materialthickness;
                    MessageBox.Show("Height is less than Sheet thickness Default value is used");
                }
                if (len < 2 * materialthickness + 1.5)
                {
                    len = 2 * materialthickness + 1.5;
                    MessageBox.Show("Length is less than Sheet thickness Default value is used");
                }
                return Basecurve(width, len);
            }
            else
            {
                if (width < 5* materialthickness)
                {
                    width = (5 * materialthickness) + 0.5;
                    MessageBox.Show("Height is less than Sheet thickness Default value is used");
                }
                if (len < 5 * materialthickness + 0.5)
                {
                    len = 5* materialthickness + 0.5;
                    MessageBox.Show("Length is less than Sheet thickness Default value is used");
                }
                return Basecurve(width, len);
            }    
        }
    }
    #endregion

    #region Crimped Edge types
    public class CrimpClass : Edgebase
    {
        //Constructor
        public CrimpClass(Curve inputcurve, double Bboxpos,  CurveEnd trimend, double BBoxlength, double BBoxwidth, double modltolerance, double modlangltolerance, double flipside, double materialthickness) : base( inputcurve,  Bboxpos,  trimend,  BBoxlength,  BBoxwidth,  modltolerance,  modlangltolerance,  flipside,materialthickness) 
        {
           
        }

        //Type1
        public Curve Type1()
        {
            var curve_copy = DupdomainCrv(inputcurv);
            pln = Orientplane(curve_copy);
            if(len<materialthickness && width<materialthickness)
            {
                len = 3*materialthickness + RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
                width = 3 * materialthickness +RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
                MessageBox.Show("Length/Height is less than Sheet thickness Default value is used");
            }
            var poly = curve_copy.Trim(trimtype, len);
            var basecurve = Basecurve(width,len);
            var fill = Curve.CreateFilletCornersCurve(basecurve, width / 2, tolerance, angltolerance);
            RhinoList<Curve> cr = new RhinoList<Curve>();
            //cr.Add(poly);
            cr.Add(fill);
            var jn = Curve.JoinCurves(cr);
            return jn[0];
        }
        //Type2
        public Curve Type2(double materialthickness)
        {
            var curve_copy = DupdomainCrv(inputcurv);
            pln = Orientplane(curve_copy);
            var poly = curve_copy.Trim(trimtype, len);
            if(len<(materialthickness+0.1))
            {
                if (checkcurvesegments(inputcurv) == false)
                {len = materialthickness + materialthickness  + 10 * RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;}
                else { len = materialthickness + materialthickness / 2 + 10 * RhinoDoc.ActiveDoc.ModelAbsoluteTolerance; }
                MessageBox.Show("Radius is less than Sheet thickness Default value is used");
            }
            var basecurve = Basecurve(len, len);
            
            var lin = CurveManupulationClass.LinefromCrv(basecurve);

            RhinoList<Curve> typcrv = new RhinoList<Curve>();
            typcrv.Add(basecurve);
            typcrv.Add(lin.ToNurbsCurve());
            var basejoin = Curve.JoinCurves(typcrv);
            Curve cc = basejoin[0].ToNurbsCurve();
            var trcrv = cc.Trim(len / 3, 0.0);
            var fill = Curve.CreateFilletCornersCurve(trcrv, len / 2, tolerance, angltolerance);
            RhinoList<Curve> cr = new RhinoList<Curve>();
            //cr.Add(poly);
            cr.Add(fill);
            var jn = Curve.JoinCurves(cr);
           
            return jn[0];

        }
        //Type3
        public Curve Type3(double materialthickness)
        {
            var curve_copy = DupdomainCrv(inputcurv);
            pln = Orientplane(curve_copy);
            var poly = curve_copy.Trim(trimtype, len);
            var basecurve = Checkdefault3(curve_copy);
            Rhino.RhinoDoc.ActiveDoc.Objects.Add(basecurve);
            if (framelen == 1) { basecurve.Reverse(); }
            var fill = Curve.CreateFilletCornersCurve(basecurve, width / 4, tolerance, angltolerance);
            RhinoList<Curve> cr = new RhinoList<Curve>();
            var trmcrv = fill.Trim(trimtype, width/2);
            Rhino.RhinoDoc.ActiveDoc.Objects.Add(trmcrv);
            cr.Add(trmcrv);
            double ht;
            if(checkcurvesegments(inputcurv) == false)
            {
                ht = (width/2) + 1;
            }
            else
            { ht = width/2; }
            CrimpClass typ1 = new CrimpClass(fill, framelen, trimtype, width / 2, ht, tolerance, angltolerance, flip,materialthickness);
            var addtyp1 = typ1.Type1();
            cr.Add(addtyp1);            
            var jn = Curve.JoinCurves(cr);            
            return jn[0];
        }
        //Type4
        public Curve Type4(double materialthickness)
        {
            Plane p;
            var newtrmcrv = inputcurv.Trim(trimtype, (materialthickness / 2));
            var curve_c = DupdomainCrv(newtrmcrv);
            var fram = curve_c.FrameAt(framelen, out p);
            if (framelen == 1)
            {
                p.Rotate(3.14, p.ZAxis);
            }
            else
            {
                p.Rotate(0.0, p.ZAxis);
            }
            if (flip == -1)
            {
                var trans = Transform.Mirror(p.Origin, p.YAxis);
                p.Transform(trans);
            }
            if (width < materialthickness)
            {
                width = (2 * materialthickness);
                MessageBox.Show("Height is too less than Sheet Thickness the default minimum is used");
            }

            Rectangle3d rec = rect(materialthickness + 2 * RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, width + 2 * RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, p);

            Curve ccx = rec.ToNurbsCurve();
            Curve[] seg = ccx.DuplicateSegments();
            RhinoList<Point3d> ptlst = new RhinoList<Point3d>();
            foreach (Curve s in seg)
            {
                ptlst.Add(s.PointAtStart);
            }

            PolylineCurve pl = new PolylineCurve(ptlst);

            var trcrv = inputcurv.Trim(trimtype, (materialthickness) + (materialthickness / 2) + 2 * RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
            var curve_copy = DupdomainCrv(trcrv);
            var cur = new Line(curve_copy.PointAt(framelen), pl.PointAtEnd);
            var cur2 = new Line(curve_copy.PointAt(framelen), pl.PointAtStart);


            var endtrm = cur2.ToNurbsCurve().Trim(CurveEnd.Start, cur2.Length / 2);

            RhinoList<Curve> curvelst = new RhinoList<Curve>() { cur.ToNurbsCurve(), pl, endtrm };
            var join = Curve.JoinCurves(curvelst);

            if (join.Length > 0)
            {
                return join[0];
            }
            else { System.Windows.Forms.MessageBox.Show("Error", "error no valid curve");
                return curvelst[0];
            }
        }


    }
    #endregion

}
