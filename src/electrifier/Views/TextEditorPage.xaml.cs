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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace electrifier.Views;

public sealed partial class TextEditorPage : Page
{
    public int TextEditorPageId
    {
        get;
    }

    public TextEditorViewModel ViewModel
    {
        get;
    }

    public string StatusCursorPosition => GetCursorPosition();
    private string GetCursorPosition() => ViewModel.StatusCursorPosition;

    public TextEditorPage()
    {
        ViewModel = App.GetService<TextEditorViewModel>();
        InitializeComponent();
    }

    private void CodeEditorControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Needs to set focus explicitly due to WinUI 3 regression https://github.com/microsoft/microsoft-ui-xaml/issues/8816 
        ((Control)sender).Focus(FocusState.Programmatic);
    }
}
