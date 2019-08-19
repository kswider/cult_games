using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    private Transform cameraTransform;
    private Vector3 cameraDesiredPosition;
    private float transitionSpeed = 5f;
    private bool cameraEventFired = false;
    private bool isCameraMoving = false;
    private Coroutine movingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (cameraEventFired)
        {
            cameraEventFired = false;
            movingCoroutine = StartCoroutine(Lerp());            
        }

        // Make sure user is on Android platform
        if (Application.platform == RuntimePlatform.Android) {
        
            // Check if Back was pressed this frame
            if (Input.GetKeyDown(KeyCode.Escape)) {
            
                // Quit the application
                Application.Quit();
            }
        }
    }

    public void LookAtMenu(Transform menuTransform)
    {
        if (movingCoroutine != null)
        {
            StopCoroutine(movingCoroutine);
        }            
        cameraDesiredPosition = cameraTransform.position;
        cameraDesiredPosition.x = menuTransform.position.x;
        cameraEventFired = true;
    }  

    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }
 
    private IEnumerator Lerp()
    {
        while (true)
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraDesiredPosition, transitionSpeed * Time.deltaTime);

            // We are at the position, stop this IEnumerator
            if (Mathf.Approximately(cameraTransform.position.x, cameraDesiredPosition.x))
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
