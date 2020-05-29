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


    public void InitPluin()
    {
      
        voiceController = GetComponent<VoiceController>();
        // voiceController = new VoiceController();
        myResponse = null;
        voiceController.InitPlugin();
    }



    public  IEnumerator TTS(string text)
    {

        voiceController.TTS(text, 1.0f);
        return null;
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

    public bool isSpeaking()
    {
        return voiceController.isSpeaking();
    }

    public bool isIntialised()
    {
        bool a = voiceController.isIntialised();
        return a;

    }

}
