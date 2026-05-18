using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.PythonEngines;

namespace ServerAlphaWebsite.GameStages.AnsweringStage
{
	public static class ImageService
	{
		public static string GenerateImage(string user)
		{
			OpenAIClient? client = ClientHost.GetClientByUsername(user);

			if (client == null) throw new Exception($"(ImageService.cs) Couldn't find user called {user}");
			string imageUrl = client.get_image(string.Empty);
			return imageUrl;
		}
	}
}
