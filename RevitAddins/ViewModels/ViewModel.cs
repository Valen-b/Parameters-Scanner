using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAddins.ViewModels
{
    public class AddinViewModel : INotifyPropertyChanged
    {
        private string parameterName;
        private string selectedParameterValue;
        private Dictionary<string, IList<ElementId>> parameterValuesAndIds;
        private ObservableCollection<string> parameterValues = new ObservableCollection<string>();
        private Boolean parameterFound;

        private ExternalCommandData commandData;
        private UIApplication uiapp;
        private UIDocument uidoc;

        public IRelayCommand ExecuteCommandSelect { get; }
        public IRelayCommand ExecuteCommandisolate { get; }
        public IRelayCommand ExecuteCommandUpdateParameterValues { get; }

        //constructor
        public AddinViewModel(ExternalCommandData commandData, UIApplication uiapp)
        {
            ExecuteCommandSelect = new RelayCommand(SelectMethod);
            ExecuteCommandisolate = new RelayCommand(IsolateMethod);
            ExecuteCommandUpdateParameterValues = new RelayCommand(UpdateParameterValues);

            this.commandData = commandData;
            this.uiapp = uiapp;
            this.uidoc = uiapp.ActiveUIDocument;

        }

        public string ParameterName
        {
            get => parameterName;
            set
            {
                if (parameterName != value)
                {
                    parameterName = value;
                    OnPropertyChanged(); // Notify change
                }
            }
        }

        public string SelectedParameterValue
        {
            get => selectedParameterValue;
            set
            {
                if (selectedParameterValue != value)
                {
                    selectedParameterValue = value;
                    OnPropertyChanged(); // Notify change
                }
            }
        }

        public ObservableCollection<string> ParameterValues
        {
            get => parameterValues;
            set
            {
                parameterValues = value;
                OnPropertyChanged(); // Notify change
            }
        }

        
        private void UpdateParameterValues()
        {
            string previousText = SelectedParameterValue;

            //necessary to delete values that were assigned in a previous call to this method
            ParameterValues.Clear();

            //This restores SelectedParameterValue value after being deleted by ParameterValues.Clear(), because the user may want to look for the same value with another parameter.
            SelectedParameterValue = previousText;


            // Logic to update ParameterValues based on ParameterName
            parameterValuesAndIds = Model.RevitAPIInteractor.GetIdsForParameterValues(ParameterName, uiapp, out parameterFound);

            ParameterValues = new ObservableCollection<string>(parameterValuesAndIds.Keys);

        }

        //method referenced by ExecuteCommandSelect command
        private void SelectMethod()
        {

            ICollection<ElementId> selectedElementIds = DialogLogic();

            Model.RevitAPIInteractor.SelectElementsMethod(selectedElementIds, uiapp);

        }

        //method referenced by ExecuteCommandisolate command
        private void IsolateMethod()
        {
            ICollection<ElementId> selectedElementIds = DialogLogic();

            if (selectedElementIds.Count > 0)
            {
                Model.RevitAPIInteractor.IsolateElementsTemporaryMethod(selectedElementIds, uiapp);
            }
            
        }

        //DialogLogic method executes an UI method in the view.cs file, thats why the ShowTaskDialogEvent event was created.
        public event Action<string, string, TaskDialogIcon> ShowTaskDialogEvent;

        //DialogLogic implements a series of logical gates to inform the user about the validity of the inputs, and to provide an ICollection<ElementId> named selectedElementIds. This contains Ids of model instances that will be either selected or isolated.
        private ICollection<ElementId> DialogLogic()
        {

            ICollection<ElementId> selectedElementIds = new List<ElementId>();

            if (ParameterName == "" || ParameterName == null)
            {
                ShowTaskDialogEvent.Invoke("Paramenter Name field is blank",
                    "Write a name in the Paramenter Name field before selecting elements.",
                    TaskDialogIcon.TaskDialogIconWarning);
            }
            else if (parameterFound == false)
            {
                ShowTaskDialogEvent.Invoke("The specified parameter doesn't exist",
                    $"No model elements contain a parameter named \"{ParameterName}\"",
                    TaskDialogIcon.TaskDialogIconWarning);
            }
            else
            {
                string noNullSelectedParameterValue = "";

                if (SelectedParameterValue != null)
                {
                    noNullSelectedParameterValue = SelectedParameterValue;
                }

                if (parameterValuesAndIds.ContainsKey(noNullSelectedParameterValue))
                {
                    selectedElementIds = parameterValuesAndIds[noNullSelectedParameterValue];

                    int selectedElementsCount = selectedElementIds.Count;

                    if (selectedElementsCount > 0)
                    {
                        

                        if (selectedElementsCount == 1)
                        {
                            ShowTaskDialogEvent.Invoke("Results",
                                $"1 element was found with its parameter value equal to \"{noNullSelectedParameterValue}\"",
                                TaskDialogIcon.TaskDialogIconInformation);
                        }
                        else
                        {
                            ShowTaskDialogEvent.Invoke("Results",
                                $"{selectedElementIds.Count} elements were found with the parameter value equal to \"{noNullSelectedParameterValue}\"",
                                TaskDialogIcon.TaskDialogIconInformation);
                        }

                        return selectedElementIds;

                    }

                }
                else
                {
                    ShowTaskDialogEvent.Invoke("Results",
                        $"No elementds were found with the parameter value equal to \"{noNullSelectedParameterValue}\"",
                        TaskDialogIcon.TaskDialogIconInformation);
                }
            }

            return selectedElementIds;

        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}