using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    public Text text;
    private Slider m_Slider;//Slider∂‘œÛ

    // Start is called before the first frame update
    void Start()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
    }

    private void ValueChangeCheck()
    {
        text.text = m_Slider.value.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
