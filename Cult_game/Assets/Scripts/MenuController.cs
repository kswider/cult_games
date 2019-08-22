using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    private Transform _cameraTransform;
    private Vector3 _cameraDesiredPosition;
    private const float TransitionSpeed = 5f;
    private bool _cameraEventFired;
    private Coroutine _movingCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
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
