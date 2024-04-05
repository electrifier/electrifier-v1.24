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
