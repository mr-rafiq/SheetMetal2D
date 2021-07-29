using MoreLinq;
using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SheetMetal2D
{
    public class Param : UserData
    {

        public Curve BasePolycurve;
        public RhinoDoc doc;
        public GlobalVar var;
        public RhinoList<Point3d> DirectionPoints;
        public double Filletvalue;
        public double Offsetvalue;
        public double Division;
        public Curve BaseParamCurve;
        public Curve SheetMetalCurve;
        public RhinoList<GeometryBase> FoilerObjects;
        public RhinoList<GeometryBase> WeldingObjects;
        public Curve[] Seg { get { return this.BasePolycurve.DuplicateSegments(); } }
        public Plane Plane { get {return Plane.WorldXY; } }

        public Param(Curve BasePolycurve, RhinoList<Point3d> DirectionPoints, double Filletvalue, double Offsetvalue, RhinoDoc doc, GlobalVar var)
        {
            this.BasePolycurve = BasePolycurve;
            this.DirectionPoints = DirectionPoints;
            this.doc = doc;
            this.var = var;
            this.Filletvalue = Filletvalue;
            this.Offsetvalue = Offsetvalue;
            this.BaseParamCurve = BaseParam();
            this.SheetMetalCurve = SheetMetal();
            
        }
        public Param(Curve BasePolycurve, RhinoList<Point3d> DirectionPoints, double Filletvalue, double Offsetvalue,double Division, RhinoDoc doc, GlobalVar var)
        {
            this.BasePolycurve = BasePolycurve;
            this.DirectionPoints = DirectionPoints;
            this.doc = doc;
            this.var = var;
            this.Filletvalue = Filletvalue;
            this.Offsetvalue = Offsetvalue;
            this.BaseParamCurve = BaseParam();
            this.SheetMetalCurve = SheetMetal();
            this.Division = Division;
            this.FoilerObjects = Foiler();
            this.WeldingObjects = Welding();
        }
        private Curve BaseParam()
        {
            RhinoList<Curve> rescrv = new RhinoList<Curve>();
            //Curve[] seg = this.BasePolycurve.DuplicateSegments();
            OffsetCurveCondition sploff = new OffsetCurveCondition(this.BasePolycurve, this.DirectionPoints, this.Offsetvalue, this.doc.ModelAbsoluteTolerance);            
            double segCount = this.Seg.ToList().Count;
            Curve res = segCount > 1 ? sploff.rescentercrv : this.BasePolycurve;
            RhinoList<Curve> curvob = new RhinoList<Curve>();
            Curve pol = (Curve)res.DuplicateCurve();

            if (segCount > 1)
            {
                double filletval = this.Filletvalue == 0 ? 0.1 : this.Filletvalue + (this.Offsetvalue / 2);
                Curve fill = Curve.CreateFilletCornersCurve(pol, filletval, this.doc.ModelAbsoluteTolerance, this.doc.ModelAngleToleranceRadians);
                curvob.Add(fill);
            }
            else { curvob.Add(pol); }
            Curve[] joinedcurv = Curve.JoinCurves(curvob, this.doc.ModelAbsoluteTolerance);

            #region sheetmetal
            //if (segCount == 1)
            //{
            //    OffsetBase off = new OffsetBase(joinedcurv[0], this.DirectionPoints[0], this.Offsetvalue, this.doc.ModelAbsoluteTolerance, false, true);
            //    rescrv.Add(off.OffsetOneSideCapped);
            //}
            //if (segCount > 1)
            //{
            //    OffsetBase off = new OffsetBase(joinedcurv[0], this.Plane, this.Offsetvalue, this.doc.ModelAbsoluteTolerance, true, true);
            //    rescrv.Add(off.BothSideCapped);
            //}
            #endregion 

            return joinedcurv[0];
        }

        /// <summary>
        /// Take the Centere curve and adds the offset to it based on the direction point
        /// Crimped edge to be added
        /// </summary>
        /// <returns></returns>
        public Curve SheetMetal()
        {
            double segCount = this.Seg.ToList().Count;
            RhinoList<Curve> rescrv = new RhinoList<Curve>();

            Curve joinedcurv = BaseParam();

            if (segCount == 1)
            {
                OffsetBase off = new OffsetBase(joinedcurv, this.DirectionPoints[0], this.Offsetvalue, this.doc.ModelAbsoluteTolerance, false, true);
                rescrv.Add(off.OffsetOneSideCapped);
            }
            if (segCount > 1)
            {
                OffsetBase off = new OffsetBase(joinedcurv, this.Plane, this.Offsetvalue, this.doc.ModelAbsoluteTolerance, true, true);
                rescrv.Add(off.BothSideCapped);
            }
            
            return rescrv[0];
        }


        /// <summary>
        /// Foiler
        /// Foiler Take the offset curve without capped 
        /// Divide one of the curve and get the list of points
        /// To get the perpendicular line find the closest point in the non divided line using the generated points.
        /// Add line of every two points comparing the list
        /// To get the centere Point for generating the region curve divide the basepolycurve into segments to using list of points from the division.
        /// Now chunks the list of perpendicular lines and add the offseted line to generate a separate list
        /// With Curve.CreateBooleanRegions create the region curves for each list
        /// Add the hatches to alternate of the regions curves.
        /// Create a block with hatch and curves
        /// </summary>
        /// <returns></returns>
        public RhinoList<GeometryBase> Foiler()
        {

            double segCount = this.Seg.ToList().Count;
            RhinoList<Curve> rescrv = new RhinoList<Curve>();
            Point3d[] dividepoints;
            double t;
            List<double> ptparam = new List<double>();
            Curve CentreCurve = BaseParam();

            #region Check for Curve segment to offset
            if (segCount == 1)
            {
                OffsetBase off = new OffsetBase(CentreCurve, this.DirectionPoints[0], this.Offsetvalue, this.doc.ModelAbsoluteTolerance, false, true);
                rescrv.Add(off.OffsetOneSide[0]);
                rescrv.Add(this.BasePolycurve);
            }
            if (segCount > 1)
            {
                OffsetBase off = new OffsetBase(CentreCurve, this.Plane, this.Offsetvalue, this.doc.ModelAbsoluteTolerance, true, true);
                rescrv.Add(off.BothSide[0][0]);
                rescrv.Add(off.BothSide[1][0]);
            }
            #endregion

            #region Dividing the Curve 
            rescrv[0].DivideByLength(this.Division, false, false , out dividepoints);
            RhinoList<Point3d> Firstcurvdividedpts =new RhinoList<Point3d>();
            Firstcurvdividedpts.Add(rescrv[0].PointAtStart);
            Firstcurvdividedpts.AddRange(dividepoints.ToList());
            Firstcurvdividedpts.Add(rescrv[0].PointAtEnd);
            foreach (Point3d item in dividepoints.ToList())
            {
                CentreCurve.ClosestPoint(item, out t);
                ptparam.Add(t);
            }
            Curve[] splittedcurv = CentreCurve.Split(ptparam);

            RhinoList<Point3d> CurvemidPoint = new RhinoList<Point3d>();
            splittedcurv.ToList().ForEach(x => CurvemidPoint.Add(x.PointAtNormalizedLength(0.5)));
            RhinoList<Point3d> PointAtSecondCurv = new RhinoList<Point3d>();
            foreach (Point3d item in Firstcurvdividedpts)
            {
                rescrv[1].ClosestPoint(item, out t);
                PointAtSecondCurv.Add(rescrv[1].PointAt(t));
            }
            RhinoList<Line> ConnectingLines = new RhinoList<Line>();
            for (int i = 0; i < PointAtSecondCurv.Count; i++)
            {
                Line ln = new Line(Firstcurvdividedpts[i], PointAtSecondCurv[i]);
                ConnectingLines.Add(ln);
            }
            #endregion

            #region List Manupulation and Boolean regions Curves
            RhinoList<Curve[]> RegionCurves = new RhinoList<Curve[]>();
            List<Curve> Connectingcurv = new List<Curve>();
            for (int i = 0; i < ConnectingLines.Count - 1; i++)
            {
                List<Curve> bc = new List<Curve>();
                bc.Add(ConnectingLines[i].ToNurbsCurve());
                bc.Add(ConnectingLines[i + 1].ToNurbsCurve());
                Connectingcurv.AddRange(bc);
            }
            IEnumerable<IEnumerable<Curve>> linelistchunks = Connectingcurv.Batch(2);
            linelistchunks.ToList();
            List<Curve> GroupedCurvlist = new List<Curve>();
            for (int i = 0; i < linelistchunks.Count(); i++)
            {
                GroupedCurvlist.AddRange(linelistchunks.ElementAt(i).ToList());
                GroupedCurvlist.AddRange(rescrv);
            }
            IEnumerable<IEnumerable<Curve>> FinalChunklist = GroupedCurvlist.Batch(4);
            foreach (IEnumerable<Curve> item in FinalChunklist)
            {
                CurveBooleanRegions Ins = Curve.CreateBooleanRegions(item, Plane.WorldXY, CurvemidPoint, true, this.doc.ModelAbsoluteTolerance);
                RegionCurves.Add(Ins.RegionCurves(0));
            }
            RhinoList<Curve> Selectalternate = new RhinoList<Curve>();

            RegionCurves.ToList().Where((s, i) => i % 2 != 0).ToList().ForEach(x => x.ToList().ForEach(y => Selectalternate.Add(y)));

            #endregion
            RhinoList<GeometryBase> geometry = new RhinoList<GeometryBase>();

            #region Making the Hatch
            HatchPattern pattern = this.doc.HatchPatterns.FindName(HatchPattern.Defaults.Solid.Name);
            int index = (null == pattern) ? this.doc.HatchPatterns.Add(HatchPattern.Defaults.Solid) : pattern.Index;
            Hatch[] hatches = Hatch.Create(Selectalternate, index, 0, 1);
            for (int i = 0; i < hatches.Length; i++)
            {
                geometry.Add(hatches[i]);
            }
            //RegionCurves.ForEach(x => x.ForEach(y => this.doc.Objects.AddCurve(y)));
            #endregion

            RegionCurves.ForEach(x => x.ForEach(y => geometry.Add(y)));
            
            return geometry;
        }

        /// <summary>
        ///  Welding
        /// Welding Take the offset curve without capped 
        /// Divide one of the curve and get the list of points
        /// </summary>
        /// <returns></returns>
        public RhinoList<GeometryBase> Welding()
        {

            double segCount = this.Seg.ToList().Count;
            RhinoList<Curve> rescrv = new RhinoList<Curve>();            
            Curve CentreCurve = BaseParam();

            #region Check for Curve segment to offset
            if (segCount == 1)
            {
                OffsetBase off = new OffsetBase(CentreCurve, this.DirectionPoints[0], this.Offsetvalue, this.doc.ModelAbsoluteTolerance, false, true);
                rescrv.Add(off.OffsetOneSide[0]);
                rescrv.Add(this.BasePolycurve);
            }
            if (segCount > 1)
            {
                OffsetBase off = new OffsetBase(CentreCurve, this.Plane, this.Offsetvalue, this.doc.ModelAbsoluteTolerance, true, true);
                rescrv.Add(off.BothSide[0][0]);
                rescrv.Add(off.BothSide[1][0]);
            }
            #endregion

            #region Divide Curve

            double[] firstcurveparam = rescrv[0].DivideByLength(this.Division, true);
            RhinoList<Plane> horizonatalplanes = new RhinoList<Plane>();             
            foreach(double item in firstcurveparam.ToList())
            {
                rescrv[0].FrameAt(item, out Plane plane);
                horizonatalplanes.Add(plane);
            }
            horizonatalplanes.RemoveAt(0);
            horizonatalplanes.RemoveAt(horizonatalplanes.Count - 1);
            #endregion

            #region Create planes and Arcs
            RhinoList<Vector3d> Yvector = new RhinoList<Vector3d>();
            RhinoList<Point3d> originpts = new RhinoList<Point3d>();
            horizonatalplanes.ForEach(x => Yvector.Add(x.YAxis));
            horizonatalplanes.ForEach(x => originpts.Add(x.Origin));

            RhinoList<Line> linesdl = new RhinoList<Line>();
            for(int i = 0; i < originpts.Count; i++)
            {
                linesdl.Add(new Line(originpts[i], Yvector[i], this.Offsetvalue));
            }
            //originpts.ForEach(x => Yvector.ForEach(y => linesdl.Add(new Line(x, y, this.Offsetvalue))));
            RhinoList<Point3d> intersecitonpoints = new RhinoList<Point3d>();
            RhinoList<Point3d> culledpoints = new RhinoList<Point3d>();
            RhinoList<CurveIntersections> intersectioneventslist = new RhinoList<CurveIntersections>();
            
            for (int i = 0; i < linesdl.Count; i++)
            {                
                CurveIntersections curvins = Intersection.CurveCurve(rescrv[1], linesdl[i].ToNurbsCurve(), Rhino.RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, 0.0);
                curvins.ForEach(x => intersecitonpoints.Add(x.PointA));
                intersectioneventslist.Add(curvins);
            } 
            for (int i = 0; i < intersectioneventslist.Count - 1; i++)
            {
                if (intersectioneventslist[i].Count != 0)
                { culledpoints.Add(originpts[i]); } 
            }
            RhinoList<Line> cleanline = new RhinoList<Line>();

            culledpoints.ForEach(x => intersecitonpoints.ForEach(y => cleanline.Add(new Line(x, y))));
            RhinoList<Plane> lineplanes = new RhinoList<Plane>();
            RhinoList<Point3d> centrepoints = new RhinoList<Point3d>();
            RhinoList<Vector3d> vectormovepoint = new RhinoList<Vector3d>();
            cleanline.ForEach(x => lineplanes.Add(CurveManupulationClass.CenterCrvPlane(x.ToNurbsCurve())));
            lineplanes.ForEach(x => centrepoints.Add(x.Origin));
            lineplanes.ForEach(x => vectormovepoint.Add(x.YAxis/2));
            centrepoints.ForEach(x => vectormovepoint.ForEach(y => x.Transform(Transform.Translation(y))));
            RhinoList<Curve> archatch = new RhinoList<Curve>();
            for (int i = 0; i < culledpoints.Count - 1; i++)
            {
                archatch.Add(new Arc(culledpoints[i], centrepoints[i], intersecitonpoints[i]).ToNurbsCurve());
            }
            RhinoList<GeometryBase> geometrybase = new RhinoList<GeometryBase>();
            geometrybase.AddRange(archatch);
            #endregion

            return geometrybase;
        }


    }
}
