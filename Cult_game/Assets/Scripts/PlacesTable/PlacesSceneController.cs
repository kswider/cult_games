using UnityEngine;

public class PlacesSceneController : MonoBehaviour
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
}
