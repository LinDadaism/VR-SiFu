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
    private static List<List<GameObject>> poses = new List<List<GameObject>>();

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
        { "Background", "impersonatingSifu" },
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
    public int maxhealth = 140;
    [HideInInspector]
    public int health;

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

    // Transforms to act as start and end markers for the translation.
    public Vector3 cueStartMarker = new Vector3(8.0f, 1.35f, 0.0f);
    public Vector3 cueEndMarker = new Vector3(2.6f, 1.35f, 0.0f);

    bool waitForGameStart = true;

    public GameObject beginHintText;
    public GameObject scoreCanvas;
    public GameObject healthCanvas;
    public GameObject lossCanvas;
    public GameObject winCanvas;
    public SiFu_Time  poseTimer;

    public List<GameObject> levels;

    public GameObject cameraObj;

    public GameObject weaponsObj;

    public List<SiFu_PickUpWeapon> weapons;

    GameObject targetWeaponObj;

    public int holdingWeaponType = 0;
    [HideInInspector]
    public int targetWeaponType;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        health = maxhealth;
        spawnPeriod = 8.0f;
        numberSpawnedEachPeriod = 1;
        poses.Add(posesToSpawn);
        poses.Add(posesLevel1);
        poses.Add(posesLevel2);
        poses.Add(posesLevel3);
    }

    void Update()
    {
        // striking a gong to start
        if (waitForGameStart)
        {
            if (trigger.CheckGrabStarting())
            {
                SetLevelGongs();
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
                currPose = Instantiate((poses[currLevel])[idx++ % (poses[currLevel]).Count], // currently looping thru the pose list
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
        if (numSpawned > 0 && numPose + numCombo + numWeapon == poses[currLevel].Count)
        {
            Win();
        }
    }

    public void StartGame(int level)
    {
        health = maxhealth;
        currLevel = level;
        gameRunning = true;
        waitForGameStart = false;
        beginHintText.SetActive(false);
        scoreCanvas.SetActive(true);
        healthCanvas.SetActive(true);
        lossCanvas.SetActive(false);
        winCanvas.SetActive(false);
        SetLevelGongs(false);
        //Time.timeScale = 1;
    }

    public void setComponentMatch(string name, bool isMatch)
    {

        componentMatchArr[BodyComponents[name]] = isMatch;
        
        // play sound effect
        if (isMatch)
        {
            PlayAudio(SoundClips[name]);
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
        }

        if (fullBodyMatch)
        {
            // add score
            if (!force)
            {
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

            currPose.GetComponent<SiFu_Pose>().Die();
            currPose = null;
            for (int i = 0; i < componentMatchArr.Length; i++)
            {
                componentMatchArr[i] = false;
            }


            if (holdingWeaponType != 0)
            {
                holdingWeaponType = 0;
                foreach (SiFu_PickUpWeapon w in weapons)
                {
                    if (w.holding)
                    {
                        w.Release();
                    }
                }
            }
            weaponsObj.SetActive(false);
            targetWeaponType = 0;
            targetWeaponObj = null;
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
        if(newWeaponType == 0)
        {
            targetWeaponObj.SetActive(false);
            targetWeaponObj = null;
        }
        else if(holdingWeaponType == targetWeaponType && targetWeaponObj != null)
        {
            Debug.Log("Show targetWeaponObj");
            targetWeaponObj.SetActive(true);
        }
        else
        {
            targetWeaponObj.SetActive(false);
        }
    }

    void SetLevelGongs(bool enabled = true)
    {
        foreach (GameObject level in levels)
        {
            level.SetActive(enabled);
        }
    }

    void PlayAudio(string clip)
    {
        AudioSource[] sounds = GetComponents<AudioSource>();
        foreach (AudioSource sound in sounds)
        {
            if (sound.clip.name == clip)
            {
                sound.Play();
            }
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
        ClearPose(true);
        gameRunning = false;
        lossCanvas.SetActive(true);
        PlayAudio(SoundClips["Loss"]);
        SetLevelGongs();

        // Time.timeScale = 0;
    }

    void Win()
    {
        ClearPose(true);
        Debug.Log("You win!");
        gameRunning = false;
        winCanvas.SetActive(true);
        PlayAudio(SoundClips["Win"]);
        SetLevelGongs();

        // Time.timeScale = 0;
    }
}
