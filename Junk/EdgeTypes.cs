using Rhino;
using Rhino.Collections;
using Rhino.Geometry;

namespace SheetMetal2D
{
    public  class EdgeTypes
    {       
        
        public static Curve type1Curve (PolylineCurve polyc,ref GlobalVar var )
        {
            Curve cur = polyc.ToNurbsCurve();
            Point3d pts = cur.PointAt(0.0);
            Point3d ptd = cur.PointAtLength(var.StartHeight);
            Line ln = new Line(pts, ptd);
            Curve c = ln.ToNurbsCurve();                 
            
            
            Curve[] off = c.Offset(Plane.WorldXY, var.StartLength, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp);
                      
            Point3d acpt = off[0].PointAtStart;            

            Vector3d vc = acpt - off[0].PointAtEnd;
            Point3d chkpt = new Point3d(acpt.X + 2, acpt.Y, 0);
            Arc arc = new Rhino.Geometry.Arc(pts, vc, acpt);
            RhinoList<Curve> joincurv = new RhinoList<Curve>();
            joincurv.Add(arc.ToNurbsCurve());
            joincurv.Add(off[0]);

            Curve[] polys = Rhino.Geometry.Curve.JoinCurves(joincurv, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
            return (polys[0]);
        }

        public static Curve endtype1Curve(PolylineCurve polyc, ref GlobalVar var)
        {
            Curve cur = polyc.ToNurbsCurve();
            Point3d pts = cur.PointAtEnd;
            double lengthinput = cur.GetLength();
            Point3d ptd = cur.PointAtLength(lengthinput - var.StartHeight);
            Line ln = new Line(pts, ptd);
            Curve c = ln.ToNurbsCurve();


            Curve[] off = c.Offset(Plane.WorldXY, var.StartLength, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp);

            Point3d acpt = off[0].PointAtStart;

            Vector3d vc = acpt - off[0].PointAtEnd;
            Point3d chkpt = new Point3d(acpt.X + 2, acpt.Y, 0);
            Arc arc = new Rhino.Geometry.Arc(pts, vc, acpt);
            RhinoList<Curve> joincurv = new RhinoList<Curve>();
            joincurv.Add(arc.ToNurbsCurve());
            joincurv.Add(off[0]);

            Curve[] polys = Rhino.Geometry.Curve.JoinCurves(joincurv, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance);
            return (polys[0]);
        }





    }
}
