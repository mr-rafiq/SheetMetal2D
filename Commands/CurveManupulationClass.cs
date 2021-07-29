using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SheetMetal2D
{
    public static class CurveManupulationClass
    {     

        public static Line linefromcurvesatstart(Curve c1,Curve c2)
        {
            var spt1 = c1.PointAtStart;
            var spt2 = c2.PointAtStart;
            Line lin = new Line(spt1, spt2);
            return lin;

        }

        public static Line linefromcurvesatend(Curve c1, Curve c2)
        {
            var ept1 = c1.PointAtEnd;
            var ept2 = c2.PointAtEnd;
            Line lin = new Line(ept1, ept2);
            return lin;
        }

        public static Line LinefromCrv(Curve Inputcrv)
        {
            var spt = Inputcrv.PointAtStart;
            var ept = Inputcrv.PointAtEnd;
            var lin = new Line(ept, spt);
            return lin;
        }

        public static RhinoList<Curve>Arraytolist(IEnumerable<Curve[]>curv)
        {
            RhinoList<Curve> lst = new RhinoList<Curve>();
            foreach(var c in curv)
            {
                foreach(var item in c)
                {
                    lst.Add(item);
                }
                
            }
            return lst;
        }

        public static PolylineCurve CreatePolyLineFromPoint(Point3d pt, PolylineCurve curv)
        {

            RhinoList<Point3d> plin = new RhinoList<Point3d>();
            var seg = curv.DuplicateSegments();
            foreach (var crv in seg) { plin.Add(crv.PointAtStart); }
            plin.Add(pt);
            PolylineCurve newcrv = new PolylineCurve(plin);
            return newcrv;

        }

        public static Point3d CreatePolyCurveoffPreview(Curve crv, GlobalVar var)
        {
            var prev = new OffsetPreview(crv, var);
            prev.Get();
            prev.SetCommandPrompt("Select Sides");

            return prev.Point();
            ///Visuals//
        }

        public static Line LineSDL(Point3d pt,Vector3d vec , double length)
        {
            return new Line(pt, vec, length);
        }
        public static Plane CenterCrvPlane(Curve crv)
        {
            crv.ToNurbsCurve().FrameAt(0.5, out Plane plane);
            return plane;
        }
    }
}
