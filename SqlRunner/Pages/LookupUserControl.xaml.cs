﻿/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using SqlRunner.DataAccess;
using SqlRunner.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Controls;

namespace SqlRunner.Pages
{    
    public partial class LookupUserControl : UserControl
    {
        CancellationTokenSource _source;
        ObservableCollection<SearchQueryFile> _results;
        QueryFileService _service;

        public LookupUserControl()
        {
            InitializeComponent();

            _source = new CancellationTokenSource();
            _results = new ObservableCollection<SearchQueryFile>();
            _service = new QueryFileService();
            gridResults.ItemsSource = _results;

            busyIndicator.IsIndeterminate = false;
        }

        private async void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            busyIndicator.IsIndeterminate = true;
            _source.Cancel();
            _results.Clear();

            TextBox text = sender as TextBox;

            _source.Dispose();
            _source = new CancellationTokenSource();

            if (!string.IsNullOrEmpty(text.Text)
                && !string.IsNullOrWhiteSpace(text.Text)
                && text.Text.Length > 1)
            {
                using (QueryFileService service = new QueryFileService())
                {
                    var result = await service.LookupAsync(text.Text, _source.Token);

                    if (result != null)
                    {
                        foreach (var r in result)
                        {
                            _results.Add(r);
                        }
                    }
                }
            }
            busyIndicator.IsIndeterminate = false;
        }
    }
}