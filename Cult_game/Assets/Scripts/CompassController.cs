using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class CompassController : MonoBehaviour
{
    public void Awake()
    {
        StartCoroutine(InitializeLocation());
    }
    public IEnumerator InitializeLocation()
    {
#if UNITY_EDITOR
        //Wait until Unity connects to the Unity Remote, while not connected, yield return null
        while (!UnityEditor.EditorApplication.isRemoteConnected)
        {
            yield return null;
        }
#endif
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("location disabled by user");
            yield break;
        }
        // enable compass
        Input.compass.enabled = true;
        // start the location service
        Debug.Log("start location service");
        Input.location.Start(10, 0.01f);
        // Wait until service initializes
        int maxSecondsToWaitForLocation = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxSecondsToWaitForLocation > 0)
        {
            yield return new WaitForSeconds(1);
            maxSecondsToWaitForLocation--;
        }
     
        // Service didn't initialize in 20 seconds
        if (maxSecondsToWaitForLocation < 1)
        {
            Debug.Log("location service timeout");
            yield break;
        }
        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("unable to determine device location");
            yield break;
        }
        Debug.Log("location service loaded");
    }
    
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdateCompass", 0.5f, 0.025f);
    }

    // Update is called once per frame
    void Update () 
    {

    }

    void UpdateCompass()
    {
        transform.localRotation = Quaternion.Euler(0, 0, Mathf.Round(Input.compass.trueHeading));
    }
}
