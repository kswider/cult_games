using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizController : MonoBehaviour
{
    QuizParametersHolder parameters;

    // Start is called before the first frame update
    void Start()
    {
        parameters = GameObject.FindObjectOfType<QuizParametersHolder>();
        Debug.LogError($"Passed parameter = {parameters?.QuizNumber ?? 100}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
