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
                    searchcontent = target.GetType().Name;
                }                                              
            }
            var query = new Query();
            var models = new List<DisplayModel>();
            try
            {                
                if (!string.IsNullOrEmpty(searchcontent))
                {
                    var array = query.GetSearchResult(searchcontent);
                    if (array != null)
                    {
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
                    }                    
                }               
            }
            catch
            {

            }
            finally
            {
                var wnd = new JumperWnd(models, searchcontent, version);
                wnd.ShowHostDialog();
            }
            return Result.Succeeded;
        }       
    }
}
