using System.Collections;
using System.Collections.Generic;
using Rokoko.Smartsuit;
using UnityEngine;

public class SmartsuitDataHandler : MonoBehaviour
{
    private Smartsuit smartsuit;
    
    
    // Start is called before the first frame update
    void Start()
    {
        smartsuit = FindObjectOfType<Smartsuit>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
