using UnityEngine;
using UnityEngine.UI;

public class VoiceController : MonoBehaviour { 

    AndroidJavaObject activity;
    AndroidJavaObject STTplugin;
    AndroidJavaObject TTSplugin = null;
    private bool isintialised = false;

    public delegate void OnResultRecieved(string result);
    public static OnResultRecieved resultRecieved;

    //private void Start() {
    //    InitPlugin();
    //}

    public void InitPlugin() {
        Debug.Log("true");
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

        activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
                STTplugin = new AndroidJavaObject(
                "com.example.matthew.plugin.VoiceBridge");
        }));

       /* activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            TTSplugin = new AndroidJavaObject(
            "com.busa.ttslibrary.androidSpeechRecognition");
        }));*/


        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            STTplugin.Call("StartPlugin");
        }));

        using (TTSplugin = new AndroidJavaClass("com.busa.ttslibrary.androidSpeechRecognition"))
        {
            if (TTSplugin != null)
            {

                TTSplugin = TTSplugin.CallStatic<AndroidJavaObject>("instance");
                TTSplugin.Call("setContext", activity);
            }
        }

        isintialised = true;
        

    }

    /// <summary>
    /// gets called via SendMessage from the android plugin GameObject must be called "VoiceController"
    /// </summary>
    /// <param name="recognizedText">recognizedText.</param>
    public void OnVoiceResult(string recognizedText) {
        Debug.Log(recognizedText);
        resultRecieved?.Invoke(recognizedText);
    }

    /// <summary>
    /// gets called via SendMessage from the android plugin
    /// </summary>
    /// <param name="error">Error.</param>
    public void OnErrorResult(string error) {
        Debug.Log(error);
    }

    public void GetSpeech() {
        // Calls the function from the jar file
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            STTplugin.Call("StartSpeaking");
        }));
    }

    public void TTS(string text, float pitch)
    {
        TTSplugin.Call("SpeakTTS", "US", text, pitch, 1.0f);
    }

    public bool isSpeaking()
    {
        bool a = TTSplugin.Call<bool>("isSpeaking");
        return a;
    }

    public bool isIntialised()
    {
        return isintialised;
    }

}
