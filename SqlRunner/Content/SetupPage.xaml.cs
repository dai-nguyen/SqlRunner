﻿/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using FirstFloor.ModernUI.Windows;
using SqlRunner.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SqlRunner.Content
{   
    public partial class SetupPage : UserControl, IContent
    {
        DatabaseView _setting;

        public SetupPage()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            _setting.Password = txtPassword.Password;

            if (_setting.IsValid())
                _setting.Save();
        }        

        // Load
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            _setting = new DatabaseView();
            _setting.Load();
            txtPassword.Password = _setting.Password;
            this.DataContext = _setting;
        }

        // Unload
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            _setting = null;
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            //throw new System.NotImplementedException();
        }
    }
}
