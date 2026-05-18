using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

public class NavigationHistoryService : IDisposable
{
	private readonly NavigationManager _navigationManager;
	private List<string> navigationHistory = new List<string>();

	public string? PreviousPage { get; private set; }
	public string? CurrentPage { get; private set; }


	public NavigationHistoryService(NavigationManager navigationManager)
	{
		_navigationManager = navigationManager;
		_navigationManager.LocationChanged += HandleLocationChanged;
		navigationHistory.Add(_navigationManager.Uri); // Add the initial page
		PreviousPage = null; // Set initial previous page
	}

	private void HandleLocationChanged(object? sender, LocationChangedEventArgs e)
	{
		var currentUri = _navigationManager.Uri;

		PreviousPage = CurrentPage;
		CurrentPage = currentUri;
	}

	public void Dispose()
	{
		_navigationManager.LocationChanged -= HandleLocationChanged;
	}
}