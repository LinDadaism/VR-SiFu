using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_PoseManager : MonoBehaviour
{
    public static SiFu_PoseManager instance;

    public float timer;
    public float spawnPeriod; // how frequently a pose is spawned e.g. every 5 seconds
    public int numberSpawnedEachPeriod;
    public List<GameObject> posesToSpawn = new List<GameObject>();
    private int idx = 0;
    public Camera VRCamera;

    [SerializeField]
    GameObject currPose;
    Vector3 originInScreenCoords;
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
    private static Dictionary<string, int> BodyComponentsSound =
    new Dictionary<string, int>()
    {
        { "LeftHand", 1 },
        { "RightHand", 1 },
        { "LeftFoot", 0 },
        { "RightFoot", 2 }
    };

    // for UI
    public int poseVal = 100; // points a static pose values 
    public int comboVal = 200; // points a moving pose values
    public int weaponVal = 300; // points a pose with weapon values
    private int score; // level of matery!
    private int numPose; // the number of each pose type being hit
    private int numCombo;
    private int numWeapon;
    private int currPoseType; // 0-pose, 1-combo, 2-weapon
    private int gameState; // 0-ongoing, 1-win, 2-loss

    public int health = 140;

    // body size calibration
    float defaultHeight = 1.7f;
    float playerHeight;

    public bool gameRunning = false;

    public Valve.VR.InteractionSystem.SiFu_Trigger trigger;

    public GameObject beginPose;

    public int holdingWeaponType = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        spawnPeriod = 8.0f;
        numberSpawnedEachPeriod = 1;

        if (VRCamera != null)
        {
            originInScreenCoords = VRCamera.WorldToScreenPoint(new Vector3(0, 0, 0));
            //Debug.Log("vr cam:" + originInScreenCoords);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // spawn poses
        //timer += Time.deltaTime;
        if (currPose == null && gameRunning /*timer > spawnPeriod*/)
        {
            //timer = 0;
            for (int i = 0; i < numberSpawnedEachPeriod; i++)
            {
                //int idx = Random.Range(0, posesToSpawn.Count);
                currPose = Instantiate(posesToSpawn[idx++ % posesToSpawn.Count], // currently looping thru the pose list
                    new Vector3(spawnDistX, spawnDistY, 0f),
                    Quaternion.AngleAxis(90.0f, Vector3.up));
                
                //Debug.Log("0-currPose: " + (currPose == null).ToString());
                SiFu_Pose poseComp = currPose.GetComponent<SiFu_Pose>();
                if (poseComp != null)
                {
                    poseComp.SetScale(1f);
                }
                else
                {
                    Debug.Log("currPose Doesn't have SiFu_Pose Component!");
                }
            }
        }
    }

    public void setComponentMatch(string name, bool isMatch)
    {

        componentMatchArr[BodyComponents[name]] = isMatch;
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
            if (currPose.tag == "StaticPose")
            {
                numPose++;
                if (!force)
                    score += poseVal;

            }
            else if (currPose.tag == "ComboPose")
            {
                numCombo++;
                if (!force)
                    score += comboVal;
            }
            else if (currPose.tag == "WeaponPose")
            {
                numWeapon++;
                if (!force)
                    score += weaponVal;
            }

            currPose.GetComponent<SiFu_Pose>().Die();
            currPose = null;
            for(int i = 0; i < componentMatchArr.Length; i++)
            {
                componentMatchArr[i] = false;
            }
        }
    }

    public void HitPlayer()
    {
        Debug.Log("Hit The Player");
        health -= 20;
        if(health <= 0)
        {
            Die();
        }
        ClearPose(true);
    }

    public void Die()
    {
        gameRunning = false;
    }
}
