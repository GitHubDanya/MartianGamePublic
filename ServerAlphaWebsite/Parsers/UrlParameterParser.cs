using Microsoft.AspNetCore.Components;

namespace ServerAlphaWebsite.Parsers
{
    public class UrlParameterParser
    {
        private NavigationManager navigationManager;

        public UrlParameterParser(NavigationManager navigationManager)
        {
            this.navigationManager = navigationManager;
        }

        public string GetUrlParameter(string parameterName)
        {
            var uri = navigationManager.ToAbsoluteUri(navigationManager.Uri);
            var query = uri.Query;
            var queryDictionary = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(query);
            return queryDictionary.ContainsKey(parameterName) ? queryDictionary[parameterName].ToString() : "";
        }
    }
}
