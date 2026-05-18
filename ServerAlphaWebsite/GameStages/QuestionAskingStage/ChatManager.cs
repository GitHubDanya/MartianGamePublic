using ServerAlphaWebsite;
using ServerAlphaWebsite.Pages;
using ServerAlphaWebsite.Classes;
using ServerAlphaWebsite.PythonEngines;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.Parsers;
using Microsoft.AspNetCore.Components;
using ServerAlphaWebsite.ServerStorage;
using Microsoft.Extensions.Localization;
using ServerAlphaWebsite.Locales;

namespace ServerAlphaWebsite.GameStages.QuestionAskingStage
{
    public class ChatManager
    {
        public List<Message> messages = new List<Message>();
		
		private Game game;
		private UserInfoStorage UserInfoStorage;

		public ChatManager(Game game)
        {
            this.game = game;
			UserInfoStorage = this.game.UserInfoStorage;
			messages = new List<Message>(UserInfoStorage.GetMessageHistory(game.Username));
		}
        
        public void SendMessage(Message message)
        {
			if (message == null || string.IsNullOrEmpty(message.Content))
				return;

            messages.Add(message);
			UserInfoStorage.LogMessage(message, game.Username);
		}

		public void DeleteMessage(Message message)
		{
			messages.Remove(message);
		}

		public void DeleteLastMessage()
		{
			DeleteMessage(messages.Last());
		}

		//public ResponseDto GetResponse(Message message)
		//{
		//	ResponseDto response = ClientHost.GetQuestionAnswer(game.Username, message);

		//	Message responseMessage = new Message()
		//	{
		//		Sender = MessageSender.GPT,
		//		Content = response.response,
		//		Time = DateTime.Now,
		//	};

		//	if (response == null)
		//	{
		//		responseMessage.Content = "There has been an error generating a response.";
		//		return null;
		//	}

		//	return responseMessage;
		//}

		public string GetChatString(IStringLocalizer<Resource> localizer, bool withTime = false)
        {
            if (messages.Count <= 0)
                return game.MessageBoxContent;

            string res = string.Empty;

			foreach (var message in messages)
			{
				res = $"{res}{message.AsString(localizer, withTime).TrimEnd('\n')}\n\n";
			}

            return res.TrimEnd('\n');
		}

        public void ClearChatMessages()
        {
            messages.Clear();
        }
    }
}
