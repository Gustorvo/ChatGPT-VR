using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Assertions;

public class Main : MonoBehaviour
{
    CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    [SerializeField] STT stt;
    [SerializeField] TTS tts;
    [SerializeField] ChatGPT chat;
    [SerializeField] AudioClip welcomePhraseAudio;
    [SerializeField] AudioClip errorAudio;

    private CancellationToken token;
    private SystemStatus currentStatus;
    public SystemStatus CurrentStatus
    {
        get => currentStatus;
        private set
        {
            currentStatus = value;
            OnCurrentStatusChanges?.Invoke(currentStatus);
        }
       
    }
    public string UserRecognizedText { get; private set; }
    public string OpenAIAnswerText { get; private set; }

    public static event Action<SystemStatus> OnCurrentStatusChanges;

    private void Start()
    {
        CurrentStatus = SystemStatus.Inactive;
        Assert.IsNotNull(welcomePhraseAudio);
        Assert.IsNotNull(errorAudio);
        token = cancellationTokenSource.Token;
        Task _ = StartDialogWithWelcomePhrase(token);
    }

    private async Task StartDialogWithWelcomePhrase(CancellationToken token)
    {
        OpenAIAnswerText = "Hello, how can I help you?";
        CurrentStatus = SystemStatus.Speaking;
        await tts.SpeakAudioAsync(welcomePhraseAudio, token);
        Task _ = StartDialogLoopAsync(token);
    }

    private async Task StartDialogLoopAsync(CancellationToken token)
    {
        print("Starting dialog");

        // repeat the loop 
        while (!token.IsCancellationRequested)
        {
            try
            {                
                await TakeVoiceInputAndAnswerAsync(token);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException)
                {
                    Debug.LogWarning("Canceled");
                    break;
                }
                else
                {
                    Debug.LogError(ex);
                    CurrentStatus = SystemStatus.Speaking;
                    await tts.SpeakAudioAsync(errorAudio, token);
                    CurrentStatus = SystemStatus.Inactive;
                    throw ex;
                }
            }

            await Task.Yield();
        }
    }

    private async Task TakeVoiceInputAndAnswerAsync(CancellationToken token)
    {
        // take voice input
        CurrentStatus = SystemStatus.Listerning;
        var recognizedText = await stt.StartRecognitionAsync(token);
        UserRecognizedText = recognizedText;

        // send recognized voice input to ChatGPT
        CurrentStatus = SystemStatus.Thinking;
        var chatResponse = await chat.SendRequestAsync(recognizedText, token);
        OpenAIAnswerText = chatResponse;       

        // Speak generated response
        CurrentStatus = SystemStatus.Speaking;
        await tts.SpeakTextAsync(chatResponse, token);
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}