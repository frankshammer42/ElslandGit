using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingAnimationControl : MonoBehaviour
{

    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>(); 
    }

   public void startPanicking()
    {
        anim.SetTrigger("Panicking");
    }

    public void startFalling()
    {
        anim.SetTrigger("Falling");
    }
}
