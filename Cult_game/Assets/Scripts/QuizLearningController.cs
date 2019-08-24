﻿using ResourcesObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizLearningController : MonoBehaviour
{
    public Text learningText;
    public Scrollbar scrollbar;

    private PlayerController _playerController;
    private Quiz _currentQuiz;

    // Start is called before the first frame update
    void Start()
    {
        _playerController = Utilities.FindPlayer();
        _currentQuiz = Resources.LoadAll<Quiz>("Quizes").First(x => x.id == _playerController.CurrentQuizId);
        learningText.text = _currentQuiz.learningText;
        scrollbar.value = 1;
    }
}
