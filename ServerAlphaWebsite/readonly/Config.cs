namespace ServerAlphaWebsite
{
    public class Config
    {
        public static string ChatbotName => "Mark";
		public static string[] ChatbotThinkingSequence => new string[] { ".", "..", "..." };
		public static int MinGptParsedResponseLength => 3;

		#region ANIMATION_NAMES

		public static string AnimationSlideInUp => "animate slideInUp";
		public static string AnimationSlideOutUp => "animate slideOutUp";

		#endregion

		#region REGEX

		public static string RegexMarkupPattern => @"\*\*(.*?)\*\*";
		public static string RegexMarkupReplacement => @"<b>$1</b>";

		#endregion

		#region	FILE_PATHS

		public static string FilePathLoadingIcon = "images/loading_icon.gif";

		#endregion
	}
}
