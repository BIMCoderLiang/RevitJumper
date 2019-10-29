using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace RevitJumper
{
    public class ExternalApplication : IExternalApplication
    {
        private static readonly string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string resourceImageFolder = $"/{assemblyName};component/Resources/Image/";
        public static string ProjectName => "Revit Jumper";
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            if (application?.ControlledApplication == null)
            {
                return Result.Failed;
            }

            var panel = application.CreateRibbonPanel(ProjectName);
            var button = CreatePushButton(panel, "Jumper", "Jumper");
            AddButtonTip(button as RibbonItem, "", "");
            return Result.Succeeded;
        }

        private PushButton CreatePushButton(RibbonPanel panel, string name, string text)
        {
            if (null == panel)
            {
                return null;
            }
            PushButtonData pushButtonData = CreatePushButtonData(name, text);
            PushButton pushButton = panel.AddItem(pushButtonData) as PushButton;
            pushButton.LargeImage = LoadImage(name);
            return pushButton;
        }

        private PushButtonData CreatePushButtonData(string name, string text)
        {
            if (null == name || null == text)
            {
                return null;
            }
            string assemblyFilePath = Assembly.GetExecutingAssembly().Location;
            string className = $"{assemblyName}.{name}Command";
            PushButtonData pushButtonData = new PushButtonData(name, text, assemblyFilePath, className);
            return pushButtonData;
        }

        private void AddButtonTip(RibbonItem item, string tooltip, string longDescription)
        {
            item.ToolTip = tooltip;
            item.LongDescription = longDescription;
        }

        private BitmapImage LoadImage(string imageName)
        {
            if (null == imageName)
            {
                return null;
            }
            string imageUriString = $"{resourceImageFolder}{imageName}.png";
            Uri imageUri = new Uri(imageUriString, UriKind.Relative);
            BitmapImage bitmapImage = new BitmapImage(imageUri);
            return bitmapImage;
        }
    }
}
