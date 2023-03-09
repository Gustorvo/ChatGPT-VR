using OpenAI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class ChatGPT : MonoBehaviour
{
    [SerializeField] TextAsset apiKey;

    private OpenAIApi openai;
    private string Instruction = "The following is a conversation with an AI assistant of female gender. The assistant is helpful, creative, clever and does whatever she has been asked. \nHuman: ";

    private const string dialogModel = "text-davinci-003"; // Most capable GPT-3 model. Can do any task the other models can do, often with higher quality, longer output and better instruction-following. Also supports inserting completions within text.
    private const string clasificationModel = "text-babbage-001";
    /// Used for serializing and deserializing PascalCase request object fields into snake_case format for JSON. Ignores null fields when creating JSON strings.
    private readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new CustomNamingStrategy()
        }
    };

    private void Awake()
    {
        var auth = JsonConvert.DeserializeObject<Auth>(apiKey.text, jsonSerializerSettings);
        openai = new OpenAIApi(auth.ApiKey, auth.Organization);
    }
    public async Task<string> SendRequestAsync(string request, CancellationToken token)
    {
        // print("Sending request to chat");
        Instruction += $"{request}\nAI: ";
        string result = await SendReply(Instruction, dialogModel);
        // check for cancellation request
        token.ThrowIfCancellationRequested();
        print($"Chat said: {result}");
        Instruction += $"{result}\nHuman: ";
        return result;
    }

    public async Task<string> DoSentimentalAnalysis(string text, CancellationToken token)
    {
        string parsedText = Regex.Replace(text, @"SENT_ANALYSIS:\s*\d+", string.Empty);
        //print("Trying to do sentimental analysis of text:" + parsedText);
        string instructions = "Decide whether a text's sentiment is positive, neutral, or negative (in the given context), returning a label (either 'positive', 'negative' or 'neutral') followed by a score between +1 and -1.";
        instructions += parsedText;
        string result = await SendReply(instructions, dialogModel);
        // check for cancellation request
        token.ThrowIfCancellationRequested();
        print($"Sentimental analysis: {result}");
        return result;
    }

    private async Task<string> SendReply(string instruction, string model)
    {
        // Complete the instruction
        var completionResponse = await openai.CreateCompletion(new CreateCompletionRequest()
        {
            Prompt = instruction,
            Model = model,
            MaxTokens = 256
        });

        return completionResponse.Choices[0].Text;
    }
}
