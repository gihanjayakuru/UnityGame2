using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class preloader : MonoBehaviour
{
    
    private CanvasGroup fadeGroup ;

    private float loadTime;
    private float minimumLogoTime = 3.0f; //logo time

    private void Start(){

        //Grab canvas group
        fadeGroup = FindObjectOfType<CanvasGroup>();

        //start with white
        fadeGroup.alpha = 1;

        //preload game
        //$$

        //get time of the completion
        //if loadtime is

        if(Time.time < minimumLogoTime){
            loadTime = minimumLogoTime;
        }
        else{
            loadTime = Time.time;
        }

    }

    private void Update(){

        //fade in
        if(Time.time < minimumLogoTime){
            fadeGroup.alpha = 1- Time.time;

        }

        //fade out
        if(Time.time > minimumLogoTime && loadTime != 0)
        {
            fadeGroup.alpha = Time.time - minimumLogoTime;
            if(fadeGroup.alpha >= 1)
            {
                SceneManager.LoadScene("Menu");
            }

        }

    }

}
