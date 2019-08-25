using UnityEngine;
using UnityEngine.UI;

public class HandleController : MonoBehaviour
{
    private Text _text;

    public Slider slider;
    private void Start()
    {
        _text = GetComponent<Text>();
        setValue(slider.value);
    }
    
    
    public void setValue(float value)
    {
        int trueValue = (int) value * 25 + 50;
        _text.text = trueValue.ToString();
    }
}
