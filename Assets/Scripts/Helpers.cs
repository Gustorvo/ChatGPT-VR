using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static List<string> ToChunks(this string text, int maxChunkSize)
    {      
        List<string> chunks = new List<string>();

        while (text.Length > 0)
        {
            int length = Mathf.Min(text.Length, maxChunkSize);

            string chunk = text.Substring(0, length);

            int lastSentenceEnd = chunk.LastIndexOfAny(new char[] { '.', '?', '!', ':' });

            if (lastSentenceEnd != -1)
            {
                // Split at the end of the sentence
                chunk = chunk.Substring(0, lastSentenceEnd + 1);
                text = text.Substring(lastSentenceEnd + 1);
            }
            else
            {
                // Split at the maximum chunk size
                text = text.Substring(length);
            }

            chunks.Add(chunk);
        }
        return chunks;
    }
}
