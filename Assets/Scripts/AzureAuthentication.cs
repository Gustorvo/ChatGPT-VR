using Microsoft.CognitiveServices.Speech;
using System.Linq;
using System.Security.Authentication;
using UnityEngine;

public class AzureAuthentication
{
    private string apiKey;
    private string region;
    public AzureAuthentication(string apiKey, string region)
    {
        this.apiKey = apiKey;
        this.region = region;
    } 

    private static AzureAuthentication cachedDefault;

    public static AzureAuthentication Default
    {
        get
        {
            if (cachedDefault != null)
            {
                return cachedDefault;
            }

            var auth = LoadFromAsset();
            cachedDefault = auth;
            return auth;
        }
        internal set => cachedDefault = value;
    }

    private static AzureAuthentication LoadFromAsset()
            => Resources.LoadAll<AzureConfigurationSettings>(string.Empty)
                .Where(asset => asset != null)
                .Where(asset => !string.IsNullOrWhiteSpace(asset.ApiKey))
                .Select(asset => new AzureAuthentication(asset.ApiKey, asset.Region)).FirstOrDefault();

    /// <summary>
    ///  Creates an instance of a speech config with subscription key and service region.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="AuthenticationException"></exception>
    public static SpeechConfig InitAzureSpeech()
    {
        var config = Default;

        if (config?.apiKey is null)
        {
            throw new AuthenticationException("You must provide API authentication for Azure services.");
        }
        return SpeechConfig.FromSubscription(config?.apiKey, config?.region);
    }
}
