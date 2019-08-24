using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = Utilities.FindPlayer();
    }

    // Update is called once per frame
    private void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform != RuntimePlatform.Android) return;
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            // Quit the application
            ExitGame();
        }
    }

    public void GoToPlacesSceneWithType(string type)
    {
        _playerController.Settings.LookedType = type;
        _playerController.Settings.SaveSettings();
        GoToScene("SCN_PLACES");
    }
    
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); 
    }

    public void ExitGame()
    {
        _playerController.SaveGame();
        _playerController.Settings.SaveSettings();
        Application.Quit();
    }
}
