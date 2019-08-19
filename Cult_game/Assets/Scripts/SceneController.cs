using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform != RuntimePlatform.Android) return;
        // Check if Back was pressed this frame
        if (Input.GetKeyDown(KeyCode.Escape)) {
            
            // Quit the application
            ExitGame();
        }
    }
    
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
