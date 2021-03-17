using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameScene : MonoBehaviour
{
    private CanvasGroup fadeGroup;
    private float fadeInDuration = 2;
    private bool gameStarted;

    public Transform arrow;
    private Transform playerTransform;
    public Objective objective;


    private void Start()
    {
        // find the player transform
        playerTransform = FindObjectOfType<PlayerMotor>().transform;

        //load up the level
        SceneManager.LoadScene(Manager.Instance.currentLevel.ToString(),LoadSceneMode.Additive);

        //get the only canvasGroup in the scene
        fadeGroup = FindObjectOfType<CanvasGroup>();
        //Set the fade to full opacity
        fadeGroup.alpha = 1;
    }

    private void Update()
    {
        if(objective != null)
        {
            //if we have an objective

            //rotate the arrow
            Vector3 dir = playerTransform.InverseTransformPoint(objective.GetCurrentRing().position);
            float a = Mathf.Atan2(dir.x,dir.z) * Mathf.Rad2Deg;
            a += 180;
            arrow.transform.localEulerAngles = new Vector3(0,180,a);//180 becuse its is down arrow
        }

        if(Time.timeSinceLevelLoad <= fadeInDuration)
        {
        //intial fade in
        fadeGroup.alpha = 1 - (Time.timeSinceLevelLoad / fadeInDuration);
        }
         // if the initial fade in is completed and the game has not been start
        else if(!gameStarted)
        {
            //ensure the fade is complete gone
            fadeGroup.alpha = 0;
            gameStarted = true;
        }
    }


    public void CompleteLevel()
    {
        //complete the level and save progress
        SaveManager.Instance.CompleteLevel(Manager.Instance.currentLevel);

        //focaus the level selection when we return the menu
        Manager.Instance.menuFocus = 1;

        ExitScene();
    }

    public void ExitScene()
    {
        SceneManager.LoadScene("Menu");
        
    }
}
