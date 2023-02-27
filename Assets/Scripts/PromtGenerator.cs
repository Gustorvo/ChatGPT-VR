using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

public class PromtGenerator : MonoBehaviour
{
    [SerializeField] string urlToPromtFile;
    private void Start()
    {
        DownloadPromptFile(urlToPromtFile);
    }

    public void DownloadPromptFile(string url)
    {
        UnityWebRequest request = UnityWebRequest.Get(new System.Uri(url));

        StartCoroutine(DownloadCsvFileCoroutine(request));
    }

    private IEnumerator DownloadCsvFileCoroutine(UnityWebRequest request)
    {
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Download error: " + request.error);
            yield break;
        }

        string csvData = request.downloadHandler.text;
        var dict = CreateDictionaryFromCSV(csvData);
        foreach (var prompt in dict)
        {
            print(prompt.Key);
        }
    }

    static public Dictionary<string, string> CreateDictionaryFromCSV(string csvText)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        List<string[]> rows = CSVSerializer.ParseCSV(csvText);

        foreach (string[] row in rows)
        {
            if (row.Length >= 2)
            {
                dict[row[0]] = row[1];
            }
        }

        return dict;
    }
}
