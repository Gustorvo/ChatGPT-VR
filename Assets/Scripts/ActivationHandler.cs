using Facebook.WitAi.Dictation.Data;
using NaughtyAttributes;
using Oculus.Voice.Dictation;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ActivationHandler : MonoBehaviour
{
    [SerializeField] private AppDictationExperience voice;
    [SerializeField] bool reactivateOnInactivity = true;
    [SerializeField] bool activateOnStart = false;

    private void Awake()
    {
        // voice = FindObjectOfType<AppDictationExperience>();
        Assert.IsNotNull(voice);
        //if (!voice.MicActive)
        //    ActivateImmediately();
        //else
        //{
        //    voice.OnInitialized -= ActivateImmediately;
        //    voice.OnInitialized += ActivateImmediately;
        //}

        //voice.DictationEvents.OnMicStoppedListening.AddListener(() => print("OnMicStoppedListening DictationEvents"));
        //voice.DictationEvents.onDictationSessionStopped.AddListener((_) => print("onDictationSessionStopped"));
        //voice.DictationEvents.onStopped.AddListener(() => print("onStopped"));
        voice.AudioEvents.OnMicStoppedListening.RemoveListener(Restart);
        voice.AudioEvents.OnMicStoppedListening.AddListener(Restart);
    }

    private void Restart()
    {
        if (waitCoroutine == null && reactivateOnInactivity)
            waitCoroutine = StartCoroutine(RestartOnInactivity());
    }

    private Coroutine waitCoroutine;
    private IEnumerator RestartOnInactivity()
    {
        yield return new WaitUntil(() => voice.Active);
        print("Restarting voice service due to inavtivity");
        voice.Activate();
        waitCoroutine = null;

    }   

 
    private void ActivateImmediately()
    {
        print("force activating");
    }
}
