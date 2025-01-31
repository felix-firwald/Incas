using Incas.Core.Views.Controls;
using Incas.Core.Views.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Incas.Core.Classes
{
    public static class ProgramStatusBar
    {
        private static DialogLoading DialogLoading { get; set; }
        private static LoadingBox LoadingBox { get; set; }
        private static void InitializeLoadingBox()
        {
            LoadingBox = new();
            ProgramState.MainWindow.PlaceStatusBar(LoadingBox);
        }
        private static void InitializeLoadingWindow(string name, string description)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                DialogLoading = new(name, description);
                DialogLoading.Show();
            });
        }
        public static void ShowLoadingWindow(string name, string description)
        {
            InitializeLoadingWindow(name, description);          
        }
        public static void SetText(string text)
        {
            
            ProgramState.MainWindow?.Dispatcher.Invoke(new Action(() =>
            {
                if (LoadingBox is null)
                {
                    InitializeLoadingBox();
                }
                ProgramStatusBar.LoadingBox.SetText(text);
            }));
            Thread.Sleep(10);
        }
        public static void HideLoadingWindow()
        {
            if (DialogLoading is null)
            {
                return;
            }
            DialogLoading.Dispatcher.Invoke(new Action(() =>
            {
                DialogLoading.Close();
                DialogLoading = null;
            }));
        }
        public static void Hide()
        {
            if (ProgramState.MainWindow is null)
            {                
                return;
            }
            ProgramState.MainWindow.Dispatcher.Invoke(new Action(() =>
            {
                LoadingBox = null;
                ProgramState.MainWindow.NullifyStatusBar();
            }));
        }
    }
}
