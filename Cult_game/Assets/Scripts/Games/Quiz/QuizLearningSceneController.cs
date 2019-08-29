using UnityEngine;

public class QuizLearningSceneController : MonoBehaviour
{
    private SceneController _sceneController;

    private void Start()
    {
        _sceneController = Utilities.FindSceneController();
    }

    public void StartQuiz()
    {
        _sceneController.GoToScene(SceneController.SCN_QUIZ);
    }

    public void ExitQuiz()
    {
        _sceneController.GoBack();
    }
}
