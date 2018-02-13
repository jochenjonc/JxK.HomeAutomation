using System;
using Windows.Storage;

namespace JxK.HomeAutomation.Controllers
{
    internal class SettingsController
    {
        ApplicationDataContainer _localSettings;

        public SettingsController()
        {
            _localSettings = ApplicationData.Current.LocalSettings;
        }

        public DateTimeOffset? PreviousTime
        {
            get => _localSettings.Values["#PreviousTime"] as DateTimeOffset?;
            set => _localSettings.Values["#PreviousTime"] = value;
        }

        public DateTimeOffset? LastOpeningTime
        {
            get => _localSettings.Values["#LastOpeningTime"] as DateTimeOffset?;
            set => _localSettings.Values["#LastOpeningTime"] = value;
        }

        public DateTimeOffset? LastClosingTime
        {
            get => _localSettings.Values["#LastClosingTime"] as DateTimeOffset?;
            set => _localSettings.Values["#LastClosingTime"] = value;
        }
    }
}