using System.ComponentModel;

using Microsoft.Extensions.AI;

using OpenAI.Chat;

using ChatCompletion = Microsoft.Extensions.AI.ChatCompletion;

List<UserData> users = [
   new UserData("Lasse Vågsæther Karlsen", 53),
   new UserData("Bill Gates", 65),
   new UserData("John Forsyth", 32),
];

IChatClient chatClient = new ChatClient("gpt-4o", Environment.GetEnvironmentVariable("OPENAI_API_KEY")).AsChatClient()
  .AsBuilder()
  .UseFunctionInvocation()
  .Build();

var options = new ChatOptions
{
   Tools =
   [
      AIFunctionFactory.Create(GetCurrentDate),
      AIFunctionFactory.Create(GetCurrentTime),
      AIFunctionFactory.Create(() => "Lasse Vågsæther Karlsen", "GetUserName", "Gets the name of the user interacting with the AI"),

      AIFunctionFactory.Create(GetUserList),
      AIFunctionFactory.Create(GetUserAge),
   ]
};

//ChatCompletion response = await chatClient.CompleteAsync("What is the current date and time, and who am I?", options);
ChatCompletion response = await chatClient.CompleteAsync("How old is John?", options);
Console.WriteLine(response.Message);

[Description("Gets the current date, in the format yyyy-MM-dd.")]
static string GetCurrentDate() => DateTime.Now.ToString("yyyy-MM-dd");

[Description("Gets the current time, in the format HH:mm:ss.")]
static string GetCurrentTime() => DateTime.Now.ToString("HH:mm:ss");

[Description("Gets a list of people, containing their full names")]
List<string> GetUserList()
{
   Console.WriteLine("Getting user list");
   return users.Select(u => u.Name).ToList();
}

[Description("Gets the age of a specific person. The full name have to be provided. Will return -1 if the person is not found.")]
int GetUserAge(string name)
{
   Console.WriteLine($"Getting age of {name}");
   return users.FirstOrDefault(u => u.Name == name)?.Age ?? -1;
}

public record UserData(string Name, int Age);