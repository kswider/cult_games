using ResourcesObjects;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public Text questionText;
    public GameObject buttonsHolder;

    private PlayerController _playerController;
    private Quiz _currentQuiz;
    private Button[] _buttons;
    private Button _correctAnswerButton;
    private int _correctlyAnsweredQuestionsNumber;
    private bool _isButtonClicked;
    private int _currentQuestionNumber;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerController = GameObject.FindObjectOfType<PlayerController>();
        _currentQuiz = Resources.LoadAll<Quiz>("Quizes").First(x => x.id == _playerController.CurrentQuizId);
        _buttons = buttonsHolder.GetComponentsInChildren<Button>();
        LoadNextQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isButtonClicked)
        {
            _isButtonClicked = false;
            StartCoroutine(PrepareNextAnswer());
        }
    }

    private IEnumerator PrepareNextAnswer()
    {
        ChangeButtonState(false);
        yield return new WaitForSeconds(5);
        _currentQuestionNumber++;
        if (_currentQuestionNumber < _currentQuiz.questions.Count)
        {
            LoadNextQuestion();
            ChangeButtonState(true);
        }
        else
        {
            Debug.LogError("Koniec pytan!");
        }
    }

    private void LoadNextQuestion()
    {
        var currentQuestion = _currentQuiz.questions[_currentQuestionNumber];
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < _buttons.Length; i++)
        {
            var buttonText = _buttons[i].GetComponentInChildren<Text>();
            var answer = currentQuestion.possibleAnswers[i];
            buttonText.text = answer.answerText;

            int localCounter = i;
            if (answer.isCorrect)
            {
                _buttons[localCounter].onClick.AddListener(delegate { CorrectAnswerAction(_buttons[localCounter]); });
                _correctAnswerButton = _buttons[i];
            }
            else
            {
                _buttons[localCounter].onClick.AddListener(delegate { WrongAnswerAction(_buttons[localCounter]); });
            }
        }
    }

    private void ChangeButtonState(bool isEnabled)
    {
        foreach(var button in _buttons)
        {
            button.enabled = isEnabled;
        }
    }

    private void CorrectAnswerAction(Button button)
    {
        _correctlyAnsweredQuestionsNumber++;
        var buttonImage = button.GetComponentInChildren<Image>();
        buttonImage.color = Color.green;
        _isButtonClicked = true;
    }

    private void WrongAnswerAction(Button button)
    {
        var buttonImage = button.GetComponentInChildren<Image>();
        buttonImage.color = Color.red;
        _isButtonClicked = true;
        var correctButtonImage = _correctAnswerButton.GetComponentInChildren<Image>();
        correctButtonImage.color = Color.green;
    }
}
