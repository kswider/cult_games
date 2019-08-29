using UnityEngine;

public class InspirationalLearningSceneController : MonoBehaviour
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
