/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using FirstFloor.ModernUI.Windows;
using SqlRunner.DataAccess;
using SqlRunner.ViewModels;
using System.Windows.Controls;

namespace SqlRunner.Content
{   
    public partial class SetupUserControl : UserControl, IContent
    {
        SetupUserControlViewModel _setupUserControlViewModel;

        public SetupUserControl()
        {
            InitializeComponent();           
        }
        
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            _setupUserControlViewModel = new SetupUserControlViewModel(new AlertMessageService(busyIndicator, txtStatus));
            this.DataContext = _setupUserControlViewModel;
        }
        
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
                        
        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            
        }
    }
}
