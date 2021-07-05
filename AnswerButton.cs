using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AnswerButton : MonoBehaviour {

    public Text answerText;

    private AnswerData answerData;
    private GameController gameController;
    

    // Use this for initialization
    void Start () 
    {
        gameController = FindObjectOfType<GameController> ();
    }

    public void Setup(AnswerData data)
    {
        answerData = data;
        answerText.text = answerData.answerText;
    }


    public void HandleClick()
    {
        gameController.ControlButtonClicked (answerData.isControl);
        gameController.MildRButtonClicked (answerData.isMildR);
        gameController.StrongRButtonClicked (answerData.isStrongR);
        gameController.MildGButtonClicked (answerData.isMildG);
        gameController.StrongGButtonClicked (answerData.isStrongG);
        gameController.SeverityButtonClicked (answerData.isSeverity);
        gameController.AnswerButtonClicked (answerData.isCorrect);
        
    }
}