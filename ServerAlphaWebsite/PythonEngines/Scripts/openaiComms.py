from openai import OpenAI
import asyncio
import sys
import json
import os

scriptDirectory = sys.argv[1]
constructorPromptDirectory = scriptDirectory + "/prompt.txt"
answerCategorizerPromptDirectory = scriptDirectory + "/answerprompt.txt"

print(f"scriptDirectory = {scriptDirectory}\nconstructorPromptDirectory = {constructorPromptDirectory}\nanswerCategorizationPromptDirectory = {answerCategorizerPromptDirectory}", flush=True)

def readEncodedPrompt(filePath):
    encodings = ["utf-8", "latin-1", "utf-16", "cp1252"]
    
    for enc in encodings:
        try:
            with open(filePath, "r", encoding=enc) as f:
                return f.read()
        except (UnicodeDecodeError, UnicodeError):
            print(f"Failed with encoding: {enc}")
    
    raise ValueError("Could not decode file with known encodings.")

#with open(constructorPromptDirectory, 'r', encoding='utf-8') as f: #C:/Users/Chervinskiy/Desktop/AlphaWebsite/AlphaWebsite/AlphaWebsite/Scripts/prompt.txt

GPT_CONSTRUCTOR_PROMPT = readEncodedPrompt(constructorPromptDirectory)
GPT_ANSWER_CATEGORIZER_PROMPT = readEncodedPrompt(answerCategorizerPromptDirectory)
GPT_SUMMARY_PROMPT = """You are an energy system planner. Your task is to review the preceding conversation history and provide a basic energy system plan.
**The plan must be based *strictly* and *only* on the components, questions, or concepts that the *user* explicitly mentioned or introduced in their messages.**
**Absolutely do NOT include any components, features, considerations (like maintenance, environmental factors, potential issues not mentioned by the user, etc.), or related concepts that were only mentioned by the *assistant* in its responses.**
Think of this as building a plan using *only* the building blocks provided *by the user*, ignoring any suggestions or additional information the assistant might have offered.
**Crucially, start your response directly with the proposed plan or description of the system.**
**Do not include any conversational filler, introductory phrases like 'Yes,' 'Okay,' 'Based on that,' or acknowledgments of the prompt.**
Just provide the plan/description using *only* the elements the user explicitly introduced.
Try to limit your responses up to 40 words, you can write less if you need to. Do not use numbered entries or entries using points.
At ends of questions, a culture is specified: "en-US", "he-IL" etc. If no culture was specified, you can default to English, however if there was then the content of the response should be written in the language of the culture specified.
"""
GPT_IMAGE_PROMPT = "You are here to create prompts to generate images. Respond with a single prompt, ready to be copied and pasted into DALL-E. Try to relate the prompts to mars, as you are going to be creating images about energy systems on mars. Relate your answers to the conversation that you've had with the user."

client = OpenAI(api_key=os.environ.get("MARTIAN_OPENAI_KEY", "unknown"))

class OpenAIClient:
    
    def __init__(self):
        self.conversation_history = []
        self.conversation_history.append({"role": "developer", "content": GPT_CONSTRUCTOR_PROMPT})

    def change_system_prompt(self, new_prompt_content):
        if not self.conversation_history:
            self.conversation_history.append({"role": "developer", "content": new_prompt_content})
        else:
            self.conversation_history[0]["content"] = new_prompt_content

        #print(f"Requested prompt:\n{new_prompt_content}\n\nself.conversation_history[0][\"content\"]:\n{self.conversation_history[0]["content"]}\n\n\n\n\n\n\n\n\n")
    
    async def fetch_gpt_response(self, prompt):
        if not isinstance(prompt, str):
            raise TypeError("Prompt is not string!")

        completion = client.chat.completions.create(
            model="gpt-4o",
            messages=self.conversation_history,
            temperature=0,
            top_p=1,
            frequency_penalty=0,
            presence_penalty=0,
            seed=42
        )

        content = completion.choices[0].message.content

        return content
    
    async def fetch_gpt_json_response(self, prompt):
        if not isinstance(prompt, str):
            raise TypeError("Prompt is not string!")
        
        completion = client.chat.completions.create(
            model="gpt-4o",
            messages=self.conversation_history,
            seed=42,
            response_format={"type": "json_object"}
        )

        content = completion.choices[0].message.content
        return content

    async def fetch_gpt_image(self, query):
        response = client.images.generate(
            prompt=query,
            model="dall-e-3",
            size="1024x1024",
            n=1,
        )

        return response.data[0].url

    def log_to_history(self, msg):
        if isinstance(msg, dict):
            self.conversation_history.append(msg)

    async def GetResponse(self, prompt):
        self.change_system_prompt(GPT_CONSTRUCTOR_PROMPT)
        self.log_to_history(({"role": "user", "content": prompt}))
        
        response = await self.fetch_gpt_response(prompt)
        self.log_to_history({"role": "assistant", "content": response})

        return response
    
    async def GetJsonResponse(self, prompt):
        self.change_system_prompt(GPT_CONSTRUCTOR_PROMPT)
        self.log_to_history(({"role": "user", "content": prompt}))
        
        response = await self.fetch_gpt_json_response(prompt)

        try:
            json_response = json.loads(response)
            self.log_to_history({"role": "assistant", "content": json_response["response"]})
        except json.JSONDecodeError as e:
            print(f"Error parsing JSON response: {e}")
            print(f"Received content: {response}")

        return response

    async def GetImage(self, prompt=None):
        self.change_system_prompt(GPT_IMAGE_PROMPT)

        if not isinstance(prompt, str) or prompt is None or prompt == "":
            prompt = await self.fetch_gpt_response("Generate a prompt for an image that would be passed to DALL-E. The prompt should be engineered in a way that it summarizes our conversation in the image.")

        response = await self.fetch_gpt_image(prompt)
        return response
    
    async def GetAnswerCategorization(self):
        self.change_system_prompt(GPT_ANSWER_CATEGORIZER_PROMPT)
        response = await self.fetch_gpt_json_response("Categorize the conversation.")
        return response

    async def GetSummary(self):
        self.change_system_prompt(GPT_SUMMARY_PROMPT)
        self.log_to_history(({"role": "user", "content": "Create a solution."}))
        response = await self.fetch_gpt_response("Create a solution.")
        return response

    async def ForgetLast(self):
        self.conversation_history.pop()

    def get_response(self, prompt):
        return asyncio.run(self.GetResponse(prompt))
    
    def get_json_response(self, prompt):
        return asyncio.run(self.GetJsonResponse(prompt))

    def get_image(self, prompt=None):
        return asyncio.run(self.GetImage(prompt))

    def get_summary(self):
        return asyncio.run(self.GetSummary())
    
    def get_answer_categorization(self):
        return asyncio.run(self.GetAnswerCategorization())

    def forget_last(self):
        if len(self.conversation_history) > 1:
            asyncio.run(self.ForgetLast())
        else:
            raise IndexError("No conversation history to forget.")

    def clear_conversation(self):
        self.conversation_history = [GPT_CONSTRUCTOR_PROMPT]
