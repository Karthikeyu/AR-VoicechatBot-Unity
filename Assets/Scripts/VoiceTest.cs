using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VoiceController))]
public class VoiceTest : MonoBehaviour {

    //public Text uiText;
    public string myResponse;

     VoiceController voiceController;

    public void GetSpeech() {
        myResponse = null;
        voiceController.GetSpeech();
    }

    void Start() {
        myResponse = null;
        voiceController = GetComponent<VoiceController>();
    }

    void OnEnable() {
        VoiceController.resultRecieved += OnVoiceResult;
    }

    void OnDisable() {
        VoiceController.resultRecieved -= OnVoiceResult;
    }

    void OnVoiceResult(string text) {
        myResponse = text;
    }
}
