using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourcesObjects
{
    [CreateAssetMenu]
    public class Quiz : ScriptableObject
    {

        public int id;
        public string learningText;
        public int points;
        public List<QuestionWithPossibleAnswers> questions;
    }

    [Serializable]
    public class QuestionWithPossibleAnswers
    {
        public string questionText;
        public Answer[] possibleAnswers;
    }

    [Serializable]
    public class Answer
    {
        public string answerText;
        public bool isCorrect;
    }
}
