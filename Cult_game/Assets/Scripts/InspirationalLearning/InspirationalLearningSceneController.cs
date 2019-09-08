using System;
using UnityEngine;

public class InspirationalLearningSceneController : MonoBehaviour
{
    private SceneController _sceneController;
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = Utilities.FindPlayerController();
        _sceneController = Utilities.FindSceneController();
    }

    public void GoBack()
    {
        _sceneController.GoBack();
    }

    public void PlayAgain()
    {
        string gameType = _playerController.Places.Find(p => p.id == _playerController.CurrentPlayedPlaceId).gameType;

        switch (gameType)
        {
            case "Quiz":
                _sceneController.GoToScene(SceneController.SCN_QUIZ_LEARNING);
                break;
            case "Puzzle":
                _sceneController.GoToScene(SceneController.SCN_PUZZLE_GAME);
                break;
            case "Action":
                _sceneController.GoToScene(SceneController.SCN_ACTION_GAME);
                break;
            default:
                throw new NotImplementedException();
        }
    }
}
