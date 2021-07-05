using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Windows.Speech;
public class MenuScreenController : MonoBehaviour
{
    private string[] keywords = new string[] { "start" };
    public ConfidenceLevel confidence = ConfidenceLevel.Low;
    private KeywordRecognizer keywordRecognizer;
    protected string word = "";
    void Start()
    {
        keywordRecognizer = new KeywordRecognizer(keywords, confidence);
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

        Debug.Log ("displays connected: " + Display.displays.Length);
            // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
            // Check if additional displays are available and activate each.
    
        for (int i = 1; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    
    private void RecognizedSpeech(PhraseRecognizedEventArgs args)
    {
        word = args.text;


        Debug.Log("answer recorded as " + word);
        WordChecker();


    }
    public void WordChecker()
    {
        switch (word)
        {
            case "start":
                Debug.Log("[Final Answer test] You answer is :" + word);
                StartGame();
                break;
            default:
                break;
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}