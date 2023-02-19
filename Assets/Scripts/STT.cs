using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Assertions;
using Microsoft.CognitiveServices.Speech;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

public class STT : MonoBehaviour
{  
    [SerializeField] TextAsset subscriptionKey;
    [SerializeField] string region = "northeurope";
    
#if PLATFORM_ANDROID
    // Required to manifest microphone permission, cf.
    // https://docs.unity3d.com/Manual/android-manifest.html
    private Microphone mic;
#endif
    private SpeechConfig config;
  

    private void Awake()
    {
        Assert.IsNotNull(subscriptionKey);
        Init();
    }
    void Start()
    {
#if PLATFORM_ANDROID
        // Request to use the microphone, cf.
        // https://docs.unity3d.com/Manual/android-RequestingPermissions.html

        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
        }
#endif
    }

    private void Init()
    {
        // Creates an instance of a speech config with specified subscription key and service region.      
        config = SpeechConfig.FromSubscription(subscriptionKey.ToString(), region);
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
                Debug.LogError($"Recognized: {result}");
                recognitionCompleted.TrySetResult(result);
            };

            recognizer.Canceled += (s, e) =>
            {
                Debug.LogError($"Recognition canceled: {e.ErrorCode}, {e.Reason}, {e.ErrorDetails}");
                recognitionCompleted.TrySetCanceled();
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
