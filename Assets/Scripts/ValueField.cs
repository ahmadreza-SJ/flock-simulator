using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueField : MonoBehaviour
{
    public Text ValueText;

    public void SetValue(float? value)
    {
        ValueText.text = value.ToString();
    }
}
