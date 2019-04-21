using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderBlink : MonoBehaviour
{
    Renderer stimuli;

    void Start()
    {
        stimuli = GetComponent<Renderer>();
    }

    void Update()
    {
        //stimuli.material.SetFloat("_BlinkRate", 1f); // change BlinkRate from this script
    }

    public void turnOffBlink()
    {
        stimuli.material.SetFloat("_BlinkRate", 0f);
    }

    public void turnOnBlink()
    {
        stimuli.material.SetFloat("_BlinkRate", 10f);
    }

}
