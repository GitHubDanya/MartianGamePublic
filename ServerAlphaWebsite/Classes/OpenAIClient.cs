using Python.Runtime;
using ServerAlphaWebsite.Models.DTOs;
using ServerAlphaWebsite.PythonEngines;
using System.Text.Json;

namespace ServerAlphaWebsite.Classes
{
	public class OpenAIClient
	{
		/*
		 * OpenAIClient
		 * 
		 * Resembles the python OpenAIClient class
		 * 
		 * --- Values ---
		 * 
		 * Score: !!deprecated!! Score for the user
		 * openai_module: Python module
		 * openai_client: Actual client class
		 * 
		 * --- Constructor ---
		 * 
		 * Imports the python module and creates an instance of the OpenAIClient class.
		 * 
		 * --- Methods ---
		 * 
		 * get_response:
		 * Returns a chatbot response to the input string.
		 * 
		 * get_json_response:
		 * Returns a chatbot response to the input string in JSON format.
		 * 
		 * get_answer_categorization:
		 * Categorizes the answer and returns the result as an AnswerCategorizationDto object.
		 * 
		 * get_image:
		 * Creates the summary image for the conversation.
		 * Also supports custom prompts (input argument)
		 * 
		 * clear_conversation:
		 * Clears the conversation history.
		 * 
		 * get_summary:
		 * Generates and returns an answer (appropriate to the user's conversation)
		 * 
		 * forget_last:
		 * Removes the last message from the conversation history.
		 * 
		 * !!deprecated!! AddScore:
		 * !!deprecated!! Appends the client score by value of the score argument.
		 */

		public float Score = 0;
		dynamic openai_module;
		dynamic openai_client;
		public OpenAIClient()
		{

			ClientHost.InitializeHost();
			try
			{
				using (Py.GIL())
				{
					dynamic sys = Py.Import("sys");
					//if (Directory.Exists(@"C:\Users\Chervinskiy\Desktop\AlphaWebsite\AlphaWebsite\AlphaWebsite\Scripts"))
					//	sys.path.append(@"C:\Users\Chervinskiy\Desktop\AlphaWebsite\AlphaWebsite\AlphaWebsite\Scripts");
					//else if (Directory.Exists(@".\ExternalFiles\"))
					//	sys.path.append(@".\ExternalFiles\");

					string pathToScripts = Path.Combine(AppContext.BaseDirectory, "PythonEngines", "Scripts");

					if (Directory.Exists(pathToScripts))
						sys.path.append(pathToScripts);
					else
					{
						Console.WriteLine("(OpenAIClient.cs) No suitable directory found!");
						return;
					}

					sys.argv = new PyList(new PyObject[] {new PyString("script.py"), new PyString(pathToScripts)});

					openai_module = Py.Import("openaiComms");
					openai_client = openai_module.OpenAIClient();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Python error: {ex.Message}");
				throw;
			}
		}

		public string get_response(string input)
		{
			return openai_client.get_response(input);
		}

		public ResponseDto get_json_response(string input)
		{
			string json = openai_client.get_json_response(input);
			return JsonSerializer.Deserialize<ResponseDto>(json) ?? new ResponseDto();
		}
		public AnswerCategorizationDto get_answer_categorization()
		{
			string json = openai_client.get_answer_categorization();
			return JsonSerializer.Deserialize<AnswerCategorizationDto>(json) ?? new AnswerCategorizationDto();
		}
		public string get_image(string input)
		{
			string response = openai_client.get_image(input);
			return response;
		}

		public void clear_conversation()
		{
			openai_client.clear_conversation();
		}

		public string get_summary()
		{
			return openai_client.get_summary();
		}

		public void forget_last()
		{
			openai_client.forget_last();
		}
		public void AddScore(float score)
		{
			Score += score;
		}
	}
}
