
using Rhino.PlugIns;
using Rhino.UI;

namespace SheetMetal2D
{
    ///<summary>
    /// <para>Every RhinoCommon .rhp assembly must have one and only one PlugIn-derived
    /// class. DO NOT create instances of this class yourself. It is the
    /// responsibility of Rhino to create an instance of this class.</para>
    /// <para>To complete plug-in information, please also see all PlugInDescription
    /// attributes in AssemblyInfo.cs (you might need to click "Project" ->
    /// "Show All Files" to see it in the "Solution Explorer" window).</para>
    ///</summary>
    public class SheetMetal2DPlugIn : Rhino.PlugIns.PlugIn

    {
        public SheetMetal2DPlugIn()
        {
            Instance = this;
        }

        ///<summary>Gets the only instance of the SheetMetal2DPlugIn plug-in.</summary>
        public static SheetMetal2DPlugIn Instance
        {
            get; private set;
        }

        protected override LoadReturnCode OnLoad(ref string errorMessage)
        {
            System.Type panelType = typeof(SheetMainDock);

            Panels.RegisterPanel(this, panelType, "SheetMetal2D", Properties.Resources.SheetMetalCrossSection);

            return LoadReturnCode.Success;
        }
        // You can override methods here to change the plug-in behavior on
        // loading and shut down, add options pages to the Rhino _Option command
        // and maintain plug-in wide options in a document.
    }
}