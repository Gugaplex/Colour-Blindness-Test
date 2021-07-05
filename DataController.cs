using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class DataController : MonoBehaviour 
{
    public RoundData[] allRoundData;


    // Use this for initialization
    void Start ()  
    {
        DontDestroyOnLoad (gameObject);

        SceneManager.LoadScene ("MenuScreen");

        Debug.Log ("displays connected: " + Display.displays.Length);
            // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
            // Check if additional displays are available and activate each.
    
        for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
    }

    public RoundData GetCurrentRoundData()
    {
        return allRoundData [0];
    }

    // Update is called once per frame
    void Update () {

    }
}