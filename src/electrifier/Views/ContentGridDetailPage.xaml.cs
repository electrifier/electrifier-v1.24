﻿using CommunityToolkit.WinUI.UI.Animations;

using electrifier.Contracts.Services;
using electrifier.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace electrifier.Views;

public sealed partial class ContentGridDetailPage : Page
{
    public ContentGridDetailViewModel ViewModel
    {
        get;
    }

    public ContentGridDetailPage()
    {
        ViewModel = App.GetService<ContentGridDetailViewModel>();
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
            var navigationService = App.GetService<INavigationService>();

            if (ViewModel.Item != null)
            {
                navigationService.SetListDataItemForNextConnectedAnimation(ViewModel.Item);
            }
        }
    }
}
