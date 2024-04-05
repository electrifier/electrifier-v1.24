/*
    Copyright 2024 Thorsten Jung, aka tajbender
        https://www.electrifier.org

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using electrifier.Contracts.Services;
using electrifier.Contracts.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
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

    public async void OnNavigatedTo(object parameter)
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
