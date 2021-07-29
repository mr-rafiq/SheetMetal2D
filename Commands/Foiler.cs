using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace SheetMetal2D.Commands
{
    [Guid("B328D19A-4B27-4E97-B8C7-7BD348E483F1")]
    public class SheetFoiler : Command
    {

        public override string EnglishName => "SheetFoiler";

        public GlobalVar var;

        [Obsolete]
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            return SheetMetalFoiler(doc, ref this.var);
        }

        [Obsolete]
        public Result SheetMetalFoiler(RhinoDoc doc, ref GlobalVar var)
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
                if (gp.CommandResult() != Result.Success)
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
                    var off = new OffsetBase(seg[seg.Length - 1], prvpoint, this.var.Offsetthick, doc.ModelAbsoluteTolerance, false, false);
                    var newcurv = off.OffsetOneSide;
                    ids.Add(doc.Objects.AddCurve(newcurv[0]));
                    ///Visuals//
                }
                doc.Views.Redraw();
            }
            doc.Objects.Delete(ids, true);



            //HatchPattern pattern = doc.HatchPatterns.FindName(HatchPattern.Defaults.Solid.Name);
            //int index = (null == pattern) ? doc.HatchPatterns.Add(HatchPattern.Defaults.Solid) : pattern.Index;
            if (plin.Count() > 1)
            {
                PolylineCurve polyline = new PolylineCurve(plin);
                Param foilerparam = new Param(polyline.ToNurbsCurve(), sides, 0.5, this.var.Offsetthick, this.var.HatchDivision, doc, this.var);
                RhinoList<GeometryBase> geometry = foilerparam.Foiler();
                Point3d base_point = polyline.ToNurbsCurve().PointAtEnd;
                string idef_name = "Foiler";
                RhinoList<Guid> groupids = new RhinoList<Guid>();
                //geometry.ForEach(x => groupids.Add(doc.Objects.Add(x)));
                //doc.Groups.Add(groupids);

                int layer_index = doc.Layers.Find(idef_name, true);
                string BlockName;
                if (layer_index >= 0)
                {
                    var layerobjs = doc.Objects.FindByLayer(idef_name);
                    int blockval = layerobjs.Length;
                    BlockName = "Foiler " + blockval;
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
                AttributeIds attid = new AttributeIds("Foiler", this.var.GetselectedMaterialname, this.var.Offsetthick, this.var.Filletradius, this.var.HatchDivision, System.Drawing.Color.DarkTurquoise);
                CustomUserData csudata = new CustomUserData
                {
                    BaseCurve = polyline.ToNurbsCurve(),
                    UpdatePoints = sides,
                    Name = "Foiler"
                };
                ObjectAttributes objatrr = attid.SetFoilerAttributes;
                objatrr.UserData.Add(csudata);
                string Description = "Foiler sheet Metal Cross section plugin Object";
                int idef_index = doc.InstanceDefinitions.Add(BlockName, Description, base_point, geometry);                
                Transform transform = Transform.Translation(((Vector3d)base_point));
                doc.Objects.AddInstanceObject(idef_index, transform, objatrr);
            }
            return Result.Success;
        }

    }


}