using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

namespace SheetMetal2D
{
    public class SheetMetal2DCommand : Command
    {
       
        public SheetMetal2DCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;            
            //GlobalVar.Docunit = doc.ModelUnitSystem.ToString();
        }

        ///<summary>The only instance of this command.</summary>
        public static SheetMetal2DCommand Instance
        {
            get; private set;
        }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName
        {
            get { return "SheetMetal2DCommand"; }
        }

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            doc.Views.Redraw();
            var Panelid = SheetMainDock.PanelId;
            
            Rhino.UI.Panels.OpenPanel(Panelid);
            return Result.Success;
        }

    }
}
