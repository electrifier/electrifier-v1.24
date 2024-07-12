using CommunityToolkit.Mvvm.ComponentModel;
using electrifier.Contracts.ViewModels;

namespace electrifier.ViewModels;

public partial class KanbanBoardDetailViewModel : ObservableRecipient, INavigationAware
{
    //private internal readonly ISampleDataService _sampleDataService;

    //[ObservableProperty]
    //private SampleOrder? item;

    public KanbanBoardDetailViewModel(/* ISampleDataService sampleDataService */)
    {
        //_sampleDataService = sampleDataService;
    }

    public void OnNavigatedFrom()
    {
    }

    public /* TODO: async */void OnNavigatedTo(object parameter)
    {
        //if (parameter is long orderID)
        //{
        //    var data = await _sampleDataService.GetContentGridDataAsync();
        //    Item = data.First(i => i.OrderID == orderID);
        //}
    }
}
