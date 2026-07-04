using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.PythonEngines;

namespace ServerAlphaWebsite.GameStages.AnsweringStage
{
    public static class ImageService
    {
        public static string GenerateImage(string user)
        {
            OpenAIClient? client = ClientHost.GetClientByUsername(user);

            Console.WriteLine("For image generation:\n" + ClientHost.GetActiveClientsReport());

            if (client == null) throw new Exception($"(ImageService.cs) Couldn't find user called {user}");
            string imageUrl = client.get_image(string.Empty);
            Console.WriteLine("Image generation function finished exectuion and returned " + imageUrl);
            return imageUrl;
        }
    }
}
