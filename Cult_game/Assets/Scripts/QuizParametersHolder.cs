using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizParametersHolder : MonoBehaviour
{

    public int QuizNumber = 5;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
