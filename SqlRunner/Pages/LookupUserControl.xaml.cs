/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using SqlRunner.DataAccess;
using SqlRunner.ViewModels;
using System.Windows.Controls;

namespace SqlRunner.Pages
{    
    public partial class LookupUserControl : UserControl
    {
        private readonly LookupUserControlViewModel _viewModel;

        public LookupUserControl()
        {
            InitializeComponent();

            _viewModel = new LookupUserControlViewModel(new AlertMessageService(busyIndicator, txtStatus));
            this.DataContext = _viewModel;
        }        
    }
}
