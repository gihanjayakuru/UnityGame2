using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MenuScene : MonoBehaviour
{
   
   private CanvasGroup fadeGroup;
   private float faddeInSpeed = 0.33f;

   public RectTransform menuContainer;
   public Transform levelPanel;
   public Transform colorPanel;
   public Transform trailPanel;

   public Button tiltControlButton;
   public Color tiltControlEnabled;
   public Color tiltControlDisabled;

    public Text colorBuySetText;
    public Text trailBuySetText;
    public Text goldText;

    private MenuCamera menuCam;

    private int[] colorCost = new int[] {0,5,5,5,10,10,10,15,15,10};
    private int[] trailCost = new int[] {0,20,40,40,60,80,80,100,100,150};
    private int selectedColorIndex;
    private int selectedTrailIndex;
    private int activeColorIndex;
    private int activeTrailIndex;

   private Vector3 desiredMenuPosition;
   private GameObject currentTrail;

   public AnimationCurve enteringLevelZoomCurve;
   private bool isEnteringLevel = false;
   private float zoomDuration = 3.0f;

   private float zoomTransition;

   private Texture previousTrail;
   private GameObject lastPreviewObject;

   public Transform trailPreviewObject;
   public RenderTexture trailPreviewTexture;


   private void Start()
   {

        //check if we have an accelerometer
        if(SystemInfo.supportsAccelerometer)
        {
            //is it currently enabled?
            tiltControlButton.GetComponent<Image>().color = (SaveManager.Instance.state.usingAccelerometer) ? tiltControlEnabled : tiltControlDisabled;
        }
        else
        {
            tiltControlButton.gameObject.SetActive(false);
        }

       //fnd the only camera and asign it
       menuCam = FindObjectOfType<MenuCamera>();

       

       //position camera on the focus menu
       SetCameraTo(Manager.Instance.menuFocus);
    

       //tell gold text how much he should displaying
       UpdateGoldText();

       //Grab canvas group
        fadeGroup = FindObjectOfType<CanvasGroup>();

        //start with white
        fadeGroup.alpha = 1;

        //add button onclick event to shop
        InitShop();

        //Add button on cllick event to level
        InitLevel();

        //set player preferences(color &trail)
        OnColorSelect(SaveManager.Instance.state.activeColor);
        SetColor(SaveManager.Instance.state.activeColor);

        OnTrailSelect(SaveManager.Instance.state.activeTrail);
        SetTrail(SaveManager.Instance.state.activeTrail);

        //make the button bigger for selected items
        colorPanel.GetChild(SaveManager.Instance.state.activeColor).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        
        //creat the trail preview
        lastPreviewObject = GameObject.Instantiate(Manager.Instance.playerTrails[SaveManager.Instance.state.activeTrail]) as GameObject;
        lastPreviewObject.transform.SetParent(trailPreviewObject);
        lastPreviewObject.transform.localPosition = Vector3.zero;

   }

    private void Update()
    {
        //fade in
        fadeGroup.alpha = 1 - Time.timeSinceLevelLoad * faddeInSpeed;

        // Menu Navigation(smooth)
        menuContainer.anchoredPosition3D = Vector3.Lerp(menuContainer.anchoredPosition3D,desiredMenuPosition,0.1f);


        //entering level zoom
        if(isEnteringLevel)
        {
            //add to the zoomTransition float
            zoomTransition += (1/ zoomDuration) * Time.deltaTime;

            menuContainer.localScale = Vector3.Lerp(Vector3.one,Vector3.one * 5, enteringLevelZoomCurve.Evaluate(zoomTransition));
        
            //change the desired position of the canvas  so it can folllow the
            //this zoom in the center level button
            Vector3 newDesiredPosition = desiredMenuPosition * 5;
            //this adds to specific position level button on canvas
            RectTransform rt = levelPanel.GetChild(Manager.Instance.currentLevel).GetComponent<RectTransform>();
            newDesiredPosition -= rt.anchoredPosition3D * 5;

            //this line will overide the previous position update
            menuContainer.anchoredPosition3D = Vector3.Lerp(desiredMenuPosition,newDesiredPosition, enteringLevelZoomCurve.Evaluate(zoomTransition));

            //fade to white screen 
            fadeGroup.alpha = zoomTransition;

            //are we done with animation
            if(zoomTransition >= 1)
            {
                //enter the level
                SceneManager.LoadScene("Game");
            }
        }

    }

    private void  InitShop()
    {
        //assaigned the reference
        if(colorPanel == null || trailPanel == null)
            Debug.Log("you did not asign the color/trail panel inspector");

        //for every children transform under our color panel, find the button and add onclick
        int i = 0;
        foreach(Transform t in colorPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnColorSelect(currentIndex));

            //set the color of the image based on if owned or not
            Image img = t.GetComponent<Image>();
            img.color = SaveManager.Instance.IsColorOwned(i) 
                 ? Manager.Instance.playerColors[currentIndex] 
                 : Color.Lerp(Manager.Instance.playerColors[currentIndex],new Color(0,0,0,1),0.25f);   


            i++;

        }

        // reset index
        i = 0;
        foreach(Transform t in trailPanel)
        {
            Debug.Log("bbbbbbbbbbbbr");
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnTrailSelect(currentIndex));

            //set the color of the image based on if owned or not
            RawImage img = t.GetComponent<RawImage>();
            img.color = SaveManager.Instance.IsTrailOwned(i) ? Color.white : new Color(0.7f,0.7f,0.7f);


            i++;

        }
        //Set the previous trail to prevent bug when swaping later
        previousTrail = trailPanel.GetChild(SaveManager.Instance.state.activeTrail).GetComponent<RawImage>().texture;

    }

    private void InitLevel()
    {
        //make sure assaigned the reference
        if(levelPanel == null)
            Debug.Log("you did not asign level panel in the inspector");

        //for every children transform under our level panel, find the button and add onclick
        int i = 0;
        foreach(Transform t in levelPanel)
        {
            int currentIndex = i;
            Button b = t.GetComponent<Button>();
            b.onClick.AddListener(() => OnLevelSelect(currentIndex));

            Image img =t.GetComponent<Image>();

            //is it Unlock?
            if(i <= SaveManager.Instance.state.completedLevel)
            {
                //it is unlocked
                if(i == SaveManager.Instance.state.completedLevel)
                {
                    //it is  not completed
                    img.color = Color.white;
                }
                else
                {
                    //Level is already completed
                    img.color =Color.green;
                }
            }
            else
            {
                //level isnot unloack s=dsable button
                b.interactable = false;
                //set to a dark color
                img.color = Color.grey;
            }

            i++;

        }

    }

    private void SetCameraTo(int menuIndex)
    {
        NavigateTo(menuIndex);

        menuContainer.anchoredPosition3D = desiredMenuPosition;
  
    }

    private void NavigateTo(int menuIndex)
    {
        switch(menuIndex)
        {
            //0 && deafault case = Main Menu
            default:

            case 0:
                desiredMenuPosition = Vector3.zero;
                menuCam.BackToMainMenu();
                break;
            // 1 =play Menu
            case 1:
                desiredMenuPosition = Vector3.right * 1280;
                menuCam.MoveToLevel();
                break;
            // 2 = shop Menu
            case 2:
                desiredMenuPosition = Vector3.left * 1280;
                menuCam.MoveToShop();
                break;
        }
    }


    private void SetColor(int index)
    {
        //set active  index of clor
        activeColorIndex = index;
        SaveManager.Instance.state.activeColor = index;
       
        //change the color on player model
        Manager.Instance.playerMetirial.color = Manager.Instance.playerColors[index];

        //change buy set button text
        colorBuySetText.text = "Current";

        //remember preferences
        SaveManager.Instance.Save();
    }

    private void SetTrail(int index)
    {
        //set active index of trail
        activeTrailIndex = index; 
        SaveManager.Instance.state.activeTrail = index;

        //change the trail on player model
        if(currentTrail != null)
            Destroy(currentTrail);
        
        //creat new trail
        currentTrail = Instantiate(Manager.Instance.playerTrails[index]) as GameObject;

        //Set it as  a children of the player
        currentTrail.transform.SetParent(FindObjectOfType<MenuPlayer>().transform);

        //Fix the wierd scalling issues /rotation issues   #######bbbbbbbbbbbbbbbbbbbbbb
        //currentTrail.transform.localPosition = Vector3.zero;
       // currentTrail.transform.localRotation = Quaternion.Euler(0,0,90);
        //currentTrail.transform.localScale = Vector3.one * 0.01f;

        //change buy set button text
        trailBuySetText.text = "Current";

        //remember preferences
        SaveManager.Instance.Save();

    }


    private void UpdateGoldText()
    {
        goldText.text = SaveManager.Instance.state.gold.ToString();
    }

    //button
    public void OnPlayClick()
    {
        NavigateTo(1);
        Debug.Log("play click");

    }

    public void OnShopClick()
    {
        NavigateTo(2);
        Debug.Log("shop click");
    }

    public void OnBackClick()
    {
        NavigateTo(0);
        Debug.Log("Back button has been clicked");
    }

    private void OnColorSelect(int currentIndex)
    {
       Debug.Log("Selecting color button : "+ currentIndex);

       //if the button  clicked is already selected exit do nothing
       if(selectedColorIndex == currentIndex)
        return;

        //make panel get bigger selected
        colorPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        // put the  one on normal   scale
        colorPanel.GetChild(selectedColorIndex).GetComponent<RectTransform>().localScale = Vector3.one;
        

       //set selected color
       selectedColorIndex = currentIndex;

       //change the content of the buy set button
       if(SaveManager.Instance.IsColorOwned(currentIndex))
       {
           //Color is owned
           //is it our current color  ?
           if(activeColorIndex == currentIndex)
           {
               colorBuySetText.text = "Current";
           }
           else
           {
                colorBuySetText.text = "Select";
           }
       }
       else
       {
           //is not owned
           colorBuySetText.text = "Buy: " + colorCost[currentIndex].ToString(); 
       }
    }

    private void OnTrailSelect(int currentIndex)
    {
        
        Debug.Log("Selecting trail button : "+ currentIndex);
        

        //if the button  clicked is already selected exit do nothing
       if(selectedTrailIndex == currentIndex)
        return;

        //preview trail
        //get the image of the preview button
        trailPanel.GetChild(selectedTrailIndex).GetComponent<RawImage>().texture = previousTrail;
        //keep the new trail's preview image in te preview
        previousTrail = trailPanel.GetChild(currentIndex).GetComponent<RawImage>().texture;
        // set the new trail preview image to the other camera
        trailPanel.GetChild(currentIndex).GetComponent<RawImage>().texture = trailPreviewTexture;

        //change the physical object of the trail preview
        if(lastPreviewObject != null)
            Destroy(lastPreviewObject);
        lastPreviewObject = GameObject.Instantiate(Manager.Instance.playerTrails[currentIndex]) as GameObject;
        lastPreviewObject.transform.SetParent(trailPreviewObject);
        lastPreviewObject.transform.localPosition = Vector3.zero;

        //make panel get bigger selected
        trailPanel.GetChild(currentIndex).GetComponent<RectTransform>().localScale = Vector3.one * 1.125f;
        // put the  one on normal scale
        trailPanel.GetChild(selectedTrailIndex).GetComponent<RectTransform>().localScale = Vector3.one;
        


           //set selected trail
       selectedTrailIndex = currentIndex;

       //change the content of the buy set button
       if(SaveManager.Instance.IsTrailOwned(currentIndex))
       {
           //Color is owned
           //is it our current color  ?
           if(activeTrailIndex == currentIndex)
           {
               trailBuySetText.text = "Current";
           }
           else
           {
                trailBuySetText.text = "Select";
           }
       }
       else
       {
           //is not owned
           trailBuySetText.text = "Buy: " + trailCost[currentIndex].ToString(); 
       }


    }

    private void OnLevelSelect(int currentIndex)
    {
        Manager.Instance.currentLevel = currentIndex;
        isEnteringLevel = true;
        Debug.Log("Selecting level : "+ currentIndex);
    }

    public void OnColorBuySet()
    {
        Debug.Log("Buy/Set color");

        //is selected color is owned
        if(SaveManager.Instance.IsColorOwned(selectedColorIndex))
        {
            //set the color
            SetColor(selectedColorIndex);
        }
        else
        {
            //Attempt to buy color
            if(SaveManager.Instance.BuyColor(selectedColorIndex, colorCost[selectedColorIndex]))
            {
                //Success
                SetColor(selectedColorIndex);

                //change the color of the button
                colorPanel.GetChild(selectedColorIndex).GetComponent<Image>().color = Manager.Instance.playerColors[selectedColorIndex];   


                //update gold text
                UpdateGoldText();

            }
            else
            {
                //Do not have enough gold
                //play sound feedback
                Debug.Log("Not enough Gold");
            }

        }

    }
    public void OnTrailBuySet()
    {
        Debug.Log("Buy/Set trail");

        //is selected trail is owned
        if(SaveManager.Instance.IsTrailOwned(selectedTrailIndex))
        {
            //set the trail
            SetTrail(selectedTrailIndex);
        }
        else
        {
            //Attempt to buy trail
            if(SaveManager.Instance.BuyTrail(selectedTrailIndex, trailCost[selectedTrailIndex]))
            {
                //Success
                SetTrail(selectedTrailIndex);

                //change the trail of the button
                trailPanel.GetChild(selectedTrailIndex).GetComponent<RawImage>().color = Color.white;

                //update gold text
                UpdateGoldText();

            }
            else
            {
                //Do not have enough gold
                //play sound feedback
                Debug.Log("Not enough Gold");
            }

        }    

    }

    public void  OnTiltControl()
    {
        //toggle the accelerometer bool
        SaveManager.Instance.state.usingAccelerometer = !SaveManager.Instance.state.usingAccelerometer;

        //make sure we save the player preferences
        SaveManager.Instance.Save();

        //change th display image of the tilt control button
        tiltControlButton.GetComponent<Image>().color = (SaveManager.Instance.state.usingAccelerometer) ? tiltControlEnabled : tiltControlDisabled;
    }

}
