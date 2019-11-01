using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitJumper.UI.MVVM.Jumper.View;
using System.Linq;

namespace RevitJumper
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    public class JumperCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var version = commandData.Application.Application.VersionNumber;
            var doc = uidoc.Document;
            var elemIds = uidoc.Selection.GetElementIds();
            var wnd = new JumperWnd(doc, elemIds.ToList(), version);
            wnd.ShowHostDialog();          
            return Result.Succeeded;
        }       
    }
}
