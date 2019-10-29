using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevitJumper.UI.MVVM.Jumper.Model;
using RevitJumper.UI.MVVM.Jumper.View;
using System;
using System.Collections.Generic;
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

            var searchcontent = string.Empty;
            if (elemIds != null && elemIds.Any())
            {
                var targetId = elemIds.FirstOrDefault();
                var target = doc.GetElement(targetId);
                if (target != null)
                {
                    var type = target.GetType().Name;
                    searchcontent = type;
                }
                else
                {
                    searchcontent = "View";
                }
                
                var query = new Query();
                try
                {
                    var models = new List<DisplayModel>();

                    var result = query.GetSearchResult(searchcontent);
                    JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                    var sections = jo["sections"].ToString();
                    JObject products = (JObject)JsonConvert.DeserializeObject(sections);
                    var productsinfo = products["Products"].ToString();
                    JArray array = JArray.Parse(productsinfo);
                    foreach (var info in array)
                    {
                        var relatedkey = info["value"].ToString();
                        var data = info["data"].ToString();
                        JObject datas = (JObject)JsonConvert.DeserializeObject(data);
                        var description = datas["description"].ToString();
                        var url = datas["url"].ToString();

                        var model = new DisplayModel()
                        {
                            RelatedKey = relatedkey,
                            Description = description,
                            Url = url,
                        };
                        models.Add(model);
                    }

                    var wnd = new JumperWnd(models, version);
                    wnd.ShowHostDialog();
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Exception", ex.Message);
                }
            }
            else
            {
                TaskDialog.Show("Warning", "No Selection");
            }
            return Result.Succeeded;
        }       
    }
}
