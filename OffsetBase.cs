using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetMetal2D
{
    #region Curve Offset Class
    public class OffsetBase
    {

        public Curve inputCurve;
        public double distance;
        public double tolerance;
        public bool bothside;
        public bool cap;
        public Point3d point;
        public Plane plane;
        public Curve[] OffsetOneSide;
        public Curve OffsetOneSideCapped;
        public RhinoList<Curve[]> BothSide;
        public Curve BothSideCapped;

        #region Constructors 

        public OffsetBase(Curve inputcurve, Point3d point, double distance, double tolerance, bool bothside, bool cap)
        {
            this.inputCurve = inputcurve;
            this.distance = distance;
            this.tolerance = tolerance;
            this.bothside = bothside;
            this.cap = cap;
            this.point = point;
            this.OffsetOneSide = OffsetOneDirection(point, distance);
            this.BothSide = bothsideoffset();
            if (bothside == false && cap == true)
            { this.OffsetOneSideCapped = capcurve(); }
                
            if (bothside == true && cap == true)
            { this.BothSideCapped = capcurve(); }          
                
        }

        public OffsetBase(Curve inputcurve,Plane plane, double distance, double tolerance, bool bothside, bool cap)
        {
            this.inputCurve = inputcurve;
            this.distance = distance;
            this.tolerance = tolerance;
            this.bothside = bothside;
            this.cap = cap;
            this.plane = plane;
            this.OffsetOneSide = OffsetOneDirection(point, distance);
            this.BothSide = bothsideoffset();
            if (bothside == false && cap == true)
            { this.OffsetOneSideCapped = capcurve(); }

            if (bothside == true && cap == true)
            { this.BothSideCapped = capcurve(); }
        }

        #endregion

        #region Offset on one direction
        private Curve[] OffsetOneDirection(Point3d point, double distance)
        {
            Curve[] offsetCurves = inputCurve.Offset(point, Vector3d.ZAxis, distance, tolerance, CurveOffsetCornerStyle.Sharp);
            return offsetCurves;
        }

        #endregion

        #region Offset Both sides
        private RhinoList<Curve[]> bothsideoffset()
        {
            RhinoList<Curve[]> offcurves = new RhinoList<Curve[]>();
            Curve[] offset1 = OffsetOneDirection(point, distance / 2);
            Curve[] offset2 = OffsetOneDirection(point, -distance / 2);
            offcurves.Add(offset1);
            offcurves.Add(offset2);
            return offcurves;
        }
        #endregion

        #region CapCurve For bothside and on one side
        private Curve capcurve()
        {
            if (bothside == true)
            {
                RhinoList<Curve> curvelist = new RhinoList<Curve>();
                var res = bothsideoffset();
                Line startlin = CurveManupulationClass.linefromcurvesatstart(res[0][0], res[1][0]);
                Line endlin = CurveManupulationClass.linefromcurvesatend(res[0][0], res[1][0]);
                curvelist.Add(res[0][0]);
                curvelist.Add(res[1][0]);
                curvelist.Add(startlin.ToNurbsCurve());
                curvelist.Add(endlin.ToNurbsCurve());
                var joincurves = Curve.JoinCurves(curvelist);
                return joincurves[0].ToNurbsCurve();
            }
            else
            {
                RhinoList<Curve> curvelist = new RhinoList<Curve>();
                var res = OffsetOneDirection(point, distance);
                Line startlin = CurveManupulationClass.linefromcurvesatstart(inputCurve, res[0]);
                Line endlin = CurveManupulationClass.linefromcurvesatend(inputCurve, res[0]);
                curvelist.Add(inputCurve);
                curvelist.Add(res[0]);
                curvelist.Add(startlin.ToNurbsCurve());
                curvelist.Add(endlin.ToNurbsCurve());
                var joincurves = Curve.JoinCurves(curvelist);
                return joincurves[0].ToNurbsCurve();
            }
        }
        #endregion

    }
    #endregion


    #region Offset Curve with Multiple Points
    public class OffsetCurveCondition
    {
        public Curve PolyCurve;
        public RhinoList<Point3d> directionPoints;
        public double distance;
        public double tolerance;
        public bool bothside;
        public bool cap;
        public Curve rescentercrv;

        #region Constructor
        public OffsetCurveCondition(Curve PolyCurve, RhinoList<Point3d> directionPoints, double distance, double tolerance)
        {
            this.PolyCurve = PolyCurve;
            this.directionPoints = directionPoints;
            this.distance = distance;
            this.tolerance = tolerance;
            this.rescentercrv = SpecialOffset();
        }
        #endregion

        private RhinoList<Curve> Offsetwithpoints()
        {
            Curve[] lin = PolyCurve.DuplicateSegments();
            RhinoList<Curve> listline = new RhinoList<Curve>();
            var ziplinept = lin.Zip(directionPoints, (linlst, ptlst) => new { lin = linlst, directionPoints = ptlst });
            foreach (var lst in ziplinept)
            {
                var off = new OffsetBase(lst.lin, lst.directionPoints, distance / 2, tolerance, false, false);
                var offlin = off.OffsetOneSide;
                foreach (Curve curlin in offlin) { listline.Add(curlin);}
            }
            return listline;
        }
        private Curve SpecialOffset()
        {
            RhinoList<Curve> linelst = Offsetwithpoints();
            RhinoDoc doc = RhinoDoc.ActiveDoc;
            RhinoList<Point3d> pts = new RhinoList<Point3d>();
            var spt1 = linelst[0].PointAtStart;            
            pts.Add(spt1);
            var last = linelst.Count - 1;
            var ept1 = linelst[last].PointAtEnd;
            RhinoList<Line> linlst = new RhinoList<Line>();
            foreach(Curve cr in linelst)
            {
                var resl = CurveManupulationClass.LinefromCrv(cr);
                linlst.Add(resl);
            }
            RhinoList<Line> l1 = new RhinoList<Line>(linlst);
            l1.RemoveAt(0);
            RhinoList<Line> l2 = new RhinoList<Line>(linlst);
            l2.RemoveAt(last);
            var ziplines = l1.Zip(l2, (lin1, lin2) => new { l1 = lin1, l2 = lin2 });
            foreach (var lins in ziplines)
            {
                double paramA, paramB;
                var pt = Rhino.Geometry.Intersect.Intersection.LineLine(lins.l1, lins.l2, out paramA, out paramB, doc.ModelAbsoluteTolerance, false);                
                if (pt == true)
                {
                    Point3d pt0 = lins.l2.PointAt(paramB);
                    pts.Add(pt0);                   
                }
            }
            pts.Add(ept1);            
            Polyline poly = new Polyline(pts);
            //var offcurv = new OffsetBase(poly.ToNurbsCurve(), Plane.WorldXY, distance, doc.ModelAbsoluteTolerance, true, true);
            //var res = offcurv.capcurve();
            return poly.ToNurbsCurve();
        }
    }

    public class OffsetPreview : Rhino.Input.Custom.GetPoint
    {
        private Curve offcurve;
        public GlobalVar var;
        public OffsetPreview(Curve offcurve, GlobalVar var)
        {
            this.offcurve = offcurve;
            this.var = var;
        }
        protected override void OnDynamicDraw(GetPointDrawEventArgs e)
        {
            base.OnDynamicDraw(e);
            var point = e.CurrentPoint;
            var off = new OffsetBase(offcurve, point, this.var.Offsetthick, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, false, false);
            var curv = off.OffsetOneSide;
            e.Display.DrawCurve(curv[0], System.Drawing.Color.Red);
        }  
    }
    #endregion


}
