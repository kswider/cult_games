﻿using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Vector3 _cameraDesiredPosition;
    private const float TransitionSpeed = 5f;
    private bool _cameraEventFired;
    private Coroutine _movingCoroutine;

    private SceneController _sceneController;
    void Start()
    {
        _cameraTransform = Camera.main.transform;

        _sceneController = Utilities.FindSceneController();
    }
    
    void Update()
    {
        if (!_cameraEventFired) return;
        _cameraEventFired = false;
        _movingCoroutine = StartCoroutine(Lerp());
    }

    public void LookAtMenu(Transform menuTransform)
    {
        if (_movingCoroutine != null)
        {
            StopCoroutine(_movingCoroutine);
        }            
        _cameraDesiredPosition = _cameraTransform.position;
        _cameraDesiredPosition.x = menuTransform.position.x;
        _cameraEventFired = true;
    }
    
    public void ExitGame()
    {
        _sceneController.ExitGame();
    }

    private IEnumerator Lerp()
    {
        while (true)
        {
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _cameraDesiredPosition, TransitionSpeed * Time.deltaTime);

            // We are at the position, stop this IEnumerator
            if (Mathf.Approximately(_cameraTransform.position.x, _cameraDesiredPosition.x))
            {
                break;
            }

            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }


}
