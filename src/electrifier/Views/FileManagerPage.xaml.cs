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

using electrifier.ViewModels;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class FileManagerPage : Page
{
    #region ContentAreaBottomAppBar

    public int ItemCount => ViewModel.ItemCount;

    public int FolderCount => ViewModel.FolderCount;

    public bool HasFolders => FolderCount > 0;

    public int FileCount => ViewModel.FileCount;
    public bool HasFiles => FileCount > 0;

    #endregion

    public FileManagerViewModel ViewModel
    {
        get;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileManagerPage"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public FileManagerPage()
    {
        ViewModel = App.GetService<FileManagerViewModel>() ?? throw new InvalidOperationException();

        InitializeComponent();

        ShellGridView.ItemsSource = ViewModel.GridAdvancedCollectionView;
    }
}
