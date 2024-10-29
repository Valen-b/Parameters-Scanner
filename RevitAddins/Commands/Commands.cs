using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using RevitAddins.Views;
using RevitAddins.ViewModels;


namespace RevitAddins.Commands
{
    
    //PARA QUE ES ESTO?
    [Transaction(TransactionMode.Manual)]
    public class ParameterScanner : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIApplication uiapp = commandData.Application;

            if ( IsAllowedTypeView(uiapp) )
            {

                //proceed to execute the add-in functionalities

                AddinViewModel addinViewModel = new AddinViewModel(commandData, uiapp);
                AddinView addinView = new AddinView(addinViewModel);
                addinView.Show();

                return Result.Succeeded;

            }
            else
            {

                TaskDialog taskDialog = new TaskDialog("Error: incorrect view type");
                taskDialog.MainInstruction = "This add-in is meant to be used within Floor Plans, Reflected Ceilings Plan or 3D Views.";
                taskDialog.MainIcon = TaskDialogIcon.TaskDialogIconError;
                taskDialog.Show();

                return Result.Failed;

            }

        }

        public static Boolean IsAllowedTypeView(UIApplication uiapp)
        {
            View activeView = uiapp.ActiveUIDocument.Document.ActiveView;

            List<ViewType> allowedViewTypes = new List<ViewType> { ViewType.FloorPlan, ViewType.CeilingPlan, ViewType.ThreeD };

            if (allowedViewTypes.Contains(activeView.ViewType))
            {
                return true;
            }

            return false;

        }

    }

}