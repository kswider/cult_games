﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using System.Linq;

public class CompassController : MonoBehaviour
{
    public int smoothingProbes = 15;
    
    private List<float> _headings;

    private void Awake()
    {
        StartCoroutine(InitializeLocation());
    }

    // Start is called before the first frame update
    private void Start()
    {
        _headings = new List<float>();
        InvokeRepeating(nameof(UpdateCompass), 0.5f, 0.025f);
    }
    
    private IEnumerator InitializeLocation()
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

    private void UpdateCompass()
    {
        if (_headings.Count > smoothingProbes)
        {
            _headings.RemoveAt(0);
        }
        _headings.Add(Input.compass.trueHeading);
        
        transform.localRotation = Quaternion.Euler(0, 0, _headings.Average());
    }
}
