using System;
using System.Globalization;

public class CultureService
{
    public event Action? OnChange;

    private string _currentCulture = "en-US";
    public string CurrentCulture => _currentCulture;

    public void SetCulture(string culture)
    {
        if (_currentCulture == culture) return;

        _currentCulture = culture;

        var cultureInfo = new CultureInfo(culture);
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        OnChange?.Invoke(); // notify all subscribers
    }
}
