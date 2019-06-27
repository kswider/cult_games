using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnClickController : MonoBehaviour
{
    public string ButtonType;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android) {
        
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape)) {
            
                // Quit the application
                Application.Quit();
            }
        }
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }
    
}
