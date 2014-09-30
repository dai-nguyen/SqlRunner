using Microsoft.Practices.Prism.Mvvm;
using SqlRunner.DataAccess;
using SqlRunner.Models;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace SqlRunner.ViewModels
{
    public class LookupUserControlViewModel : BindableBase
    {
        private readonly IAlertMessageService _alertService;
        private readonly QueryFileService _queryFileService;
        private CancellationTokenSource _tokenSource;

        private string _searchKey;
        public string SearchKey
        {
            get { return _searchKey; }
            set { SetProperty(ref _searchKey, value); }
        }

        public ObservableCollection<SearchQueryFile> Results { get; private set; }        

        public LookupUserControlViewModel(IAlertMessageService alertService)
        {
            _alertService = alertService;
            _queryFileService = new QueryFileService();
            _tokenSource = new CancellationTokenSource();
            Results = new ObservableCollection<SearchQueryFile>();            
        }

        private async Task SearchAsync()
        {
            _alertService.ShowMessage(true, "Searching...");
            _tokenSource.Cancel();
            Results.Clear();
            _tokenSource.Dispose();
            _tokenSource = new CancellationTokenSource();

            var results = await _queryFileService.LookupAsync(SearchKey, _tokenSource.Token);

            if (results != null)
            {
                foreach (var r in results)
                {
                    Results.Add(r);
                }
            }
            _alertService.ShowMessage(false, "Ready");
        }
    }
}
