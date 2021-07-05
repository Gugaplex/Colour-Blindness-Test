using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using System.IO;
//using UnityEngine.Networking; // For Database

using System;
using System.Diagnostics;
using UnityEngine.Windows.Speech;
using Debug = UnityEngine.Debug;
using UnityEngine.Rendering;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Text;

public class GameController : MonoBehaviour {

    NumericData[] answeredData;
    private string[] keywords;/*= new string[] { "a", "b", "c","see", "dee", "yi", "eff", "yes" };*/
    public ConfidenceLevel confidence = ConfidenceLevel.Low;
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();
    protected string word = "";

    

    public Text questionDisplayText;
    public Text scoreDisplayText;
    public Text controlScoreDisplayText;
    public Text redWeakScoreDisplayText;
    public Text redStrongScoreDisplayText;
    public Text greenWeakScoreDisplayText;
    public Text greenStrongScoreDisplayText;
    public Text totalBlindnessScoreDisplayText;
    public Image numberPlateDisplay;
    public SimpleObjectPool answerButtonObjectPool;
    public Transform answerButtonParent;
    public GameObject questionDisplay;
    public GameObject idDisplay;
    public GameObject newUserDisplay;
    public GameObject roundEndDisplay;
    public InputField patientIdInput;
    public InputField nameFirstInput;
    public InputField nameLastInput;
    public InputField ageInput;
    public ToggleGroup sexInput;
    public ToggleGroup ethnicityInput;
    public Camera vrCamera;
    public Camera normalCamera;

    public GameObject BackBtn;
    public String[] Labels;
    public Questions[] numberOfQuestions;

    private DataController dataController;
    private RoundData currentRoundData;
    private QuestionData[] questionPool;
    private QuestionData[] tempPool;

    
    private Toggle sexToggle;
    private Toggle ethnicityToggle;

    

    private int myId;    
    private string myFirstName;
    private string myLastName;
    private string myAge;
    private string mySex;
    private string myEthnicity;
    private bool isRoundActive;
    private int questionIndex;
    private int playerScore;
    private int controlScore;
    private int redMildScore;
    private int redStrongScore;
    private int greenMildScore;
    private int greenStrongScore;
    private int totalBlindnessScore;

   
    private List<GameObject> answerButtonGameObjects = new List<GameObject>();

    public string patientData;
    public string ishiharaData;

    [Serializable]
    public class CreatePatient
    {
        public string first_name;
        public string last_name;
        public string sex;
        public string ethnicity;
    }
    public void savePatient()
    {
        CreatePatient createPatient = new CreatePatient();
        createPatient.first_name = myFirstName;
        createPatient.last_name = myLastName;
        createPatient.sex = mySex;
        createPatient.ethnicity = myEthnicity;

        patientData = JsonConvert.SerializeObject(createPatient);
        Debug.Log(patientData);
    }

    public class AddIshiTest
    {
        public int patient_id;
        public List<IshiTest> tests;
    }

    public class IshiTest
    {
        public string test_name;
        public string test_datetime;
        public List<IshiharaTestResult> test_results;
    }
    public class IshiharaTestResult
    {
        public List<MainScore> main_score;
        public List<ControlScore> control_score;
        public List<RedWeaknessScore> red_weakness_score;
        public List<RedDeficiencyScore> red_deficiency_score;
        public List<GreenWeaknessScore> green_weakness_score;
        public List<GreenDeficiencyScore> green_deficiency_score;
    }
    public class MainScore
    {
        public int total;
        public int obtained;
    }

    public class ControlScore
    {
        public int total;
        public int obtained;
    }

    public class RedWeaknessScore
    {
        public int total;
        public int obtained;
    }

    public class RedDeficiencyScore
    {
        public int total;
        public int obtained;
    }

    public class GreenWeaknessScore
    {
        public int total;
        public int obtained;
    }

    public class GreenDeficiencyScore
    {
        public int total;
        public int obtained;
    }

    public void saveIshiharaTest()
    {
        AddIshiTest createIshiTest = new AddIshiTest
        {
            patient_id = myId,
            tests = new List<IshiTest>
            {
                new IshiTest()
                {
                    test_name = "Ishihara Test",
                    test_datetime = DateTime.Now.ToString("dd/MM/yy") + ", " + DateTime.Now.ToString("HH:mm"),
                    test_results = new List<IshiharaTestResult>
                    {
                        new IshiharaTestResult()
                        {
                            main_score = new List<MainScore>
                            {
                                new MainScore()
                                {
                                    total = 14,
                                    obtained = playerScore
                                }
                            },
                            control_score = new List<ControlScore>
                            {
                                new ControlScore()
                                {
                                    total = 1,
                                    obtained = controlScore
                                }
                            },
                            red_weakness_score = new List<RedWeaknessScore>
                            {
                                new RedWeaknessScore()
                                {
                                    total = 2,
                                    obtained = redMildScore
                                }
                            },
                            red_deficiency_score = new List<RedDeficiencyScore>
                            {
                                new RedDeficiencyScore()
                                {
                                    total = 2,
                                    obtained = redStrongScore
                                }
                            },
                            green_weakness_score = new List<GreenWeaknessScore>
                            {
                                new GreenWeaknessScore()
                                {
                                    total = 2,
                                    obtained = greenMildScore
                                }
                            },
                            green_deficiency_score = new List<GreenDeficiencyScore>
                            {
                                new GreenDeficiencyScore()
                                {
                                    total = 2,
                                    obtained = greenStrongScore
                                }
                            }
                        }
                    }
                }

            }
        };
        ishiharaData = JsonConvert.SerializeObject(createIshiTest);

        const string quote = "\"";
        ishiharaData = ishiharaData.Replace("[{" + quote + "total", "{" + quote + "total").Replace("}],", "},").Replace("}]}]}","}}]}");

        Debug.Log(ishiharaData);


    }

    IEnumerator Post(string url, string bodyJsonString)
    {
         
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        
        Debug.Log("Status Code: " + request.responseCode);
    }

    public void UploadPatient()
    {
        StartCoroutine(Post("http://3.0.17.212:3000/create/patient", patientData));
    }

    public void UploadIshiharaTest()
    {
        StartCoroutine(Post("http://3.0.17.212:3000/add/test", ishiharaData));
        
    }

    void Awake()
    {
        int totalCount=0;
        for(int i=0;i<numberOfQuestions.Length;i++)
        {
            totalCount += numberOfQuestions[i].cases.Length;
        }
        keywords = new string[totalCount+2];
        int c = 0;
        for(int i=0;i< numberOfQuestions.Length; i++)
        {
            for (int a = 0; a < numberOfQuestions[i].cases.Length; a++)
            {
                
                keywords[c]= numberOfQuestions[i].cases[a];
                c++;
            }
        }
        keywords[keywords.Length - 2] = "yes";
        keywords[keywords.Length - 1] = "back";
    }

    // Use this for initialization
    void Start () 
    {
        
        dataController = FindObjectOfType<DataController> ();
        currentRoundData = dataController.GetCurrentRoundData ();
        questionPool = currentRoundData.questions;
        Image numberPlateDisplay = GetComponent<Image>();
        answeredData =new NumericData[questionPool.Length];
        Shuffle();

        playerScore = 0;
        controlScore = 0;
        questionIndex = 0;
        redMildScore = 0;
        redStrongScore = 0;
        greenMildScore = 0;
        greenStrongScore = 0;
        totalBlindnessScore = 0;
        
        //url = "3.0.17.212:3000/create/patient";
        //json = "";
        
       
        ShowQuestion ();
        isRoundActive = false;

        keywordRecognizer = new KeywordRecognizer(keywords, confidence);
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeech;
        keywordRecognizer.Start();

        //send to mongoDB
        //StartCoroutine(Upload(url, json));

    }

    public void Shuffle()
    {
       
        for (int i = 1; i < questionPool.Length; i++)
        {      
            QuestionData tempPool = questionPool[i];
            int rnd = UnityEngine.Random.Range(1,i);
            questionPool[i] = questionPool[rnd];
            questionPool[rnd] = tempPool;

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
       

       /* Debug.Log("answer recorded as " + word);*/
        WordChecker();


    }

    bool check = false;

    

    public void WordChecker()
    {
        check = false;
        for (int z = 0; z < numberOfQuestions.Length; z++)
        {
            for(int a=0;a<numberOfQuestions[z].cases.Length;a++)
            {
                if(word== numberOfQuestions[z].cases[a])
                {
                    for (int i = 0; i < answerButtonParent.childCount; i++)
                    {
                        answerButtonParent.transform.GetChild(i).GetComponent<Button>().image.color = Color.white;
                    }
                    BackBtn.GetComponent<Button>().image.color = Color.white;
                    answerButtonParent.transform.GetChild(z).GetComponent<Button>().image.color = Color.green;
                    check = true;
                    break;
                }
              
            }
            if (check)
                break;
        }
          if(word=="back")
        {
            for (int i = 0; i < answerButtonParent.childCount; i++)
            {
                answerButtonParent.transform.GetChild(i).GetComponent<Button>().image.color = Color.white;
            }
            BackBtn.GetComponent<Button>().image.color = Color.green;
        }
        if (word == "yes")
        {
            for (int i = 0; i < answerButtonParent.childCount; i++)
            {
                if (answerButtonParent.transform.GetChild(i).GetComponent<Button>().image.color == Color.green)
                    answerButtonParent.transform.GetChild(i).GetComponent<AnswerButton>().HandleClick();

            }
            if(BackBtn.GetComponent<Button>().image.color == Color.green)
            {
                BackBtnClicked();
                BackBtn.GetComponent<Button>().image.color = Color.white;
            }
                
        }
        Debug.Log("[Final Answer test] You first answer is :" + word); 
     
    }

    
    private void Save()
    {
        myId = int.Parse(patientIdInput.text);
        myFirstName = nameFirstInput.text;
        myLastName = nameLastInput.text;
        myAge = ageInput.text;
        CheckSexToggleOn();
        CheckEthnicityToggleOn();

        string path = "C:/Users/taysh/OneDrive/Desktop/GreenDeficiency/Results/" + myId + "test.txt";
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Colour Blindness Test Results \n");
        }

        string content = DateTime.Now + "\n" + "Name: " + myFirstName + " " + myLastName + "\n" + "Age: " + myAge + "\n" + "Sex: " + mySex + "\n" 
        + "Ethnicity: " + myEthnicity + "\n" + "Main Score: " + playerScore + "/14 \n" + "Control Score: " + controlScore + "/1 \n" + "Red Weakness Score: " +
        redMildScore + "/2 \n" + "Red Deficiency Score: " + redStrongScore + "/2 \n" + "Green Weakness Score: " + greenMildScore + "/2 \n"
        + "Green Deficiency Score: " + greenStrongScore + "/2 \n" + "Total Colour Blindness Score: " + totalBlindnessScore + "/6 \n";

        File.AppendAllText(path, content);

    }


    // Call this function to enable Normal camera,
    // and enable VR camera.
    public void ShowNormalView()
    {
        vrCamera.enabled = false;
        normalCamera.enabled = true;
    }

    // Call this function to enable VR camera,
    // and disable Normal camera.
    public void ShowVRView()
    {
        vrCamera.enabled = true;
        normalCamera.enabled = false;
    }

    private void ShowQuestion()
    {
        RemoveAnswerButtons ();
        QuestionData questionData = questionPool [questionIndex];
        questionDisplayText.text = questionData.questionText;
        numberPlateDisplay.sprite = questionData.numberPlate;


        for (int i = 0; i < questionData.answers.Length; i++) 
        {
            GameObject answerButtonGameObject = answerButtonObjectPool.GetObject();
            answerButtonGameObjects.Add(answerButtonGameObject);
            answerButtonGameObject.transform.SetParent(answerButtonParent);

            AnswerButton answerButton = answerButtonGameObject.GetComponent<AnswerButton>();
            answerButton.Setup(questionData.answers[i]);
        }
    }

    private void RemoveAnswerButtons()
    {
        while (answerButtonGameObjects.Count > 0) 
        {
            answerButtonObjectPool.ReturnObject(answerButtonGameObjects[0]);
            answerButtonGameObjects.RemoveAt(0);
        }
    }


    public void ControlButtonClicked(bool isControl)
    {
        if (isControl) 
        {
            controlScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Main Score: " + playerScore + " / 14";
            controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
            redWeakScoreDisplayText.text = "Red-weakness: " + redMildScore + " / 2";
            redStrongScoreDisplayText.text = "Red-blindness: " + redStrongScore + " / 2";
            greenWeakScoreDisplayText.text = "Green-weakness: " + greenMildScore + " / 2";
            greenStrongScoreDisplayText.text = "Green-blindness: " + greenStrongScore + " / 2";
            totalBlindnessScoreDisplayText.text = "Total Colour Blindness: " + totalBlindnessScore + " / 6";
            
        }


    }
    

    public void MildRButtonClicked(bool isMildR)
    {
        if (isMildR) 
        {
            redMildScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Main Score: " + playerScore + " / 14";
            controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
            redWeakScoreDisplayText.text = "Red-weakness: " + redMildScore + " / 2";
            redStrongScoreDisplayText.text = "Red-blindness: " + redStrongScore + " / 2";
            greenWeakScoreDisplayText.text = "Green-weakness: " + greenMildScore + " / 2";
            greenStrongScoreDisplayText.text = "Green-blindness: " + greenStrongScore + " / 2";
            totalBlindnessScoreDisplayText.text = "Total Colour Blindness: " + totalBlindnessScore + " / 6";
            
        }

    }

    public void StrongRButtonClicked(bool isStrongR)
    {
        if (isStrongR) 
        {
            redStrongScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Main Score: " + playerScore + " / 14";
            controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
            redWeakScoreDisplayText.text = "Red-weakness: " + redMildScore + " / 2";
            redStrongScoreDisplayText.text = "Red-blindness: " + redStrongScore + " / 2";
            greenWeakScoreDisplayText.text = "Green-weakness: " + greenMildScore + " / 2";
            greenStrongScoreDisplayText.text = "Green-blindness: " + greenStrongScore + " / 2";
            totalBlindnessScoreDisplayText.text = "Total Colour Blindness: " + totalBlindnessScore + " / 6";
            
        }

    }

    public void MildGButtonClicked(bool isMildG)
    {
        if (isMildG) 
        {
            greenMildScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Main Score: " + playerScore + " / 14";
            controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
            redWeakScoreDisplayText.text = "Red-weakness: " + redMildScore + " / 2";
            redStrongScoreDisplayText.text = "Red-blindness: " + redStrongScore + " / 2";
            greenWeakScoreDisplayText.text = "Green-weakness: " + greenMildScore + " / 2";
            greenStrongScoreDisplayText.text = "Green-blindness: " + greenStrongScore + " / 2";
            totalBlindnessScoreDisplayText.text = "Total Colour Blindness: " + totalBlindnessScore + " / 6";
            
            
        }

    }

    public void StrongGButtonClicked(bool isStrongG)
    {
        if (isStrongG) 
        {
            greenStrongScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Main Score: " + playerScore + " / 14";
            controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
            redWeakScoreDisplayText.text = "Red-weakness: " + redMildScore + " / 2";
            redStrongScoreDisplayText.text = "Red-blindness: " + redStrongScore + " / 2";
            greenWeakScoreDisplayText.text = "Green-weakness: " + greenMildScore + " / 2";
            greenStrongScoreDisplayText.text = "Green-blindness: " + greenStrongScore + " / 2";
            totalBlindnessScoreDisplayText.text = "Total Colour Blindness: " + totalBlindnessScore + " / 6";
           
            
        }

    }

    public void SeverityButtonClicked(bool isSeverity)
    {
        if (isSeverity) 
        {
            totalBlindnessScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Main Score: " + playerScore + " / 14";
            controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
            redWeakScoreDisplayText.text = "Red-weakness: " + redMildScore + " / 2";
            redStrongScoreDisplayText.text = "Red-blindness: " + redStrongScore + " / 2";
            greenWeakScoreDisplayText.text = "Green-weakness: " + greenMildScore + " / 2";
            greenStrongScoreDisplayText.text = "Green-blindness: " + greenStrongScore + " / 2";
            totalBlindnessScoreDisplayText.text = "Total Colour Blindness: " + totalBlindnessScore + " / 6";
            
        }
        

    }

    public void AnswerButtonClicked(bool isCorrect) 
    {
        if (isCorrect) 
        {
            playerScore += currentRoundData.pointsAddedForCorrectAnswer;
            scoreDisplayText.text = "Main Score: " + playerScore + " / 14";
            controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
            redWeakScoreDisplayText.text = "Red-weakness: " + redMildScore + " / 2";
            redStrongScoreDisplayText.text = "Red-blindness: " + redStrongScore + " / 2";
            greenWeakScoreDisplayText.text = "Green-weakness: " + greenMildScore + " / 2";
            greenStrongScoreDisplayText.text = "Green-blindness: " + greenStrongScore + " / 2";
            totalBlindnessScoreDisplayText.text = "Total Colour Blindness: " + totalBlindnessScore + " / 6";
            
           
        }

        answeredData[questionIndex]=new NumericData();
        answeredData[questionIndex].playerScore = playerScore;
        answeredData[questionIndex].controlScore = controlScore;
        answeredData[questionIndex].redMildScore = redMildScore;
        answeredData[questionIndex].redStrongScore = redStrongScore;
        answeredData[questionIndex].greenMildScore = greenMildScore;
        answeredData[questionIndex].greenStrongScore = greenStrongScore;
        answeredData[questionIndex].totalBlindnessScore = totalBlindnessScore;

        if (questionPool.Length > questionIndex + 1) 
        {
            questionIndex++;
            ShowQuestion ();
        }
        else 
        {
            EndRound();
        }

    }

    public void BackBtnClicked()
    {
        if(questionIndex>0 && isRoundActive)
        {
            if (questionIndex > 1)
            {
                //answeredData[questionIndex - 2] = new NumericData();
                playerScore = answeredData[questionIndex - 2].playerScore;
                controlScore= answeredData[questionIndex-2].controlScore;
                redMildScore =answeredData[questionIndex-2].redMildScore;
                redStrongScore = answeredData[questionIndex-2].redStrongScore;
                greenMildScore = answeredData[questionIndex-2].greenMildScore;
                greenStrongScore= answeredData[questionIndex-2].greenStrongScore;
                totalBlindnessScore= answeredData[questionIndex-2].totalBlindnessScore;

            }
            else
            {
                playerScore = 0;
                controlScore = 0;
                redMildScore = 0;
                redStrongScore = 0;
                greenMildScore = 0;
                greenStrongScore = 0;
                totalBlindnessScore = 0;
            }

            questionIndex--;
            ShowQuestion();
              
        }
        if(!isRoundActive)
        {
            BackFromMenu();
        }
        scoreDisplayText.text = "Score: " + playerScore + " / 14";
        controlScoreDisplayText.text = "Control Score: " + controlScore + " / 1";
        redWeakScoreDisplayText.text = "Red Weakness: " + redMildScore + " / 2";
        redStrongScoreDisplayText.text = "Red Deficiency: " + redStrongScore + " / 2";
        greenWeakScoreDisplayText.text = "Green Weakness: " + greenMildScore + " / 2";
        greenStrongScoreDisplayText.text = "Green Deficiency: " + greenStrongScore + " / 2";
        totalBlindnessScoreDisplayText.text = "Total Blindness: " + totalBlindnessScore + " / 6";
    }
    
    public void BackFromMenu()
    {
        isRoundActive = true;

        questionDisplay.SetActive(true);
        roundEndDisplay.SetActive(false);
        playerScore = answeredData[questionIndex - 2].playerScore;
        controlScore = answeredData[questionIndex - 2].controlScore;
        redMildScore = answeredData[questionIndex - 2].redMildScore;
        redStrongScore = answeredData[questionIndex - 2].redStrongScore;
        greenMildScore = answeredData[questionIndex - 2].greenMildScore;
        greenStrongScore = answeredData[questionIndex - 2].greenStrongScore;
        totalBlindnessScore = answeredData[questionIndex - 2].totalBlindnessScore;
        
        
        ShowQuestion();
    }

    public void CheckSexToggleOn ()
    {
        foreach (Toggle sexToggle in sexInput.ActiveToggles()) 
        {
            if (sexToggle.isOn == true) {
                mySex = (sexToggle.name);
                break;
            }
        }
    }

    public void CheckEthnicityToggleOn ()
    {
        foreach (Toggle ethnicityToggle in ethnicityInput.ActiveToggles()) 
        {
            if (ethnicityToggle.isOn == true) {
                myEthnicity = (ethnicityToggle.name);
                break;
            }
        }
    }


    public void GoToPatientId()
    {
        newUserDisplay.SetActive (false);
        idDisplay.SetActive (true);
    }

    public void SaveAndGoToPatientId()
    {
        myFirstName = nameFirstInput.text;
        myLastName = nameLastInput.text;
        myAge = ageInput.text;
        CheckSexToggleOn();
        CheckEthnicityToggleOn();
        savePatient();
        UploadPatient();
        newUserDisplay.SetActive (false);
        idDisplay.SetActive (true);
    }

    public void EnterPatientId()
    {
        myId = int.Parse(patientIdInput.text);
        idDisplay.SetActive (false);
        questionDisplay.SetActive (true);
        isRoundActive = true;
    }

    public void EndRound()
    {
        isRoundActive = false;

        questionDisplay.SetActive (false);
        roundEndDisplay.SetActive (true);
        
    }

    public void ReturnToMenu()
    {
        //Save();
        saveIshiharaTest();
        UploadIshiharaTest();
        
        SceneManager.LoadScene("MenuScreen");
        
        
    }

   

    
}

[Serializable]
public class Questions
{
    public string[] cases;
}
[Serializable]
public class NumericData
{  
    public int playerScore, controlScore, redMildScore, redStrongScore, greenMildScore, greenStrongScore, totalBlindnessScore;
 
}
