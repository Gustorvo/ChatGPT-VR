using OpenAI;
using System.IO;
using UnityEngine;
using OpenAI.Chat;
using OpenAI.Models;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using OpenAI.Completions;

public class ChatGPT : MonoBehaviour
{
    private OpenAIClient openai;
    private string Instruction = "The following is a conversation with an AI assistant of female gender. The assistant is helpful, creative, clever and does whatever she has been asked.";
    private List<Message> chatPrompts = new List<Message>();
  
    private void Awake()
    {
        var pathToKey = Path.Combine(Application.dataPath, "Keys/");
        openai = new OpenAIClient(OpenAIAuthentication.LoadFromDirectory(pathToKey));
        chatPrompts.Add(new Message(OpenAI.Chat.Role.System, Instruction));
        chatPrompts.Add(new Message(OpenAI.Chat.Role.Assistant, "Hello, how can I help you?"));
    }
    public async Task<string> SendRequestAsync(string request, CancellationToken token)
    {
        chatPrompts.Add(new Message(OpenAI.Chat.Role.User, request));
        string result = await SendReply();
        chatPrompts.Add(new Message(OpenAI.Chat.Role.Assistant, result));
        token.ThrowIfCancellationRequested();
        print($"Chat said: {result}");
        return result;
       
    }

    public async Task<string> DoSentimentalAnalysis(string text, CancellationToken token)
    {
        string instructions = "Decide whether a given text's sentiment is positive, neutral, or negative, returning a label (either 'positive', 'negative' or 'neutral') followed by a score between 1 and -1, Where most positive sentiment is 1 and most negative = -1. Text: ";
        instructions += text;
        print($"sending text for sentiment analysis, text: {instructions}");
        CompletionRequest complitionRequest = new CompletionRequest(model: Model.Ada);
        complitionRequest.Prompt = instructions;    
        var result = await openai.CompletionsEndpoint.CreateCompletionAsync(complitionRequest, token);
        token.ThrowIfCancellationRequested();
        print($"Sentimental analysis: {result}");
        return result.ToString();
    }

    private async Task<string> SendReply()
    {
        var chatRequest = new ChatRequest(chatPrompts, model: Model.GPT3_5_Turbo);
        var result = await openai.ChatEndpoint.GetCompletionAsync(chatRequest);
        return result.FirstChoice;
    }

    private void OnDestroy()
    {
        chatPrompts.Clear();
    }
}
