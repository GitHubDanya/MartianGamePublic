using Microsoft.AspNetCore.Components;

namespace ServerAlphaWebsite.Parsers
{
    public class UrlParameterParser
    {
        public string GetUrlParameter(string parameterName, NavigationManager NavigationManager)
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var query = uri.Query;
            var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query);
            return queryDictionary.ContainsKey(parameterName) ? queryDictionary[parameterName].ToString() : "";
        }
    }
}
