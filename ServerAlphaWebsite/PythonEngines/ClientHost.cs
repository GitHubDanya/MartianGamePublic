using Python.Runtime;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.Models.DTOs;

namespace ServerAlphaWebsite.PythonEngines
{
    public static class ClientHost
    {
        private static Dictionary<string, OpenAIClient> clients = new Dictionary<string, OpenAIClient>();
        private static bool engineInitiated = false;

        public static void InitializeHost()
        {
            if (!engineInitiated)
            {
                engineInitiated = true;
                PythonNet.InitPythonEngine();
                var m_threadState = PythonEngine.BeginAllowThreads();
            }
        }

        public static OpenAIClient GetClientByUsername(string username)
        {
            if (clients.ContainsKey(username))
                return clients[username];
            return null;
        }

        public static ResponseDto GetQuestionAnswer(string user, Message message)
        {
            if (message == null || string.IsNullOrWhiteSpace(message.Content) || string.IsNullOrWhiteSpace(user))
                return new ResponseDto();

            if (!clients.ContainsKey(user))
                clients.Add(user, new OpenAIClient());

            using (Py.GIL())
                return clients[user].get_json_response(message.Content);
        }

        public static AnswerCategorizationDto GetAnswerCategorizationDto(string user)
        {
            if (!clients.ContainsKey(user))
                clients.Add(user, new OpenAIClient());

            using (Py.GIL())
                return clients[user].get_answer_categorization();
        }

        public static string GetConversationSummary(string user)
        {
            if (!clients.ContainsKey(user))
                clients.Add(user, new OpenAIClient());
            using (Py.GIL())
                return clients[user].get_summary();
        }

        public static void ForgetLastMessage(string user)
        {
            if (string.IsNullOrWhiteSpace(user))
                return;

            if (!clients.ContainsKey(user))
                return;

            using (Py.GIL())
                clients[user].forget_last();
        }

        public static string GenerateImageURL(string user, string? customPrompt)
        {
            if (string.IsNullOrWhiteSpace(user))
                return string.Empty;

            if (!clients.ContainsKey(user))
                clients.Add(user, new OpenAIClient());

            using (Py.GIL())
                return clients[user].get_image((string.IsNullOrEmpty(customPrompt) ? string.Empty : customPrompt));
        }

        private static void clearUserConversation(string user)
        {
            if (!clients.ContainsKey((string)user))
            {
                Console.WriteLine("(ClientHost.cs) Cannot clear conversation history for user " + user + " - user client not found!");

                return;
            }

            clients[user].clear_conversation();
        }

        private static void clearGlobalConversations()
        {
            foreach (OpenAIClient client in clients.Values)
                client.clear_conversation();
        }

        public static async Task ClearConversation(string? user)
        {
            if (!string.IsNullOrWhiteSpace(user))
                clearUserConversation(user);
            else
                clearGlobalConversations();
        }

        public static string GenerateUserId()
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string randomPart = Guid.NewGuid().ToString("N").Substring(0, 6);
            return $"user_{timestamp}_{randomPart}";
        }
    }
}
