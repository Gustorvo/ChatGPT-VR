using System;
using UnityEngine;
using System.Threading;
using UnityEngine.Assertions;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;

public class TTS : MonoBehaviour
{
    [SerializeField] TextAsset subscriptionKey;
    [SerializeField] string region = "northeurope";
    [SerializeField] AudioSource audioSource;

    private SpeechConfig config;
    private SpeechSynthesizer synthesizer;
    private Connection connection;   
    private const int numChannels = 1; // mono
    private const int bytesPerSample = 2; // 16-bit audio
    private const int sampleRate = 24000; // 24kHz audio    

    private void Awake()
    {
        Assert.IsNotNull(subscriptionKey);
    }

    private void Start()
    {
        config = SpeechConfig.FromSubscription(subscriptionKey.ToString(), region);
        // The default format is RIFF, which has a riff header.
        // We are playing the audio in memory as audio clip, which doesn't require riff header.
        // So we need to set the format to raw (24KHz for better quality).
        config.SetSpeechSynthesisOutputFormat(SpeechSynthesisOutputFormat.Raw24Khz16BitMonoPcm);

        // Creates a speech synthesizer.
        // passing null will not start the playback, as we only want to extract audio data from the synthesizer (without starting the playback)
        synthesizer = new SpeechSynthesizer(config, null);

        synthesizer.SynthesisCanceled += (s, e) =>
        {
            var cancellation = SpeechSynthesisCancellationDetails.FromResult(e.Result);
            print("canceled due to: " + e.Result.Reason);
            print($"CANCELED:\nReason=[{cancellation.Reason}]\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you update the subscription info?");
        };

        synthesizer.SynthesisStarted += (s, e) =>
        {
            //print("STT started");
        };

        synthesizer.SynthesisCompleted += (s, e) =>
        {
            //print("STT completed");
        };

        synthesizer.VisemeReceived += (s, e) =>
        {
            // print($"Viseme recieved: animation {e.Animation}, audio offset {e.AudioOffset}, result id {e.ResultId}, viseme id {e.VisemeId}");
        };

        using (connection = Connection.FromSpeechSynthesizer(synthesizer))
        {
            connection.Open(true);
        }
    }

    public async Task<AudioClip> GenerateAudioFromTextWithTimeout(string text, CancellationToken token, TimeSpan timeout)
    {
        // Start a Task that will complete after the specified timeout
        var timeoutTask = Task.Delay(timeout, token);

        // Start a Task that generates the audio clip
        var generateAudioTask = GenerateAudioFromText(text, token);

        // Wait for either task to complete
        var completedTask = await Task.WhenAny(generateAudioTask, timeoutTask);

        // If the generateAudioTask completed first, return its result
        if (completedTask == generateAudioTask)
        {
            print("returned generated audio");
            return await generateAudioTask;
        }
        // If the timeoutTask completed first, throw an exception
        else
        {
            throw new TimeoutException($"Audio generation timed out after {timeout.TotalSeconds} seconds.");
        }
    }

    public async Task SpeakTextAsync(string text, CancellationToken token)
    {        
        // azure TTS can't handle long text for some reason, so we need to split it into smaller chunks
        var chunks = text.ToChunks(400);
        //print("number of chunks " + chunks.Count);
        foreach (var textChunk in chunks)
        {
            //print($"speaking chunk: {textChunk}");
            var audio = await GenerateAudioFromText(textChunk, token);
            await SpeakAudioAsync(audio, token);
        }
    }

    public async Task SpeakAudioAsync(AudioClip audioClip, CancellationToken token)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        //print($"Start speaking.");
        while (audioSource.isPlaying)
        {
            token.ThrowIfCancellationRequested();
            await Task.Delay(50, token); // Use provided token with Task.Delay to cancel playback 
        }
        audioSource.Stop();
        //print("Stop speaking");       
    }

    public async Task<AudioClip> GenerateAudioFromText(string text, CancellationToken token)
    {
        AudioClip audioClip = null;
        //print("Start generating audio");
        using (var result = await synthesizer.StartSpeakingTextAsync(text))
        {
            token.ThrowIfCancellationRequested(); // check for cancellation request
            using (var audioDataStream = AudioDataStream.FromResult(result))
            {
                token.Register(() =>
                {
                    //stop the speech synthesis process immediately when the token is canceled
                    synthesizer.StopSpeakingAsync().Wait();
                });

                byte[] buffer = new byte[sampleRate * 60]; // 1 min audio buffer
                uint totalSize = 0;
                uint filledSize = 0;
                while ((filledSize = audioDataStream.ReadData(buffer)) > 0)
                {
                    totalSize += filledSize;
                    token.ThrowIfCancellationRequested(); // check for cancellation request
                }
               // print("total size " + totalSize);
                audioClip = ToAudioClip(buffer, totalSize);
                print("clip ready " + audioClip.length);
                return audioClip;
                //print($"Generated audio clip with of {audioClip.length} seconds");
            }
        }
    }

    private AudioClip ToAudioClip(byte[] audioData, uint totalBytes)
    {
        int numSamples = (int)(totalBytes / (numChannels * bytesPerSample));
        float[] floatArray = new float[numSamples];

        // convert to float array
        for (int i = 0; i < numSamples; i++)
        {
            int byteIndex = i * numChannels * bytesPerSample;
            floatArray[i] = BitConverter.ToInt16(audioData, byteIndex) / 32768.0f;
        }

        AudioClip audioClip = AudioClip.Create("GeneratedAudioClip", numSamples, numChannels, sampleRate, false);
        audioClip.SetData(floatArray, 0);
        return audioClip;
    }
}