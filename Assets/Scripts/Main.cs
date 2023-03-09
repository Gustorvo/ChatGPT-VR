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

    private void Start()
    {
        Assert.IsNotNull(welcomePhraseAudio);
        Assert.IsNotNull(errorAudio);
        token = cancellationTokenSource.Token;
        Task _ = StartDialogWithWelcomePhrase(token);
    }

    private async Task StartDialogWithWelcomePhrase(CancellationToken token)
    {
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
                    print("Canceled");
                    break;
                }
                else
                {
                    Debug.LogError(ex);
                    Task _ = tts.SpeakAudioAsync(errorAudio, token);
                    throw ex;
                }
            }

            await Task.Yield();
        }
    }

    private async Task TakeVoiceInputAndAnswerAsync(CancellationToken token)
    {
        // take voice input
        var recognizedText = await stt.StartRecognitionAsync(token);

        // send recognized voice input to ChatGPT
        var chatResponse = await chat.SendRequestAsync(recognizedText, token);

        // try get sentimental analysis
       // var analisis = chat.DoSentimentalAnalysis(chatResponse, token);

        // Speak generated response
        await tts.SpeakTextAsync(chatResponse, token);
    }

    private void OnDisable()
    {
        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();
    }
}