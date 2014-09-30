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
using System.Linq;
using System.IO;
using System.Text;
using System;
using Microsoft.Win32;
using ClosedXML.Excel;

namespace SqlRunner.Pages
{
    public partial class RunUserControl : UserControl, IContent
    {
        CancellationTokenSource _source;
        RunUserControlViewModel _file;
        ModernDialog _dialog;
        DataTable _dataTable;

        public RunUserControl()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            _dialog = new ModernDialog();
            _dialog.Title = "Lookup";
            _dialog.Buttons = new Button[] { _dialog.OkButton, _dialog.CancelButton };
            _dialog.Content = new LookupUserControl();
            _dialog.OkButton.Click += LookupOkButton_Click;
            _dialog.ShowDialog();            
        }

        void LookupOkButton_Click(object sender, RoutedEventArgs e)
        {
            var page = _dialog.Content as LookupUserControl;
            var result = page.gridResults.SelectedItem as SearchQueryFile;

            // Load Result
            if (result != null)
            {
                ClearDataTable();
                _file.LoadFromSearchResult(result);
            }
        }

        private void ClearDataTable()
        {
            _dataTable.Clear();
            _dataTable.Columns.Clear();
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SetBusyIndicator(true, "Please wait...");
            if (await _file.CanSaveAsync() && await _file.SaveSync())
                SetBusyIndicator(false, "Saved");
            else
                SetBusyIndicator(false, "Failed");            
        }

        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            SetBusyIndicator(true, "Please wait...");
            
            if (await _file.DeleteAsync())            
                SetBusyIndicator(false, "Deleted");
            else
                SetBusyIndicator(false, "Failed");

            ClearDataTable();
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

        private async void btnRun_Click(object sender, RoutedEventArgs e)
        {            
            SetBusyIndicator(true, "Please wait...");

            if (btnRun.Tag.ToString() == "Run")
            {
                btnRun.Tag = "Stop";
                btnRun.IconData = Geometry.Parse(Properties.Resources.IconStop);

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

                btnRun.IconData = Geometry.Parse(Properties.Resources.IconRun);
                btnRun.Tag = "Run";
            }
            else
            {
                _source.Cancel();
                btnRun.IconData = Geometry.Parse(Properties.Resources.IconRun);
                btnRun.Tag = "Run";
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
                                if (!string.IsNullOrEmpty(param.Key) && !string.IsNullOrEmpty(param.Value))
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

        private async void btnExport_Click(object sender, RoutedEventArgs e)
        {
            if (_dataTable == null)
                return;

            SetBusyIndicator(true, "Please wait...");

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = !string.IsNullOrEmpty(_file.Name) ? _file.Name : "export";
            dialog.DefaultExt = ".xlsx";
            dialog.Filter = "Excel (.xlsx)|*.xlsx";

            if (dialog.ShowDialog() == true)
            {
                await Task.Run(() =>
                    {

                        var workbook = new XLWorkbook();
                        var worksheet = workbook.Worksheets.Add("Exported Data");

                        worksheet.Cell("A1").Value = DateTime.Now;
                        worksheet.Cell("A2").Value = _file.Name;

                        // columns
                        for (int col = 0; col < _dataTable.Columns.Count; col++)
                        {
                            var col_cell = worksheet.Cell(4, col + 1);
                            col_cell.Value = _dataTable.Columns[col].ColumnName;
                            col_cell.Style.Font.Bold = true;
                            col_cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                        }

                        // rows
                        for (int row = 0; row < _dataTable.Rows.Count; row++)
                        {
                            for (int col = 0; col < _dataTable.Columns.Count; col++)
                            {
                                worksheet.Cell(row + 5, col + 1).Value = _dataTable.Rows[row][col];
                            }
                        }

                        worksheet.Columns().AdjustToContents();
                        workbook.SaveAs(dialog.FileName);                        
                    });
            }

            SetBusyIndicator(false, "Done");
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
            _file = new RunUserControlViewModel();
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
            ClearDataTable();
            _dataTable = null;
        }

        private void SetBusyIndicator(bool busy, string msg)
        {
            txtStatus.Text = msg;
            busyIndicator.IsIndeterminate = busy ? true : false;

            btnOpen.IsEnabled = !busy;            
            btnDelete.IsEnabled = !busy;
            btnEdit.IsEnabled = !busy;
            btnSave.IsEnabled = !busy;
            btnExport.IsEnabled = !busy;
        }

        private void txtSql_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void txtSql_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void txtSql_PreviewDrop(object sender, DragEventArgs e)
        {
            _dataTable.Clear();
            string file = "";

            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                object text = e.Data.GetData(DataFormats.FileDrop);
                string[] files = (string[])text;
                string f = files.FirstOrDefault(t => Path.GetExtension(t).ToLower() == ".sql");

                if (f != null)
                    _file.LoadFromDropFile(File.ReadAllText(f));
            }
            else if (e.Data.GetDataPresent("FileGroupDescriptor")) // drop from outlook
            {
                // http://www.codeproject.com/Articles/7140/Drag-and-Drop-Attached-File-From-Outlook-and-ab

                using (Stream theStream = (Stream)e.Data.GetData("FileGroupDescriptor"))
                {
                    byte[] fileGroupDescriptor = new byte[512];
                    theStream.Read(fileGroupDescriptor, 0, 512);
                    StringBuilder fileName = new StringBuilder();
                    for (int i = 76; fileGroupDescriptor[i] != 0; i++)
                    {
                        fileName.Append(Convert.ToChar(fileGroupDescriptor[i]));
                    }

                    if (Path.GetExtension(fileName.ToString()).ToLower() != ".sql")
                        return;
                }

                file = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".sql");

                using (MemoryStream ms = (MemoryStream)e.Data.GetData("FileContents", true))
                {
                    byte[] fileBytes = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(fileBytes, 0, (int)ms.Length);

                    using (FileStream fs = new FileStream(file, FileMode.Create))
                    {
                        fs.Write(fileBytes, 0, (int)fileBytes.Length);
                    }
                }

                if (!string.IsNullOrEmpty(file) && File.Exists(file))
                {
                    _file.LoadFromDropFile(File.ReadAllText(file));
                }
            }
        }
    }
}
