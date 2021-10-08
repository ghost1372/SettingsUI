﻿using Microsoft.UI.Xaml.Controls;
using SettingsUI.Demo.Pages;
using SettingsUI.ViewModel;

namespace SettingsUI.Demo
{
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            this.InitializeComponent();
            ViewModel.Initialize(shellFrame, navigationView, autoSuggestBox, KeyboardAccelerators);
            shellFrame.Navigate(typeof(GeneralPage));
        }

        private void UserControl_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.OnLoaded();
        }

        private void navigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ViewModel.OnItemInvoked(args);
        }

        private void OnControlsSearchBoxQuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
           ViewModel.OnAutoSuggestBoxQuerySubmitted(args);
        }

        private void OnControlsSearchBoxTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            ViewModel.OnAutoSuggestBoxTextChanged(args);
        }
    }
}