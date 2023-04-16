using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : MonoBehaviour
{
    [SerializeField] MessageUI messageUIPrefab;
    [SerializeField] Main main;
    [SerializeField] Transform chatView;
    [SerializeField] ScrollRect scrollRect;

    private string currentText;

    private void Awake()
    {
        Main.OnCurrentStatusChanges += TakeAction;
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
    }

    private void TakeAction(SystemStatus status)
    {
        string prevText = currentText;
        Role role = Role.None;
        switch (status)
        {
            case SystemStatus.Inactive:
                break;
            case SystemStatus.Speaking:
                currentText = main.OpenAIAnswerText;
                role = Role.Assistant;
                break;
            case SystemStatus.Listerning:
                break;
            case SystemStatus.Thinking:
                currentText = main.UserRecognizedText;
                role = Role.User;
                break;
            default:
                break;
        }

        if (!string.IsNullOrEmpty(currentText))
        {
            if (currentText != prevText)
            {
                var messageUI = Instantiate(messageUIPrefab, chatView);
                messageUI.SetText(currentText);
                messageUI.SetLogoAndBackgroundColor(role);

                //// we want to set the parent after the item has been instantiated and updated its dimentions
                //messageUI.transform.SetParent(chatView);
                LayoutRebuilder.ForceRebuildLayoutImmediate(messageUI.transform as RectTransform);
               
            }
        }
    }

    private void OnScrollValueChanged(Vector2 scrollPos)
    {
        // Check if the vertical scroll position is at the bottom
       // if (scrollPos.y <= 0f)
       // {
            // Set the vertical scroll position to the maximum value
            scrollRect.verticalNormalizedPosition = 0f;        
       // }
    }

    private void OnDestroy()
    {
        Main.OnCurrentStatusChanges -= TakeAction;
        scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
    }
}
