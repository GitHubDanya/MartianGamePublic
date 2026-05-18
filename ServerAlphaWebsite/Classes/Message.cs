using Microsoft.Extensions.Localization;
using ServerAlphaWebsite.Locales;
using ServerAlphaWebsite.Parsers;

namespace ServerAlphaWebsite.Classes
{
	public class Message
	{

		/* Message class
		 * 
		 * Used for defining chat messages.
		 * 
		 * --- Values ---
		 *	
		 *	Content: Content of the message,
		 *	Sender: Enum for message sender
		 *	Time: Time when the message was sent
		 *	
		 * --- Methods ---
		 *	
		 *	AsString():
		 *  Purposed for converting the message to a string.
		 *	
		 *	Returns the string that is sent to the chat window. Includes markup.
		 *	If withTime is true, includes the time of the message in the string aswell.
		 */

		public string? Content { get; set; } = string.Empty;
		public MessageSender Sender { get; set; }
		public DateTime Time { get; set; }

		public Message Clone()
		{
			return (Message)this.MemberwiseClone();
		}

		public string AsString(IStringLocalizer<Resource> localizer, bool withTime = false)
		{
			string? messageString;

			messageString = $"<b>{(Sender == MessageSender.GPT? localizer["GameUsernameMark"] : localizer["GameUsernameUser"])}:</b> {Content}";

			if (messageString == null)
				return string.Empty;

			if (withTime)
				messageString = $"{Time.Hour}:{Time.Minute} - {messageString}";

			return messageString;
		}
	}
}
