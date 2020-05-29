using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class demo : MonoBehaviour
{
    public GameObject[] welcomePanelGameObjects;
    public GameObject[] normalPlaneGameObjects;
    // Start is called before the first frame update
    void Start()
    {


        Screen.sleepTimeout = SleepTimeout.NeverSleep;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {

            Application.Quit();
        }

    }

    public void changeToNormalPanel(bool value)
    {
        foreach (var item in welcomePanelGameObjects)
        {
            item.SetActive(!value);
        }
        foreach (var item in normalPlaneGameObjects)
        {
            item.SetActive(value);
        }
    }

    public void loadScene(string text)
        {
        SceneManager.LoadScene(text);
        Debug.Log("Sceneloaded,");
    }
}
