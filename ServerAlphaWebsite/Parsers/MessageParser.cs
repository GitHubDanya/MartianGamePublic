using ServerAlphaWebsite.Classes;

namespace ServerAlphaWebsite.Parsers
{
	public static class MessageParser
	{
        /* !!!!!!!!!!!!
		 *  DEPRECATED
		 * !!!!!!!!!!!!
		 

        public static string[] Parse(Message message)
		{
			if (message == null || string.IsNullOrEmpty(message.Content))
				return new string[0];

			string[] parsedMessage = message.Content.Split("-<->-");

			if (parsedMessage[0].Contains("This question is irrelevant or was already asked. Please ask another question.") && parsedMessage.Length > 2)
				parsedMessage[2] = "0";

			parsedMessage[0] = parsedMessage[0].Trim('"');

			return parsedMessage;
		}

		*/
    }
}
