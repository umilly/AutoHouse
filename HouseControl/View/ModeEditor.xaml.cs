﻿using System.Windows;
using Facade;
using ViewModel;
using VMBase;
using MessageBox = System.Windows.MessageBox;

namespace View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class ModeEditor
    {
        public ModeEditor(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }
    }
}