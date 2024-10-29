using System;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;


namespace RevitAddins
{
    public static class Constants
    {
        public const double EPS = 0.0001;
    }

    public static class MathHelper
    {
        public static bool IsCloseToZero(double a)
        {
            return Math.Abs(a) < Constants.EPS;
        }
    }

    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {

            // Creating a ribbon tab named "Parameters"
            string tabName = "Parameters";
            application.CreateRibbonTab(tabName);

            //Creating a ribbon panel named "Parameters"
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "Parameters");
            
            //Creating a button named "Parameter Scanner" whithin the ribbon
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData pushButtonData = new PushButtonData("ParameterScannerCommand", "Parameter Scanner", thisAssemblyPath, "RevitAddins.Commands.ParameterScanner");
            PushButton pushButton = ribbonPanel.AddItem(pushButtonData) as PushButton;

            //settiing an image for the button
            string iconFilePath = Path.Combine(Path.GetDirectoryName(thisAssemblyPath), "Resources", "icons8-parameter-32.png");
            Uri iconUri = new Uri(iconFilePath);
            BitmapImage bitmapImage = new BitmapImage(iconUri);
            pushButton.LargeImage = bitmapImage;

            return Result.Succeeded;

        }
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

    }

}