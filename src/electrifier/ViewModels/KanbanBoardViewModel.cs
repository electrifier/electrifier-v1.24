using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using electrifier.Contracts.Services;
using electrifier.Contracts.ViewModels;
//using electrifier.Core.Contracts.Services;
//using electrifier.Core.Models;

namespace electrifier.ViewModels;

public partial class KanbanBoardViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;
    //    private readonly ISampleDataService _sampleDataService;

    //    public ObservableCollection<SampleOrder> Source { get; } = new ObservableCollection<SampleOrder>();

    public KanbanBoardViewModel(INavigationService navigationService /*, ISampleDataService sampleDataService */)
    {
        _navigationService = navigationService;
        //_sampleDataService = sampleDataService;
    }

    [RelayCommand]
    private void OnItemClick(/*SampleOrder? clickedItem*/ object? clickedItem)
    {
        //if (clickedItem != null)
        //{
        //    _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
        //    _navigationService.NavigateTo(typeof(KanbanBoardDetailViewModel).FullName!, clickedItem.OrderID);
        //}
    }
    public void OnNavigatedFrom()
    {
    }

    public /* TODO: async */ void OnNavigatedTo(object parameter)
    {
        //Source.Clear();

        //// TODO: Replace with real data.
        //var data = await _sampleDataService.GetContentGridDataAsync();
        //foreach (var item in data)
        //{
        //    Source.Add(item);
        //}
    }
}
