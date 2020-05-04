using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class myvoiceController : MonoBehaviour
{

    // Audio variables
    public AudioClip audioClip;
    private int samplerate = 16000;
    public AudioSource audioSource;
    public bool activeRequest = false;
    public Text myHandleTextBox;
    public string witAIResponse = null;

    // API access parameters
    private string wit_url = "https://api.wit.ai/speech?v=20180206";
    private string wit_apiKey = "GS6J4YIN3645G6I3SDCJBE76PGHWTM7F";

    string rss_APIKey = "4741efdc3fa44cb6b890899e9a84e051";
    string rss_url = "https://api.voicerss.org/?key=";


    // GameObject to use as a default spawn point
    private bool isRecording = false;
    private bool pressedButton = false;

    public Animator animator;
    


   
    void Start()
    {
     

    }

  
    void Update()
    {
        
    }


    public IEnumerator sendRequestToRSSAndPlayAudio(string text)
    {
        activeRequest = true;
        string total_url = rss_url + rss_APIKey + "&hl=en-us&src=" + text + "&c=WAV";
        UnityEngine.Debug.Log("URL: " + total_url);
        WWW www = new WWW(total_url);
        yield return www;
        audioClip = www.GetAudioClip(false, true, AudioType.WAV);


        if (audioClip.length > 0 && audioClip != null)
        {

            audioSource.outputAudioMixerGroup.audioMixer.SetFloat("pitchBend", 1f / 0.7f);
            audioSource.clip = audioClip;
            audioSource.Play();
            animator.Play("talking");
            UnityEngine.Debug.Log("Audio played.");
        }
        else
        {
            UnityEngine.Debug.LogError("Failed to get the voice from API RSS.");

        }
        activeRequest = false;
    }


    /*
    public IEnumerator SendRequestToWitAi()
    {
        witAIResponse = null;

        string file = Application.persistentDataPath + "/sample.wav";

        FileStream filestream = new FileStream(file, FileMode.Open, FileAccess.Read);
        BinaryReader filereader = new BinaryReader(filestream);
        byte[] postData = filereader.ReadBytes((Int32)filestream.Length);
        filestream.Close();
        filereader.Close();

        //Custom 7
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers["Content-Type"] = "audio/wav";
        headers["Authorization"] = "Bearer " + wit_apiKey;

        float timeSent = Time.time;
        WWW www = new WWW(wit_url, postData, headers);


        yield return www;

        while (!www.isDone)
        {
            yield return null;
        }
        float duration = Time.time - timeSent;

        if (www.error != null && www.error.Length > 0)
        {
            UnityEngine.Debug.Log("Error: " + www.error + " (" + duration + " secs)");
            yield break;
        }
        UnityEngine.Debug.Log("Success (" + duration + " secs)");
        UnityEngine.Debug.Log("Result: " + www.text);

        //Text myHandleTextBox = inputField;
        Pair<bool, string> pair = Handle(www.text);
        if (pair.First())
            witAIResponse = pair.Second();
        myHandleTextBox.text = "You: " + pair.Second();

  
    }
    */

    



}
