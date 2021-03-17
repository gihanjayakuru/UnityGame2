using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { set; get;}
    public SaveState state;

    private void Awake()
    {
        
  
        DontDestroyOnLoad(gameObject);
        Instance = this;
        Load();

        //are we using the accelerometer and can we use it
        if(state.usingAccelerometer && !SystemInfo.supportsAccelerometer)
        {
            //if we cant make sure weare not trying next time
            state.usingAccelerometer = false;
            Save();
        }

    }

    //save the state of this savescript to rthe player pref
    public void Save()
    {
       PlayerPrefs.SetString("save",Helper.Encrypt(Helper.Serialize<SaveState>(state)));
    }

    public void Load()
    {
        //Do we already have save?
        if(PlayerPrefs.HasKey("save"))
        {
             state= Helper.Deserialize<SaveState>(Helper.Decrypt(PlayerPrefs.GetString("save"))); /* DESERIALIZED OUR CLASS*/

        }
            else
            {
                state = new SaveState();
                Save();
                Debug.Log("No save file found creatinfg new one");
            }
    }

    //check color is owned
    public bool IsColorOwned(int index)
    {
        //check if the bit is set,if so the color is owned 
        return(state.colorOwned & (1 << index )) != 0;
    }

    //check trail is owned
    public bool IsTrailOwned(int index)
    {
        //check if the bit is set,is that trail is owned 
        return(state.trailOwned & (1 << index )) != 0;
    }

    //attemp to buy color
    public bool BuyColor(int index,int cost)
    {
        if(state.gold >= cost)
        {
            //enough money remove from current gold stack
            state.gold -= cost;
            UnlockColor(index);

            //save prgress
            Save();

            return true;
        }
        else
        {
            //not enough money retur false
            return false;
        }
    }

       //attemp to buy trail
    public bool BuyTrail(int index,int cost)
    {
        if(state.gold >= cost)
        {
            //enough money remove from current gold stack
            state.gold -= cost;
            UnlockTrail(index);

            //save prgress
            Save();

            return true;
        }
        else
        {
            //not enough money retur false
            return false;
        }
    }


    //unlock color in "colorowned"
    public void UnlockColor(int index)
    {
        //toggle on bit
        state.colorOwned |= 1 << index;
    }


    //unlock a trail  in the "trailowned" int
    public void UnlockTrail(int index)
    {
        //toggle on bit
        state.trailOwned |= 1 << index;
    }


    //Complete level
    public void CompleteLevel(int index)
    {
        //if this this current active level
        if(state.completedLevel == index)
        {
            state.completedLevel++;
            Save();
        }

    }
    

    //reset the save file
    public void ResetSave()
    {
        PlayerPrefs.DeleteKey("save");
    }

}
