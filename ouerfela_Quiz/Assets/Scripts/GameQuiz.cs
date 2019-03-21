using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameQuiz : MonoBehaviour
{
    public Text timeText;
    public Text scoreText;
    public Text questionText;
    public Text correctText;
    public Text wrongText;
    public VerticalLayoutGroup answers;
    public Button answerButton;

    public static int TimeRemaining;
    public static int Score;

    private int _questionsIndex;
    private bool _questionChanged;

    private readonly List<Question> _questions = new List<Question>
    {
        new Question(
            "What is the return type of constructors?",
            new List<Answer>(new[]
            {
                new Answer("int"), new Answer("float"), new Answer("struct"),
                new Answer("boolean"), new Answer("None of the mentioned", true),
            }), 100, -50),
        new Question(
            "The data member of a class by default are?",
            new List<Answer>(new[]
            {
                new Answer("protected,public"), new Answer("private,public"), new Answer("private", true),
                new Answer("public"),
            }), 100, -100),
        new Question(
            "Destroy(this) will:",
            new List<Answer>(new[]
            {
                new Answer("Destroy only the current script", true),
                new Answer("Destroy only the current GameObject"),
                new Answer("Destroy the current GameObject and its components")
            }), 100, -40),
        new Question(
            "Does C#.NET support partial implementation of interfaces?",
            new List<Answer>(new[]
            {
                new Answer("Yes"), new Answer("No", true)
            }), 100, -100),
    };

    private void Awake()
    {
        TimeRemaining = 30;
        Score = 0;
        _questionsIndex = 0;
        _questionChanged = true;
    }

    private void Update()
    {
        TimeRemaining = 30 - Mathf.RoundToInt(Time.timeSinceLevelLoad);
        timeText.text = $"Time: {TimeRemaining}";

        if (_questionsIndex >= _questions.Count || TimeRemaining <= 0)
        {
            SceneManager.LoadScene("EndScene");
            return;
        }
        if (!_questionChanged) return;

        scoreText.text = $"Score: {Score}/400";
        var question = _questions[_questionsIndex];
        questionText.text = question.QuestionText;
        correctText.text = $"Correct answer gets: {question.CorrectPoints}";
        wrongText.text = $"Wrong answer gets: {question.WrongPoints}";
        ReplaceAnswers(question);

        _questionChanged = false;
    }

    private void ReplaceAnswers(Question question)
    {
        foreach (Transform child in answers.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        question.Answers.ForEach(InstantiateAnswerButton);
    }

    private void InstantiateAnswerButton(Answer answer)
    {
        var answerObject = Instantiate(answerButton, answers.transform);
        var answerText = answerObject.GetComponentInChildren<Text>();
        answerText.text = answer.Text;
        answerObject.onClick.AddListener(() => OnAnswerClicked(answer));
    }

    private void OnAnswerClicked(Answer answer)
    {
        var question = _questions[_questionsIndex];
        Score += answer.IsCorrect ? question.CorrectPoints : question.WrongPoints;
        _questionsIndex++;
        _questionChanged = true;
    }
}