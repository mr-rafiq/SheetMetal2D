using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SheetMetal2D
{
    [Guid("C42A94AA-B242-4FBE-9E65-BB4F30EB94CC")]
    public class SheetMetalCrossSection : Command
    {

        public override string EnglishName => "SheetMetalCrossSection";

        public GlobalVar var;

        protected override Result RunCommand(Rhino.RhinoDoc doc, RunMode mode )
        {            
            // TODO: complete command.
            return SheetCrossSection(doc,ref this.var);
        }


        public static Curve StartSwtich(Curve pol,Curve res, CrimpClass edb, GlobalVar var, RhinoDoc doc)
        {
            RhinoList<Curve> crv = new RhinoList<Curve>();
            switch (var.StartSwitch)
            {                
                case "Starttype1":
                    if (var.StartLength < var.Offsetthick && var.StartHeight < var.Offsetthick)
                    {
                        pol = (Curve)pol.Trim(CurveEnd.Start, 2 * var.Offsetthick + doc.ModelAbsoluteTolerance);
                    }
                    else { pol = (Curve)pol.Trim(CurveEnd.Start, var.StartLength); }
                    crv.Add(edb.Type1());
                    break;                    

                case "Starttype2":
                    if (edb.checkcurvesegments(res) == true)
                    {
                        if (var.StartLength < (var.Offsetthick / 2 + 0.6))
                        {
                            pol = (Curve)pol.Trim(CurveEnd.Start, (var.Offsetthick + var.Offsetthick / 2 + 10 * doc.ModelAbsoluteTolerance));
                        }
                    }
                    else if (edb.checkcurvesegments(res) == false)
                    {
                        if (var.StartLength < (var.Offsetthick / 2 + 0.6))
                        {
                            pol = (Curve)pol.Trim(CurveEnd.Start, (2 * var.Offsetthick + 10 * doc.ModelAbsoluteTolerance));
                        }
                    }
                    else { pol = (Curve)pol.Trim(CurveEnd.Start,var.StartLength); }
                    crv.Add(edb.Type2(var.Offsetthick));                    
                    break;

                case "Starttype3":
                    if (edb.checkcurvesegments(res) == true)
                    {
                        if (var.StartLength < (2 * var.Offsetthick + 0.5) && (var.StartHeight < (2 * var.Offsetthick)))
                        {
                            pol = (Curve)pol.Trim(CurveEnd.Start, (2 * var.Offsetthick + 0.5 + (var.Offsetthick / 2)));
                        }
                    }
                    else if (edb.checkcurvesegments(res) == false)
                    {
                        if (var.StartLength < (5 * var.Offsetthick + 0.5) && (var.StartHeight < (5 * var.Offsetthick)))
                        {
                            pol = (Curve)pol.Trim(CurveEnd.Start, (5 * var.Offsetthick + 0.5));
                        }
                    }
                    else { pol = (Curve)pol.Trim(CurveEnd.Start, var.StartLength); }
                    crv.Add(edb.Type3(var.Offsetthick));
                    break;

                case "Starttype4":
                    if (var.StartLength < 2 * var.Offsetthick)
                    {
                        pol = (Curve)pol.Trim(CurveEnd.Start, (var.Offsetthick) + (var.Offsetthick / 2) + 2 * doc.ModelAbsoluteTolerance);
                    }
                    else { pol = (Curve)pol.Trim(CurveEnd.Start, var.StartLength); }
                    crv.Add(edb.Type4(var.Offsetthick));                    
                    break;
            }
            return crv[0];
        }
        public static Curve EndSwtich(Curve pol, Curve res, CrimpClass edb, GlobalVar var, RhinoDoc doc)
        {
            RhinoList<Curve> curvob = new RhinoList<Curve>();
            switch (var.EndSwitch)
            {
                case "Endtype1":
                    if (var.EndLength < var.Offsetthick && var.EndHeight < var.Offsetthick)
                    {
                        pol = (Curve)pol.Trim(CurveEnd.End, 2 * var.Offsetthick + doc.ModelAbsoluteTolerance);
                    }
                    else { pol = (Curve)pol.Trim(CurveEnd.End, var.EndLength); }
                    var curv = edb.Type1();
                    curvob.Add(curv);
                    break;

                case "Endtype2":
                    if (var.EndLength < (var.Offsetthick / 2 + 0.6))
                    {
                        pol = (Curve)pol.Trim(CurveEnd.End, (var.Offsetthick + var.Offsetthick / 2 + 10 * doc.ModelAbsoluteTolerance));
                    }
                    var curv2 = edb.Type2(var.Offsetthick);
                    curvob.Add(curv2);
                    break;

                case "Endtype3":
                    if (var.EndLength < (4 * var.Offsetthick + 0.5) && var.EndHeight < 4 * var.Offsetthick)
                    {
                        pol = (Curve)pol.Trim(CurveEnd.End, ((3 * var.Offsetthick) + 0.5));
                    }
                    else { pol = (Curve)pol.Trim(CurveEnd.End, var.EndLength); }
                    var curv3 = edb.Type3(var.Offsetthick);
                    curvob.Add(curv3);
                    break;

                case "Endtype4":
                    if (var.EndLength < 2 * var.Offsetthick)
                    {
                        pol = (Curve)pol.Trim(CurveEnd.End, (var.Offsetthick) + (var.Offsetthick / 2) + 2 * doc.ModelAbsoluteTolerance);
                    }
                    else { pol = (Curve)pol.Trim(CurveEnd.End, var.EndLength); }
                    var curv4 = edb.Type4(var.Offsetthick);
                    curvob.Add(curv4);                  
                    break;
            }
            return curvob[0];
        }
        
        #region CrossSection
        public  Result SheetCrossSection(RhinoDoc doc, ref GlobalVar var)
        {
            this.var = var;
            RhinoList<Point3d> plin = new RhinoList<Point3d>();
            var gp = new GetPolylineCurve(plin);
            RhinoList<Guid> ids = new RhinoList<Guid>();
            Guid addedplId = Guid.Empty;
            RhinoList<Point3d> sides = new RhinoList<Point3d>();
            while (true)
            {
                gp.SetCommandPrompt("click location to create point. (<ESC> exit)");
                gp.Get();
                if (gp.CommandResult() != Rhino.Commands.Result.Success)
                    break;
                Point3d pt = gp.Point();
                plin.Add(pt);
                ids.Add(doc.Objects.AddPolyline(plin));
                PolylineCurve curv = new PolylineCurve(plin);
                var respolycrv = CurveManupulationClass.CreatePolyLineFromPoint(pt, curv);
                ids.Add(doc.Objects.AddCurve(respolycrv));
                if (plin.Count > 1)
                {                   
                    var seg = respolycrv.DuplicateSegments();
                    var prvpoint = CurveManupulationClass.CreatePolyCurveoffPreview(seg[seg.Length - 1], this.var);
                    ///Visuals//
                    sides.Add(prvpoint);        
                    ids.Add(doc.Objects.AddPoint(prvpoint));
                    var off = new OffsetBase(seg[seg.Length - 1], prvpoint, this.var.Offsetthick, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, false, false);
                    var newcurv = off.OffsetOneSide;
                    ids.Add(doc.Objects.AddCurve(newcurv[0]));
                    ///Visuals//
                }
                doc.Views.Redraw();
            }
            doc.Objects.Delete(ids, true);

            if (plin.Count() > 1)
            {
                PolylineCurve polyline = new PolylineCurve(plin);
                var lin = polyline.DuplicateSegments();
                var sploff = new OffsetCurveCondition(polyline,sides,this.var.Offsetthick,doc.ModelAbsoluteTolerance);
                Curve res;
                if (lin.Count() > 1){  res = sploff.rescentercrv; } // If the line segment is more than one then add the special offset
                else {  res = polyline; }          
                RhinoList<Curve> curvob = new RhinoList<Curve>();
                Curve pol = (Curve)res.DuplicateCurve();

                #region Adding Crimped Egde
                if (var.StartType)
                {CrimpClass edb = new CrimpClass(res, 0, CurveEnd.Start, this.var.StartLength, this.var.StartHeight, doc.ModelAbsoluteTolerance, doc.ModelAngleToleranceRadians, this.var.startflip, this.var.Offsetthick);
                    curvob.Add(StartSwtich(pol, res, edb, this.var, doc));
                }

                if (this.var.EndType)
                {CrimpClass edb = new CrimpClass(res, 1.0, CurveEnd.End, this.var.EndLength, this.var.EndHeight, doc.ModelAbsoluteTolerance, doc.ModelAngleToleranceRadians, this.var.endflip, this.var.Offsetthick);
                    curvob.Add(EndSwtich(pol, res, edb, this.var, doc));
                }
                #endregion

                #region Add Fillet if the number of segments is more than one line 
                if (plin.Count > 2)
                {
                    var fill = Curve.CreateFilletCornersCurve(pol, (this.var.Filletradius+(this.var.Offsetthick/2)), doc.ModelAbsoluteTolerance, doc.ModelAngleToleranceRadians);
                    curvob.Add(fill);
                }
                else { curvob.Add(pol);}
                #endregion

                Curve[] joinedcurv = Curve.JoinCurves(curvob,doc.ModelAbsoluteTolerance); // if the Curves are not joined properly check the crimped edge

                if(lin.Count() == 1)// If it's line just offset on one side with base point.
                {
                    var off = new OffsetBase(joinedcurv[0], sides[0], this.var.Offsetthick, doc.ModelAbsoluteTolerance, false, true);
                    AttributeIds attid = new AttributeIds("SheetCrossSection", this.var.GetselectedMaterialname, this.var.Offsetthick, this.var.Filletradius, this.var.StartType, this.var.StartSwitch, this.var.EndType, this.var.EndSwitch, System.Drawing.Color.DarkOrange);
                    CustomUserData csudata = new CustomUserData
                    {
                        BaseCurve = polyline.ToNurbsCurve(),
                        UpdatePoints = sides,
                        Name = "SheetCrossSection"
                    };
                    //off.OffsetOneSideCapped.UserData.Add(csudata);
                    attid.SetSheetCrossSectionAttributes.UserData.Add(csudata);
                    doc.Objects.AddCurve(off.OffsetOneSideCapped, attid.SetSheetCrossSectionAttributes);
                }
                if (lin.Count() > 1)
                {
                    var off = new OffsetBase(joinedcurv[0], Plane.WorldXY, this.var.Offsetthick, doc.ModelAbsoluteTolerance, true, true); 
                    AttributeIds attid = new AttributeIds("SheetCrossSection", this.var.GetselectedMaterialname, this.var.Offsetthick, this.var.Filletradius, this.var.StartType, this.var.StartSwitch, this.var.EndType, this.var.EndSwitch, System.Drawing.Color.DarkOrange);
                    CustomUserData csudata = new CustomUserData 
                    {
                        BaseCurve = polyline.ToNurbsCurve(),
                        UpdatePoints = sides, 
                        Name = "SheetCrossSection" };                    
                    attid.SetSheetCrossSectionAttributes.UserData.Add(csudata);
                    _ = doc.Objects.AddCurve(off.BothSideCapped, attid.SetSheetCrossSectionAttributes);
                }
            } 
            doc.Views.Redraw();
            return Result.Success;
        }


        //CloseEvents
        public  Result SheetCrossSectionClosePanel(RhinoDoc doc, ref GlobalVar var)
        {
            Guid Panelid = SheetMainDock.PanelId;
            Rhino.UI.Panels.ClosePanel(Panelid);
            return Result.Success;
        }

        #endregion


    }
    //DynamicDraw Events
    public class GetPolylineCurve : Rhino.Input.Custom.GetPoint
    {
        private RhinoList<Point3d> pl_point { get; set; }
        public GetPolylineCurve(RhinoList<Point3d> ppoint)
        {
            pl_point = ppoint;
        }
        protected override void OnDynamicDraw(GetPointDrawEventArgs e)
        {
            float f = 2F;
            RhinoList<Point3d> fn_point = new RhinoList<Point3d>();
            fn_point.AddRange(pl_point);
            fn_point.Add(e.CurrentPoint);
            e.Display.DrawPolyline(fn_point, Rhino.RhinoDoc.ActiveDoc.Layers.CurrentLayer.Color);
            e.Display.DrawPoints(fn_point, PointStyle.RoundSimple, f, System.Drawing.Color.Red);
            base.OnDynamicDraw(e);
        }

    }
}
