/** 
 * This file is part of the SqlRunner project.
 * Copyright (c) 2014 Dai Nguyen
 * Author: Dai Nguyen
**/

using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using SqlRunner.Models;
using SqlRunner.ViewModels;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SqlRunner.Pages
{
    public partial class RunPage : UserControl, IContent
    {
        CancellationTokenSource _source;
        SqlFileViewModel _file;
        ModernDialog _dialog;
        DataTable _dataTable;

        public RunPage()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            _dialog = new ModernDialog();
            _dialog.Title = "Lookup";
            _dialog.Buttons = new Button[] { _dialog.OkButton, _dialog.CancelButton };
            _dialog.Content = new LookupPage();
            _dialog.OkButton.Click += LookupOkButton_Click;
            _dialog.ShowDialog();            
        }

        void LookupOkButton_Click(object sender, RoutedEventArgs e)
        {
            var page = _dialog.Content as LookupPage;
            var result = page.gridResults.SelectedItem as SearchQueryFile;

            // Load Result
            if (result != null)
                _file.LoadFromSearchResult(result);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SetBusyIndicator(true, "Please wait...");
            if (await _file.CanSaveAsync() && await _file.SaveSync())
                SetBusyIndicator(false, "Saved");
            else
                SetBusyIndicator(false, "Failed");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (txtSql.IsReadOnly)
            {
                txtSql.IsReadOnly = false;
                btnEdit.IconData = Geometry.Parse(Properties.Resources.IconThumbUp);
            }
            else
            {
                txtSql.IsReadOnly = true;
                btnEdit.IconData = Geometry.Parse(Properties.Resources.IconEdit);
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            _source.Cancel();
        }

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {
            SetBusyIndicator(true, "Please wait...");

            _source.Cancel();
            _source.Dispose();
            _source = new CancellationTokenSource();
            _dataTable.Clear();

            if (!string.IsNullOrEmpty(_file.Content) 
                && !string.IsNullOrWhiteSpace(_file.Content))
            {
                var dt = await GetDataTableAsync();

                if (dt != null)
                {
                    _dataTable = dt;
                    gridResult.DataContext = _dataTable.DefaultView;
                }
            }

            SetBusyIndicator(false, "Done");
        }

        private async Task<DataTable> GetDataTableAsync()
        {
            string connStr = string.Format("server={0};database={1};user id={2};password={3}", 
                Properties.DbSettings.Default.Server, 
                Properties.DbSettings.Default.Database, 
                Properties.DbSettings.Default.User, 
                Properties.DbSettings.Default.Password);

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connStr);
            builder.AsynchronousProcessing = true;

            using (SqlConnection conn = new SqlConnection(builder.ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(_file.Content, conn))
                {
                    try
                    {
                        await conn.OpenAsync(_source.Token);

                        if (_file.Params.Count > 0)
                        {
                            foreach (var param in _file.Params)
                            {
                                cmd.Parameters.AddWithValue(param.Key, param.Value);
                            }
                        }

                        using (var reader = await cmd.ExecuteReaderAsync(_source.Token))
                        {
                            var td = new DataTable();
                            td.Load(reader);
                            return td;
                        }
                    }
                    catch { }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            return null;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {

        }

        public void OnFragmentNavigation(FirstFloor.ModernUI.Windows.Navigation.FragmentNavigationEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void OnNavigatedFrom(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            //throw new NotImplementedException();
        }

        // Load
        public void OnNavigatedTo(FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs e)
        {
            _source = new CancellationTokenSource();
            _file = new SqlFileViewModel();
            this.DataContext = _file;
            _dataTable = new DataTable();
            SetBusyIndicator(false, "Ready");
        }

        // Unload
        public void OnNavigatingFrom(FirstFloor.ModernUI.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            _source.Cancel();
            _source.Dispose();
            _file = null;
            _dialog = null;
        }

        private void SetBusyIndicator(bool busy, string msg)
        {
            txtStatus.Text = msg;
            busyIndicator.IsIndeterminate = busy ? true : false;
        }
    }
}
