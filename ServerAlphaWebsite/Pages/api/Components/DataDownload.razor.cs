using ServerAlphaWebsite.Forms;
using ServerAlphaWebsite.DB;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;

namespace ServerAlphaWebsite.Pages.api.Components
{
	public partial class DataDownload
	{
		[Inject] private IJSRuntime JS { get; set; } = default!;

		ApiDownloadForm apiForm = new ApiDownloadForm() { Table = "conversations" };

		public async Task Submit()
		{
			SQLTable table;
			table = apiForm.Table switch
			{
				"conversations" => SQLTable.conversations,
				"answers" => SQLTable.answers,
				"forms" => SQLTable.personalinfo,
				_ => throw new ArgumentException("Invalid table name", nameof(apiForm.Table))
			};

			await DownloadCSVFromStream(table);
		}

		public async Task DownloadCSVFromStream(SQLTable table)
		{
			DbCommunicationProvider db = new DbCommunicationProvider();
			var fileStream = db.GetFileStream(table);

			if (fileStream == null)
			{
				Console.WriteLine("No data found to download.");
				return;
			}

			var fileName = table.ToString() + ".csv";

			using var streamRef = new DotNetStreamReference(stream: fileStream);
			await JS.InvokeVoidAsync("downloadFileFromStream", fileName, streamRef);
		}
	}
	public class DownloadForm
	{

	}
}
