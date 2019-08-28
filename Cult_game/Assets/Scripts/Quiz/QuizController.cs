using ResourcesObjects;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class QuizController : MonoBehaviour
{
    public Text questionText;
    public Text remainingTimeText;
    public GameObject buttonsHolder;

    private PlayerController _playerController;
    private SceneController _sceneController;
    private Quiz _currentQuiz;
    private Button[] _buttons;
    private Button _correctAnswerButton;
    private int _correctlyAnsweredQuestionsNumber = 0;
    private bool _isButtonClicked;
    private int _currentQuestionNumber;
    private Coroutine _timerCoroutine;

    private const int TimeForAnswer = 10;
    
    // Start is called before the first frame update
    void Start()
    {
        _playerController = Utilities.FindPlayer();
        _sceneController = Utilities.FindSceneController();
        _currentQuiz = Resources.LoadAll<Quiz>("Quizes").First(x => x.id == _playerController.CurrentPlayedGameId);
        _buttons = buttonsHolder.GetComponentsInChildren<Button>();
        LoadNextQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isButtonClicked) return;
        
        _isButtonClicked = false;
        StartCoroutine(AnswerWasClicked());
    }

    private IEnumerator AnswerWasClicked()
    {
        StopCoroutine(_timerCoroutine);
        yield return PrepareForNextQuestion();
    }

    private IEnumerator PrepareForNextQuestion()
    {
        ChangeButtonState(false);
        _currentQuestionNumber++;
        if (_currentQuestionNumber < _currentQuiz.questions.Count)
        {
            yield return new WaitForSeconds(5);
            LoadNextQuestion();
            ResetColorOfButtons();
            ChangeButtonState(true);
        }
        else
        {
            yield return new WaitForSeconds(2);
            if (_correctlyAnsweredQuestionsNumber == _currentQuiz.questions.Count)
            {
                _playerController.AddPoints(_currentQuiz.points);
                questionText.text = "Quiz was passed successfully!";
                yield return new WaitForSeconds(3);
                _sceneController.GoToScene(SceneController.SCN_INSPIRATIONAL_LEARNING);
            }
            else
            {
                var pb = new Save.PlaceBlock { placeId = _playerController.CurrentPlayedPlaceId, blockUntil = DateTime.Now.AddSeconds(300) };
                _playerController.BlockedPlaces.Add(pb);
                questionText.text = $"Unfortunately you have not passed the quiz successfully. Number of correct answers: {_correctlyAnsweredQuestionsNumber}/{_currentQuiz.questions.Count}";
                yield return new WaitForSeconds(5);
                _sceneController.GoToScene(SceneController.SCN_EXPLORING_VIEW);
            }
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
                _buttons[localCounter].onClick.RemoveAllListeners();
                _buttons[localCounter].onClick.AddListener(delegate { CorrectAnswerAction(_buttons[localCounter]); });
                _correctAnswerButton = _buttons[i];
            }
            else
            {
                _buttons[localCounter].onClick.RemoveAllListeners();
                _buttons[localCounter].onClick.AddListener(delegate { WrongAnswerAction(_buttons[localCounter]); });
            }
        }
        _timerCoroutine = StartCoroutine(StartAnswerTimer());
    }

    private IEnumerator StartAnswerTimer()
    {
        for (int i = TimeForAnswer; i > 0; i--)
        {
            remainingTimeText.text = $"Remaining time: {i}s";
            yield return new WaitForSeconds(1);
        }
        remainingTimeText.text = $"Time has expired!";
        NoAnswerAction();
        yield return PrepareForNextQuestion();
    }

    private void ResetColorOfButtons()
    {
        foreach (var button in _buttons)
        {
            button.image.color = Color.white;
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

    private void NoAnswerAction()
    {
        var correctButtonImage = _correctAnswerButton.GetComponentInChildren<Image>();
        correctButtonImage.color = Color.green;
    }
}
