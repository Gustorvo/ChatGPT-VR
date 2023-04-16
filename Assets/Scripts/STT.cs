using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class STT : MonoBehaviour
{

#if PLATFORM_ANDROID
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif
    private SpeechConfig config;


    private void Awake()
    {      
        config = AzureAuthentication.InitAzureSpeech();
    }
    void Start()
    {
#if PLATFORM_ANDROID
        // Request to use the microphone, cf.
        // https://docs.unity3d.com/Manual/android-RequestingPermissions.html

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
            Debug.LogError("App needs microphone permissions to function!");
        }
#endif
    }
  

    public async Task<string> StartRecognitionAsync(CancellationToken token)
    {
        string result = "";
        // print("starting recognition");

        using (var recognizer = new SpeechRecognizer(config))
        {
            TaskCompletionSource<string> recognitionCompleted = new TaskCompletionSource<string>();

            token.Register(() =>
            {
                // Stop recognition if token is cancelled
                recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
                recognitionCompleted.TrySetCanceled();
            });

            recognizer.Recognized += (s, e) =>
            {
                result = e.Result.Text;
                Debug.Log($"Recognized: {result}");
                recognitionCompleted.TrySetResult(result);
            };

            recognizer.Canceled += (s, e) =>
            {
                recognitionCompleted.TrySetCanceled();
                Debug.LogError($"Recognition canceled: {e.ErrorCode}, {e.Reason}, {e.ErrorDetails}");
            };

            recognizer.SessionStarted += (s, e) =>
            {
                // Debug.Log("\n    Session started event.");                        
            };

            recognizer.SessionStopped += (s, e) =>
            {
                // Debug.Log("\n    Session stopped event.");
            };

            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false);

            try
            {
                if (!token.IsCancellationRequested)
                {
                    result = await recognitionCompleted.Task;
                    recognitionCompleted = new TaskCompletionSource<string>();
                }
            }
            finally
            {
                await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false);
            }

            return result;
        }
    }
}
