using Incas.Core.Views.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Incas.Core.Classes
{
    public static class ProgramStatusBar
    {
        private static LoadingBox LoadingBox { get; set; }
        private static void InitializeLoadingBox()
        {
            LoadingBox = new();
            ProgramState.MainWindow.PlaceStatusBar(LoadingBox);
        }
        public static void SetText(string text)
        {
            
            ProgramState.MainWindow.Dispatcher.Invoke(new Action(() =>
            {
                if (LoadingBox is null)
                {
                    InitializeLoadingBox();
                }
                ProgramStatusBar.LoadingBox.SetText(text);
            }));
            Thread.Sleep(10);
            //if (LoadingBox is null)
            //{
            //    InitializeLoadingBox();
            //}
            //ProgramStatusBar.LoadingBox.SetText(text);
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
