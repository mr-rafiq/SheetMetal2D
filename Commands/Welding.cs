using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Linq;

namespace SheetMetal2D
{
    public class Welding : Command
    {
        public override string EnglishName => "Welding";

        public GlobalVar var;

       
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            return SheetMetalWelding(doc, ref this.var);
        }

        
        public Result SheetMetalWelding(RhinoDoc doc, ref GlobalVar var)
        {
            this.var = var;
            RhinoList<Point3d> plin = new RhinoList<Point3d>();
            var gp = new GetPolylineCurve(plin);
            RhinoList<Guid> ids = new RhinoList<Guid>();
            Guid addedplId = Guid.Empty;
            RhinoList<Point3d> sides = new RhinoList<Point3d>();
            int ctor = 0;
            while (ctor>5)
            {
                gp.SetCommandPrompt("click location to create point. (<ESC> exit)");
                gp.Get();
                if (gp.CommandResult() != Result.Success)
                    break;
                else {
                    ctor += 1; }
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
                    var off = new OffsetBase(seg[seg.Length - 1], prvpoint, this.var.Offsetthick, doc.ModelAbsoluteTolerance, false, false);
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
                Param weldingparam = new Param(polyline.ToNurbsCurve(), sides, 0.5, this.var.Offsetthick, this.var.HatchDivision, doc, this.var);
                RhinoList<GeometryBase> geometry = weldingparam.WeldingObjects;
                Point3d base_point = polyline.ToNurbsCurve().PointAtEnd;
                string idef_name = "Welding";
                RhinoList<Guid> groupids = new RhinoList<Guid>();

                int layer_index = doc.Layers.Find(idef_name, true);
                string BlockName;
                if (layer_index >= 0)
                {
                    var layerobjs = doc.Objects.FindByLayer(idef_name);
                    int blockval = layerobjs.Length;
                    BlockName = "Welding " + blockval;
                }
                else
                {
                    BlockName = idef_name;
                }

                Rhino.DocObjects.InstanceDefinition existing_idef = doc.InstanceDefinitions.Find(BlockName, true);
                if (existing_idef != null)
                {
                    Rhino.RhinoApp.WriteLine("Block definition {0} already exists", BlockName);
                    return Rhino.Commands.Result.Nothing;
                }
                AttributeIds attid = new AttributeIds("Welding", this.var.GetselectedMaterialname, this.var.Offsetthick, this.var.Filletradius, this.var.HatchDivision, System.Drawing.Color.Red);
                CustomUserData csudata = new CustomUserData
                {
                    BaseCurve = polyline.ToNurbsCurve(),
                    UpdatePoints = sides,
                    Name = "Welding"
                };
                ObjectAttributes objatrr = attid.SetFoilerAttributes;
                objatrr.UserData.Add(csudata);
                string Description = "Welding sheet Metal Cross section plugin Object";
                int idef_index = doc.InstanceDefinitions.Add(BlockName, Description, base_point, geometry);
                Transform transform = Transform.Translation(((Vector3d)base_point));
                doc.Objects.AddInstanceObject(idef_index, transform, objatrr);
            }
            return Result.Success;
        }

    }
}