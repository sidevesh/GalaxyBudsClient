using System;
using System.Collections;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using GalaxyBudsClient.Interface.Dialogs;
using GalaxyBudsClient.Interface.Pages;
using GalaxyBudsClient.Model.Constants;
using GalaxyBudsClient.Platform;
using GalaxyBudsClient.Utils;
using GalaxyBudsClient.Utils.Interface.DynamicLocalization;

namespace GalaxyBudsClient.Interface.Developer
{
    public sealed class TranslatorTools : Window
    {
        private class ViewModel
        {
            public IEnumerable PagesSource => Enum.GetValues(typeof(AbstractPage.Pages)).Cast<AbstractPage.Pages>().ToList();
            public IEnumerable LocaleSource => Enum.GetValues(typeof(Locales)).Cast<Locales>().ToList();
            public IEnumerable ModelSource => Enum.GetValues(typeof(Model.Constants.Models)).Cast<Model.Constants.Models>().ToList();
        }

        private readonly CheckBox _ignoreConnLoss;
        private readonly CheckBox _dummyDevices;
        private readonly ComboBox _pages;
        private readonly ComboBox _locales;
        private readonly TextBox _xamlPath;

        private readonly ViewModel _vm = new ViewModel();

        public TranslatorTools()
        {
            DataContext = _vm;
            AvaloniaXamlLoader.Load(this);
            this.AttachDevTools();

            _ignoreConnLoss = this.GetControl<CheckBox>("IgnoreConnLoss");
            _dummyDevices = this.GetControl<CheckBox>("DummyDevices");
            _pages = this.GetControl<ComboBox>("Pages");
            _locales = this.GetControl<ComboBox>("Locales");
            _xamlPath = this.GetControl<TextBox>("XamlPath");

            _locales.SelectedItem = SettingsProvider.Instance.Locale;
            _xamlPath.Text = Loc.GetTranslatorModeFile();
            _ignoreConnLoss.IsChecked = BluetoothImpl.Instance.SuppressDisconnectionEvents;
            _dummyDevices.IsChecked = MainWindow.Instance.DeviceSelectionPage.EnableDummyDevices;
            
            Loc.ErrorDetected += (title, content) =>
            {
                new MessageBox
                {
                    Title = title, 
                    Description = content
                }.ShowDialog(this);
            };
        }

        private void GoToPage_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_pages.SelectedItem is AbstractPage.Pages page)
            {
                MainWindow.Instance.Pager.SwitchPage(page);
            }
        }

        private void ReloadXaml_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_locales.SelectedItem is Locales locale)
            {
                SettingsProvider.Instance.Locale = locale;
            }

            Loc.Load();
        }

        private void IgnoreConnLoss_OnChecked(object? sender, RoutedEventArgs e)
        {
            BluetoothImpl.Instance.SuppressDisconnectionEvents = _ignoreConnLoss.IsChecked ?? false;
        }

        private void DummyDevices_OnChecked(object? sender, RoutedEventArgs e)
        {
            MainWindow.Instance.DeviceSelectionPage.EnableDummyDevices = _dummyDevices.IsChecked ?? false;
        }
    }
}