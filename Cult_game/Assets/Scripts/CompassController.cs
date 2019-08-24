using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CompassController : MonoBehaviour
{
    public int smoothingProbes = 15;
    
    private List<float> _headings = new List<float>();
    
    private void Start()
    {
        InvokeRepeating(nameof(UpdateCompass), 0.5f, 0.025f);
    }

    private void UpdateCompass()
    {
        EnqueueHeading();
        RotateCompass();
    }

    private void EnqueueHeading()
    {
        if (_headings.Count > smoothingProbes)
        {
            _headings.RemoveAt(0);
        }
        _headings.Add(Input.compass.trueHeading);
    }

    private void RotateCompass()
    {
        transform.localRotation = Quaternion.Euler(0, 0, _headings.Average());
    }
}
