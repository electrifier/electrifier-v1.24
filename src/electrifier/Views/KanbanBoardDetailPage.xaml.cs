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

using CommunityToolkit.WinUI.UI.Animations;
using electrifier.Contracts.Services;
using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace electrifier.Views;

public sealed partial class KanbanBoardDetailPage : Page
{
    public KanbanBoardDetailViewModel ViewModel
    {
        get;
    }

    public KanbanBoardDetailPage()
    {
        ViewModel = App.GetService<KanbanBoardDetailViewModel>();
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        this.RegisterElementForConnectedAnimation("animationKeyContentGrid", itemHero);
    }

    protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        base.OnNavigatingFrom(e);
        if (e.NavigationMode == NavigationMode.Back)
        {
            //var navigationService = App.GetService<INavigationService>();
            //
            //if (ViewModel.Item != null)
            //{
            //    navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            //}
        }
    }
}
