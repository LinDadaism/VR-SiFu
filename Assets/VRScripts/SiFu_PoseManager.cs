using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_PoseManager : MonoBehaviour
{
    public static SiFu_PoseManager instance;

    public float timer;
    public float spawnPeriod; // how frequently a pose is spawned e.g. every 5 seconds
    public int numberSpawnedEachPeriod;
    int numSpawned;

    // Level pose sets
    public int currLevel = 0;
    public List<GameObject> posesLevel1     = new List<GameObject>();
    public List<GameObject> posesLevel2     = new List<GameObject>();
    public List<GameObject> posesLevel3     = new List<GameObject>();
    public List<GameObject> posesToSpawn    = new List<GameObject>();
    private static List<List<GameObject>> poses;

    private int idx = 0;
    public Camera VRCamera;

    public GameObject currPose;
    float spawnDistX = 8.0f;
    float spawnDistY = 1.35f;

    bool[] componentMatchArr = new bool[4];
    private static Dictionary<string, int> BodyComponents =
        new Dictionary<string, int>()
    {
        { "LeftHand", 0 },
        { "RightHand", 1 },
        { "LeftFoot", 2 },
        { "RightFoot", 3 }
    };
    private static Dictionary<string, string> SoundClips =
    new Dictionary<string, string>()
    {
        { "LeftHand", "hoo" },
        { "RightHand", "ha" },
        { "LeftFoot", "hee" },
        { "RightFoot", "hey" },
        { "Background", "impersonatingSifu" }
        { "Level1", "kaiTheme" },
        { "Level2", "skilledInTheArts" },
        { "Level3", "skilledInTheArts" },
        { "Loss", "oogwayAscending" },
        { "Win", "dragonArises" }
    };

    // for UI
    [HideInInspector]
    public int score;              // level of matery!
    [HideInInspector]
    public int health = 140;

    private int poseVal = 100;      // points a static pose values 
    private int comboVal = 200;     // points a moving pose values
    private int weaponVal = 300;    // points a pose with weapon values
    private int numPose;            // the number of each pose type being hit
    private int numCombo;
    private int numWeapon;
    private int currPoseType;       // 0-pose, 1-combo, 2-weapon
    private int gameState;          // 0-ongoing, 1-win, 2-loss

    // body size calibration
    float defaultHeight = 1.574f;
    float playerHeight;

    public bool gameRunning = false;

    public Valve.VR.InteractionSystem.SiFu_Trigger trigger;

    public GameObject beginPose;

    public int holdingWeaponType = 0;

    // Transforms to act as start and end markers for the translation.
    public Vector3 cueStartMarker = new Vector3(8.0f, 1.35f, 0.0f);
    public Vector3 cueEndMarker = new Vector3(2.6f, 1.35f, 0.0f);

    bool waitForGameStart = true;

    public GameObject beginHintText;
    public GameObject scoreCanvas;
    public GameObject healthCanvas;
    public SiFu_Time  poseTimer;

    public GameObject gong;

    public GameObject cameraObj;

    public GameObject weaponsObj;

    public List<SiFu_PickUpWeapon> weapons;

    GameObject targetWeaponObj;

    [HideInInspector]
    public int targetWeaponType;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {   
        spawnPeriod = 8.0f;
        numberSpawnedEachPeriod = 1;
        poses = { posesToSpawn, posesLevel1, posesLevel2, posesLevel3 };
    }

    // Update is called once per frame
    void Update()
    {
        /* switched to striking a gong to start */
        if (waitForGameStart)
        {
            if (trigger.CheckGrabStarting())
            {
                gong.SetActive(true);
                playerHeight = cameraObj.transform.position.y;
                Debug.Log("playerHeight: " + playerHeight.ToString());
            }
            else
            {
                return;
            }
        }

        // spawn poses
        //timer += Time.deltaTime;
        if (currPose == null && gameRunning /*timer > spawnPeriod*/)
        {
            //timer = 0;
            for (int i = 0; i < numberSpawnedEachPeriod; i++)
            {
                //int idx = Random.Range(0, posesToSpawn.Count);
                currPose = Instantiate(poses[currLevel][idx++ % poses[currLevel].Count], // currently looping thru the pose list
                    new Vector3(spawnDistX, spawnDistY, 0f),
                    Quaternion.AngleAxis(90.0f, Vector3.up));
                numSpawned++;

                //Debug.Log("0-currPose: " + (currPose == null).ToString());
                SiFu_Pose poseComp = currPose.GetComponent<SiFu_Pose>();
                if (poseComp != null)
                {
                    poseComp.SetScale(playerHeight / defaultHeight);
                    // start timer animation
                    if (poseTimer) poseTimer.BurnIncense(poseComp.waitTime);

                    if(poseComp.tag == "WeaponPose")
                    {
                        Debug.Log("active weapons");
                        weaponsObj.SetActive(true);
                    }
                    else {
                        Debug.Log("not active weapons");
                    }
                }
                else
                {
                    Debug.Log("currPose Doesn't have SiFu_Pose Component!");
                }
            }
        }

        // win condition
        if (numPose + numCombo + numWeapon == numSpawned)
        {
            Win();
        }
    }

    public void StartGame()
    {
        gameRunning = true;
        waitForGameStart = false;
        beginHintText.SetActive(false);
        scoreCanvas.SetActive(true);
        healthCanvas.SetActive(true);
        Time.timeScale = 1;
    }

    public void setComponentMatch(string name, bool isMatch)
    {

        componentMatchArr[BodyComponents[name]] = isMatch;
        
        // play sound effect
        if (isMatch)
        {
            AudioSource[] sounds = GetComponents<AudioSource>();
            foreach (AudioSource sound in sounds)
            {
                if (sound.clip.name == SoundClips[name])
                {
                    sound.Play();
                }
            }
        }
        
        ClearPose();
    }

    public void setCurrPose(string poseName)
    {
        currPose = GameObject.Find(poseName);
    }

    void ClearPose(bool force = false)
    {
        if (currPose == null) return;

        currPose.GetComponent<SiFu_Pose>().Die();
        currPose = null;
        for (int i = 0; i < componentMatchArr.Length; i++)
        {
            componentMatchArr[i] = false;
        }

        bool fullBodyMatch = true;
        //int id = 0;
        if (!force)
        {
            foreach (bool v in componentMatchArr)
            {
                //Debug.Log("i: " + id + " = " + v);
                //id++;
                fullBodyMatch = fullBodyMatch && v;
            }

            if (fullBodyMatch)
            {
                // add score
                if (currPose.tag == "StaticPose")
                {
                    numPose++;
                    score += poseVal;

                }
                else if (currPose.tag == "ComboPose")
                {
                    numCombo++;
                    score += comboVal;
                }
                else if (currPose.tag == "WeaponPose")
                {
                    numWeapon++;
                    score += weaponVal;
                }
            }
            
            if (holdingWeaponType != 0)
            {
                holdingWeaponType = 0;
                foreach (SiFu_PickUpWeapon w in weapons)
                {
                    if (w.weaponType == holdingWeaponType)
                    {
                        w.Release();
                    }
                }
            }
            weaponsObj.SetActive(false);
        }
    }

    public void LookForWeapon(int type, GameObject weaponObj)
    {
        targetWeaponType = type;
        targetWeaponObj = weaponObj;
    }

    public void SetWeapon(int newWeaponType)
    {
        Debug.Log("SetWeapon " + newWeaponType.ToString());
        holdingWeaponType = newWeaponType;
        if(holdingWeaponType == targetWeaponType && targetWeaponObj != null)
        {
            Debug.Log("Show targetWeaponObj");
            targetWeaponObj.SetActive(true);
        }
        else
        {
            targetWeaponObj.SetActive(false);
        }
    }

    public void HitPlayer()
    {
        Debug.Log("Hit The Player");
        health -= 20;
        if(health <= 0)
        {
            Lose();
        }
        ClearPose(true);
    }

    // loss condition
    void Lose()
    {
        Debug.Log("You lose!");
        gameRunning = false;
        currPose = null;
        Time.timeScale = 0;
        SoundClips["Loss"].play();
    }

    void Win()
    {
        Debug.Log("You win!");
        gameRunning = false;
        currPose = null;
        Time.timeScale = 0;
        SoundClips["Win"].play();
    }
}
