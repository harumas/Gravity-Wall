using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleUnmaskTransitionSamplePlayFadeOutStart : MonoBehaviour
{
    public Animator anim;

    void Start(){
        Invoke("FadeOut", 3f);
        Invoke("FadeIn", 6f);
        Invoke("FadeOut", 9f);
        Invoke("FadeIn", 12f);
    }

    void FadeIn(){
        anim.SetBool("FadeIn", true);
        anim.SetBool("FadeOut", false);
    }
    void FadeOut(){
        anim.SetBool("FadeIn", false);
        anim.SetBool("FadeOut", true);
    }



}
