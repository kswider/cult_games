using UnityEngine;

public class ExploringViewSceneController : MonoBehaviour
{
    private SceneController _sceneController;

    private void Start()
    {
        _sceneController = Utilities.FindSceneController();
    }

    public void GoBack()
    {
        _sceneController.GoBack();
    }

    public void ChooseAttraction()
    {
        _sceneController.GoToScene(SceneController.SCN_PLACES);
    }
}
