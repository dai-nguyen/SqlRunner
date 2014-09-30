using System.Windows.Controls;

namespace SqlRunner.DataAccess
{
    public class AlertMessageService : IAlertMessageService
    {
        ProgressBar _progressBar;
        TextBlock _txtStatus;

        public AlertMessageService(ProgressBar progressBar, TextBlock txtStatus)
        {
            _progressBar = progressBar;
            _txtStatus = txtStatus;
        }

        public void ShowMessage(string msg)
        {
            _txtStatus.Text = msg;
        }

        public void ShowMessage(bool busy, string msg)
        {
            _progressBar.IsIndeterminate = busy;
            _txtStatus.Text = msg;
        }
    }
}
