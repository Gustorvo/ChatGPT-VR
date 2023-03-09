using OpenAI;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OpenAI.Models;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using OpenAI.Chat;

public class ChatGPT : MonoBehaviour
{
    [SerializeField] TextAsset apiKey;

    private OpenAIClient openai;
    private string Instruction = "The following is a conversation with an AI assistant of female gender. The assistant is helpful, creative, clever and does whatever she has been asked.";
    private List<ChatPrompt> chatPrompts = new List<ChatPrompt>();
    private void Awake()
    {
        var pathToKey = Path.Combine(Application.dataPath, "Keys/");
        openai = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory(pathToKey));
        chatPrompts.Add(new ChatPrompt("Instruction", Instruction));
    }
    public async Task<string> SendRequestAsync(string request, CancellationToken token)
    {
        chatPrompts.Add(new ChatPrompt("Human", request));

        // print("Sending request to chat");
        //Instruction += $"{request}\nAI: ";
        string result = await SendReply();
        chatPrompts.Add(new ChatPrompt("AI", result));
        // check for cancellation request
        token.ThrowIfCancellationRequested();
        print($"Chat said: {result}");
        //Instruction += $"{result}\nHuman: ";
        return result;
    }

    public async Task<string> DoSentimentalAnalysis(string text, CancellationToken token)
    {
        string parsedText = Regex.Replace(text, @"SENT_ANALYSIS:\s*\d+", string.Empty);
        //print("Trying to do sentimental analysis of text:" + parsedText);
        string instructions = "Decide whether a text's sentiment is positive, neutral, or negative (in the given context), returning a label (either 'positive', 'negative' or 'neutral') followed by a score between +1 and -1.";
        instructions += parsedText;
        string result = await SendReply();
        // check for cancellation request
        token.ThrowIfCancellationRequested();
        print($"Sentimental analysis: {result}");
        return result;
    }

    private async Task<string> SendReply()
    {
        var chatRequest = new ChatRequest(chatPrompts, model: Model.GPT3_5_Turbo);
        var result = await openai.ChatEndpoint.GetCompletionAsync(chatRequest);
        Debug.Log(result.FirstChoice);

        return result.FirstChoice;
    }
}
