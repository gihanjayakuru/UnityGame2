using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance { set; get; }

    public Material playerMetirial;
    public Color[] playerColors = new Color[10];
    public GameObject[] playerTrails = new GameObject[10];

    public int currentLevel = 0; //used when changing from menu to game scene
    public int menuFocus = 0 ;    //used when entering the menu scene to know which menu foacus

    private Dictionary<int, Vector2> activeTouches = new Dictionary<int, Vector2>();

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;

    }
    


    public Vector3 GetPlayerInput()
    {
        //are we using the accelerometer?
        if(SaveManager.Instance.state.usingAccelerometer)
        {
            //if we can us it replace Y param by Z 
            Vector3 a = Input.acceleration;
            a.y = a.z;
            return a;
        }


        //read all touches from the user
        Vector3 r = Vector3.zero;
        foreach(Touch touch in Input.touches)
        {
            //if we just start pressing on the screen
            if(touch.phase == TouchPhase.Began)
            {
                activeTouches.Add(touch.fingerId, touch.position);

            }
            //if we remove our finger off the screen
            else if(touch.phase == TouchPhase.Ended)
            {
                if(activeTouches.ContainsKey(touch.fingerId))
                    activeTouches.Remove(touch.fingerId);
            }
            // our finger is either moving or stationary in both cases lets use the delta
            else
            {
                float mag = 0;
                r = (touch.position - activeTouches[touch.fingerId]);
                mag = r.magnitude / 300;
                r = r.normalized * mag;

                if(r.magnitude > 1)
                {
                    r.Normalize();
                }

                
            }

        }

        return r;
    }


}
