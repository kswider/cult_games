using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
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
