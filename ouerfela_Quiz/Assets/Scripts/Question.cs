using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question
{
    public readonly string QuestionText;
    public readonly List<Answer> Answers;
    public readonly int CorrectPoints;
    public readonly int WrongPoints;

    public Question(string questionText, List<Answer> answers, int correctPoints, int wrongPoints)
    {
        this.QuestionText = questionText;
        this.Answers = answers;
        this.CorrectPoints = correctPoints;
        this.WrongPoints = wrongPoints;
    }
}
