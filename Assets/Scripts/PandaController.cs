using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using UnityEngine.SceneManagement;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif

enum PlayerState
{
    Idle,
    Listening,
    Talking,
    NotActive
}


public class PandaController : MonoBehaviour
{


    private pandaAIMLBot pandaBot;
    private myvoiceController speechController;
    private PlayerState playerState = PlayerState.Idle;

    //private AudioClip audioClip;
    public Text inputField;
    public Text robotOutput;
    public Text myHandleTextBox;
    private AudioSource userAudioSource;
    private AudioSource pandaOutputAudioSource;
    private Vector3 originalPosition;
    private Quaternion originalRotationValue;
    //VoiceController voiceController;
    private VoiceTest voiceTest;
    // ANIMATION
    private Animator animator;
    private float[] clipSampleData;
    private GameObject voiceC;
    // Start is called before the first frame update
    public void Start()
    {

#if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            Permission.RequestUserPermission(Permission.Microphone);
           
        }
#endif
        //voiceTest = new VoiceTest();
        voiceC = this.transform.GetChild(2).gameObject;
        voiceTest = voiceC.GetComponent<VoiceTest>();
        VoiceController voiceController = voiceC.GetComponent<VoiceController>();
        voiceTest.InitPluin();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        pandaBot = new pandaAIMLBot();
        pandaBot.Start();
        speechController = new myvoiceController();
        var aSources = GetComponents<AudioSource>();
        userAudioSource = aSources[0];
        pandaOutputAudioSource = aSources[1];
        originalPosition = gameObject.transform.position;
        originalRotationValue = transform.rotation;
        speechController.audioSource = pandaOutputAudioSource;
        speechController.myHandleTextBox = myHandleTextBox;
        animator = GetComponent<Animator>();
        speechController.animator = animator;
        robotOutput.text = "Hi, I am panda bot. How can I help you?";
        Debug.Log(robotOutput.text);
        if(voiceTest.isIntialised())
        {
            voiceTest.TTS(robotOutput.text);
            robotOutput.text = "true";
        }else
        {
            StartCoroutine(speechController.sendRequestToRSSAndPlayAudio(robotOutput.text));
        }
        animator.Play("talking");
        animator.Play("waving");
        clipSampleData = new float[1024];
        userAudioSource.clip = null;
        //Invoke("voiceTest.TTS(robotOutput.text)", 1);
        Idle();
  

    }

    private void Idle()
    {
        if (animator != null)
        {
            gameObject.transform.position = originalPosition;
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue, Time.time * 1.0f);



            //if (animator.GetCurrentAnimatorStateInfo(0).IsName("sitting"))
            //{
            //    Debug.Log("sitting");
            //    animator.Play("standup");
            //    gameObject.transform.position = originalPosition;
            //    transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue, Time.time * 1.0f);
            //}
            
            Debug.Log("Player state is Idle.");

            if (userAudioSource.clip != null)// if playback happened
            {
                userAudioSource.Stop();
                userAudioSource.clip = null;
            }
#if PLATFORM_ANDROID
            if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                userAudioSource.clip = Microphone.Start(null, true, 1, 16000);
            }
            else
            {
                inputField.text = "Please allow microphone permission.";
            }
#endif
            animator.Play("idle1");
        }
    }




    // Update is called once per frame
    public void Update()
    {

        // if (playerState == PlayerState.Idle && !speechController.activeRequest && !pandaOutputAudioSource.isPlaying)
        if (playerState == PlayerState.Idle && !voiceTest.isSpeaking())
        {
            //Debug.Log("Robot is not playing audio");

            //if (true)
            if (isUserSpeaking())
            {
                Debug.Log("Need to switch state.");
                SwitchState();
            }


        }

        if (Input.GetKey(KeyCode.Escape))
        {

            quit();
        }




    }

    private bool isUserSpeaking()
    {

        if (userAudioSource.clip == null)
            return false;

        userAudioSource.clip.GetData(clipSampleData, userAudioSource.timeSamples); //Read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
        var clipLoudness = 0f;
        foreach (var sample in clipSampleData)
        {
            clipLoudness += Mathf.Abs(sample);
        }
        clipLoudness /= 1024;

        //if(clipLoudness > 0.005f)
        // sDebug.Log("Clip Loudness = " + clipLoudness);

        return clipLoudness > 0.0005f;
    }

    private void SwitchState()
    {
        switch (playerState)
        {
            case PlayerState.Idle:
                playerState = PlayerState.Listening;
                Microphone.End(null);
                Listen();
                break;

            case PlayerState.Listening:
                playerState = PlayerState.Talking;
                //inputField.text = "Saving Voice Request";
                //Microphone.End(null);

                //if (SavWav.Save("sample", userAudioSource.clip))
                //{
                //    inputField.text = "Sending audio to AI...";
                //}
                //else
                //{
                //    inputField.text = "FAILED";
                //}

                //// At this point, we can delete the existing audio clip
                //userAudioSource.clip = null;

                //Start a coroutine called "WaitForRequest" with that WWW variable passed in as an argument
                // StartCoroutine(speechController.SendRequestToWitAi());

                //Invoke("Talk", 6);

                inputField.text = "Thinking ...";

                Talk();
                break;

            case PlayerState.Talking:
                playerState = PlayerState.Idle;
                Idle();
                break;
        }
    }

    private void Talk()
    {
        //inputField.text = "Talk called.....";

        if (animator != null)
        {
            Debug.Log("Talk called");
            //inputField.text = "Talk called.";
            gameObject.transform.position = originalPosition;
            
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue, Time.time * 1.0f);
            if (!(voiceTest.myResponse == null))
            {
                inputField.text = "My question is: " + voiceTest.myResponse;
                char[] a = voiceTest.myResponse.ToCharArray();
                a[0] = char.ToUpper(a[0]);
                string userAudioResponse = new string(a);
                string answer = pandaBot.getResponse(userAudioResponse);
                voiceTest.TTS(answer);
                //StartCoroutine(speechController.sendRequestToRSSAndPlayAudio(answer));
                robotOutput.text = answer;
                SwitchState();

                //Invoke("SwitchState", 4);
            }
            else
            {
                myHandleTextBox.text = "Please speak loudly.";
                Invoke("SwitchState", 1);
                
            }

        }else
        {
            inputField.text = "Animator is not running.";
        }

    }

    private void Listen()
    {

        if (animator != null)
        {
            Debug.Log("Listening state");
            gameObject.transform.position = originalPosition;
            transform.rotation = Quaternion.Slerp(transform.rotation, originalRotationValue, Time.time * 1.0f);

            //if (animator.name == "sitting")
           // animator.Play("idle1"); 
           
            inputField.text = "Listening for your command..";
            voiceTest.GetSpeech();
            //userAudioSource.clip = Microphone.Start(null, false, 5, 16000);
            //voiceController.GetSpeech();


            Invoke("SwitchState",5);
        }

    }

    public void quit()
    {
        userAudioSource.Stop();
        pandaOutputAudioSource.Stop();
        pandaBot.quit();
    }

}


