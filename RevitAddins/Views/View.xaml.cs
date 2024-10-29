using System;
using System.Windows;
using Autodesk.Revit.UI;
using RevitAddins.ViewModels;

namespace RevitAddins.Views
{
    public partial class AddinView : Window
    {
        public AddinView(AddinViewModel addinViewModel) 
        {
            InitializeComponent();

            this.DataContext = addinViewModel;

            //setup events from viewmodel
            addinViewModel.ShowTaskDialogEvent += ShowTaskDialog;

            // Set window position with given margin to the right
            SetWindowPosition(50);

        }

        private void BringToFront()
        {
            this.Activate();
        }

        private void ShowTaskDialog(string title, string instruction, TaskDialogIcon icon)
        {
            TaskDialog taskDialog = new TaskDialog(title);
            taskDialog.MainInstruction = instruction;
            taskDialog.MainIcon = icon;
            taskDialog.Show();
        }


        private void SetWindowPosition(double rightMargin)
        {

            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            // Set window height-centered and right-positioned
            this.Left = screenWidth - this.Width - rightMargin;
            this.Top = (screenHeight / 2) - (this.Height / 2);
        }

        private void TextBox_LostFocus(object sender, EventArgs e)
        {
            if (DataContext is AddinViewModel viewModel)
            {
                if (viewModel.ExecuteCommandUpdateParameterValues.CanExecute(null))
                {
                    viewModel.ExecuteCommandUpdateParameterValues.Execute(null);
                }
            }
        }

    }
}

