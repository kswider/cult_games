using UnityEngine;

public class PlayPanelController : MonoBehaviour
{
    private SceneController _sceneController;

    private void Start()
    {
        _sceneController = Utilities.FindSceneController();
    }

    public void GoToExploringView()
    {
        _sceneController.GoToScene(SceneController.SCN_EXPLORING_VIEW);
    }

    public void GoToPlacesWithType(string type)
    {
        _sceneController.GoToPlacesSceneWithType(type);
    }

    public void GoToMultiplayer()
    {
        _sceneController.GoToScene(SceneController.SCN_MULTIPLAYER);
    }
}
