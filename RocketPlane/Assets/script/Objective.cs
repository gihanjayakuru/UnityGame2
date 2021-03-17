using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Objective : MonoBehaviour
{
    private List<Transform> rings= new List<Transform>();

    public Material activeRing;
    public Material inactiveRing;
    public Material finalRing;

    private int ringPassed = 0;

    private void Start()
    {
        //set the objective field in the game scene
        FindObjectOfType<GameScene>().objective = this;


        //at the start of the level assign inactive to all rings
        foreach(Transform t in transform)
        {
            rings.Add(t);
            t.GetComponent<MeshRenderer>().material = inactiveRing;
        }

        //making sure we 
        if(rings.Count == 0)
        {
            Debug.Log("There is no objective");
            return;
        }

        //activate th first ring
        rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;
        rings[ringPassed].GetComponent<Ring>().ActivateRing();
    }

    public void NextRing()
    {
        //play FX on the currrent ring
        rings[ringPassed].GetComponent<Animator>().SetTrigger("collectionTrigger");
        

        //up the int
        ringPassed++;

        //if it is the final ring lets call the victory
        if(ringPassed == rings.Count)
        {
           Victory();
            return;
        }

        //if this is the previous lastt give the next ring the "final ring " material
        if(ringPassed == rings.Count - 1)
            rings[ringPassed].GetComponent<MeshRenderer>().material = finalRing;
        
        else
            rings[ringPassed].GetComponent<MeshRenderer>().material = activeRing;

            //in both cases we need to active the ring
            rings[ringPassed].GetComponent<Ring>().ActivateRing();
    }

    public Transform GetCurrentRing()
    {
        return rings[ringPassed];
    }
    private void Victory()
    {
        FindObjectOfType<GameScene>().CompleteLevel();
    }
}
