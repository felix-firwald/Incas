﻿using Incas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Incas.Admin.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ClassPrimaryCreator.xaml
    /// </summary>
    public partial class ClassPrimaryCreator : Window
    {
        public ClassPrimaryCreatorViewModel ViewModel { get; set; }
        public ClassPrimaryCreator()
        {
            this.InitializeComponent();
            this.ViewModel = new();
            this.ViewModel.OnFinished += this.ViewModel_OnFinished;
            this.DataContext = this.ViewModel;
        }

        private void ViewModel_OnFinished()
        {
            this.DialogResult = true;
            this.Close();
        }
    }
}
