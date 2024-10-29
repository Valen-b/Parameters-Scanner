using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Diagnostics;


namespace RevitAddins.Model
{
    public static class RevitAPIInteractor
    {

        public static Dictionary<string, IList<ElementId>> GetIdsForParameterValues(string parameterName, UIApplication uiapp, out bool parameterFound)
        {
            //boolean used to determine if a parameter with the especified name exists.
            parameterFound = false;

            var doc = uiapp.ActiveUIDocument.Document;

            //dictionary keys: unique values for the given parameter. Dictionary values: Lists of element IDs that match the parameter value stored in the key.
            Dictionary<string, IList<ElementId>> keyValuePairs = new Dictionary<string, IList<ElementId>>();


            //obtain instances of model elements
            IList<Element> modelInstances = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .Where(e => e.Category?.CategoryType == CategoryType.Model)
                .ToList();


            //populates the dictionary
            foreach (Element modelInstance in modelInstances)
            {
                IList<Parameter> parameters = modelInstance.GetParameters(parameterName);

                //each instance might contain more than one parameter with the same name
                foreach(Parameter parameter in parameters)
                {
                    parameterFound = true;

                    string key = parameter.AsValueString();
                    ElementId value = modelInstance.Id;

                    if(key == null){
                        key = "";
                    }

                    Debug.WriteLine($"key: {key}    value: {value}");

                    if (!keyValuePairs.ContainsKey(key))
                    {
                        keyValuePairs[key] = new List<ElementId>();
                    }

                    if (!keyValuePairs[key].Contains(value) )
                    {
                        keyValuePairs[key].Add(value);
                    }
                    
                }

            }

            return keyValuePairs;

        }

        public static void SelectElementsMethod(ICollection<ElementId> selectedElementIds, UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;

            uidoc.Selection.SetElementIds(selectedElementIds);

        }

        public static void IsolateElementsTemporaryMethod(ICollection<ElementId> selectedElementIds, UIApplication uiapp)
        {
            View activeView = uiapp.ActiveUIDocument.ActiveView;

            activeView.IsolateElementsTemporary(selectedElementIds);

        }
    }
}