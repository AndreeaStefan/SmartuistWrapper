using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private bool display;
    private string text;
    private static Text _text;


    void Start()
    {
        _text = FindObjectOfType<Text>();
        _text.text = "";
    }

    public static void startDisplay(string text)
    {
        _text.text = text;
    }

    public static void stopDisplaying()
    {
        _text.text = "";
    }

}
