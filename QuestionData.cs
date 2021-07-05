using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class QuestionData 
{
    public string questionText;
    public UnityEngine.Sprite numberPlate;
    public AnswerData[] answers;
}