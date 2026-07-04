using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.DB;
using ServerAlphaWebsite.Forms;

namespace ServerAlphaWebsite.Pages.api.Components
{
    public partial class DataDisplay : ComponentBase
    {
        ApiSearchForm apiForm = new ApiSearchForm() { Table = "conversations" };
        List<object> Data = new List<object>();
        Dictionary<string, HiddenKeyFormResponse> HiddenProperties;

        private string SortByProperty { get; set; } = "id";

        //List<object> Data;

        public DataDisplay()
        {
            Data = GetTableData(apiForm).ToList();
            Data = FilterDataByUsername(Data, apiForm.Username);

            HiddenProperties = InitializeFormHiddenValues(Data);
        }

        private void Submit()
        {
            Data = GetTableData(apiForm).ToList();
            Data = FilterDataByUsername(Data, apiForm.Username);

            HiddenProperties = InitializeFormHiddenValues(Data);

            StateHasChanged();
        }

        private List<object> FilterDataByUsername(List<object> data, string username)
        {
            if (username == "")
            {
                return data;
            }

            username = username.Trim('\t');

            List<object> filteredData = new List<object>();
            try
            {
                foreach (object dataItem in data)
                {
                    if (dataItem is IDictionary<string, object> dict)
                    {
                        IDictionary<string, object> dataItemDict = dataItem as IDictionary<string, object>;
                        if (dataItemDict != null && dataItemDict.ContainsKey("userid") && (string)dataItemDict["userid"] == username)
                        {
                            filteredData.Add(dataItemDict as object);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return filteredData;
        }

        private IEnumerable<object> GetTableData(ApiSearchForm apiform)
        {
            try
            {
                DbCommunicationProvider dbCommunicationProvider = new DbCommunicationProvider();
                return apiform.Table switch
                {
                    "conversations" => dbCommunicationProvider.FetchConversations()
                        .Where(x => x != null)
                        .OrderBy(x => GetDynamicPropertyValue(x, SortByProperty))
                        .Cast<object>(),

                    "answers" => dbCommunicationProvider.FetchAnswers()
                        .Where(x => x != null)
                        .OrderBy(x => GetDynamicPropertyValue(x, SortByProperty))
                        .Cast<object>(),

                    "forms" => dbCommunicationProvider.FetchPersonalInfo()
                        .Where(x => x != null)
                        .OrderBy(x => GetDynamicPropertyValue(x, SortByProperty))
                        .Cast<object>(),

                    _ => Enumerable.Empty<object>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<object>();
            }
        }

        private object GetDynamicPropertyValue(dynamic obj, string propertyName)
        {
            if (obj == null) return null;

            if (obj is IDictionary<string, object> dict && dict.ContainsKey(propertyName))
                return dict[propertyName];

            try
            {
                return ((object)obj).GetType().GetProperty(propertyName)?.GetValue(obj, null);
            }
            catch
            {
                return null;
            }
        }

        private List<string> GetPropertiesOfData(List<object> data)
        {
            var firstRow = data.FirstOrDefault();

            if (firstRow == null) { return new List<string>(); }

            IDictionary<string, object> firstRowDictionary = (IDictionary<string, object>)firstRow;
            List<string> properties = new List<string>();

            foreach (KeyValuePair<string, object> pair in firstRowDictionary)
            {
                properties.Add(pair.Key);
            }

            return properties;
        }

        private Dictionary<string, HiddenKeyFormResponse> InitializeFormHiddenValues(List<object> _data)
        {
            List<string> properties = GetPropertiesOfData(_data);
            Dictionary<string, HiddenKeyFormResponse> hiddenValues = new Dictionary<string, HiddenKeyFormResponse>();

            foreach (var property in properties)
            {
                hiddenValues[property] = new HiddenKeyFormResponse()
                {
                    Value = false
                };
            }

            return hiddenValues;
        }
    }

    public class HiddenKeyFormResponse
    {
        public bool Value { get; set; }
    }
}
