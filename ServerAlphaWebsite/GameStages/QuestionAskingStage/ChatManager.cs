using ServerAlphaWebsite.Classes;
using Microsoft.Extensions.Localization;
using ServerAlphaWebsite.Locales;

namespace ServerAlphaWebsite.GameStages.QuestionAskingStage
{
    public class ChatManager
    {
        public List<Message> messages = new List<Message>();

        private User currentUser;
        private IStringLocalizer<Resource> localizer;

        public ChatManager(User user, IStringLocalizer<Resource> localizer)
        {
            currentUser = user;
            this.localizer = localizer;
            messages = new List<Message>(user.ChatHistory);
        }

        public void SendMessage(Message message)
        {
            if (message == null || string.IsNullOrEmpty(message.Content)) return;

            messages.Add(message);
            currentUser.LogMessage(message);
        }

        public void DeleteMessage(Message message) => messages.Remove(message);
        public void DeleteLastMessage() => DeleteMessage(messages.Last());
        public void ClearChatMessages() => messages.Clear();

        public string GetChatString(bool withTime = false)
        {
            if (messages.Count <= 0) return string.Empty;

            string res = string.Empty;

            foreach (var message in messages)
            {
                res = $"{res}{message.AsString(localizer, withTime).TrimEnd('\n')}\n\n";
            }

            return res.TrimEnd('\n');
        }
    }
}
