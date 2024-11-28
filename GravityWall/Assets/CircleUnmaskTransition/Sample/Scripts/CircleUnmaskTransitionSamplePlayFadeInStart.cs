using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleUnmaskTransitionSamplePlayFadeInStart : MonoBehaviour
{
    public Animator anim;

    void Start(){
        Invoke("FadeIn", 3f);
        Invoke("FadeOut", 6f);
        Invoke("FadeIn", 9f);
        Invoke("FadeOut", 12f);

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
