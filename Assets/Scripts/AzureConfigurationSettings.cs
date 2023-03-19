using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(AzureConfigurationSettings), menuName = "Azure/" + nameof(AzureConfigurationSettings), order = 0)]
public class AzureConfigurationSettings : ScriptableObject
{   
    [SerializeField, Tooltip("The Azure api key.")]
    internal string apiKey;

    /// <summary>
    /// The Azure api key.
    /// </summary>
    public string ApiKey => apiKey;

    [SerializeField]  
    internal string region;

    /// <summary>
    /// The location(or region) of your resource. You may need to use this field when making calls to this API.
    /// </summary>
    public string Region => region;
}
