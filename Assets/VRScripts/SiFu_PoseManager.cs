using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiFu_PoseManager : MonoBehaviour
{
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
    float spawnDistY = 1.2f;

    bool[] componentMatchArr = new bool[4];
    private static Dictionary<string, int> BodyComponents =
        new Dictionary<string, int>()
    {
        { "LeftHand", 0 },
        { "RightHand", 1 },
        { "LeftFoot", 2 },
        { "RightFoot", 3 }
    };

    float defaultHeight = 1.7f;
    float playerHeight;

    public bool gameRunning = false;

    public Valve.VR.InteractionSystem.SiFu_Trigger trigger;

    public GameObject beginPose;

    // Start is called before the first frame update
    void Start()
    {
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
        if (!gameRunning) {
            if (trigger.CheckGrabStarting())
            {
                Debug.Log("Grab!!!!!!!!!!!!!!!!!!");

                // TODO: initialize the game
                // gameRunning = true;
                // beginPose.SetActive(true);
            }
            else
            {
                //Debug.Log("No Grab");
            }
            //return; // for debugging
        }

        // spawn poses
        //timer += Time.deltaTime;
        if (currPose == null /*timer > spawnPeriod*/)
        {
            //timer = 0;
            for (int i = 0; i < numberSpawnedEachPeriod; i++)
            {
                //int idx = Random.Range(0, posesToSpawn.Count);
                currPose = Instantiate(posesToSpawn[idx++ % posesToSpawn.Count],
                    //VRCamera.ScreenToWorldPoint(
                    //    new Vector3(originInScreenCoords.x + spawnDistX, 
                    //                originInScreenCoords.y + spawnDistY, 
                    //                originInScreenCoords.z)),
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

    void ClearPose()
    {
        if (currPose == null) return;

        bool fullBodyMatch = true;
        //int id = 0;
        foreach (bool v in componentMatchArr)
        {
            //Debug.Log("i: " + id + " = " + v);
            //id++;
            fullBodyMatch = fullBodyMatch && v;
        }

        if (fullBodyMatch)
        {
            currPose.GetComponent<SiFu_Pose>().Die();
            currPose = null;
            for(int i = 0; i < componentMatchArr.Length; i++)
            {
                componentMatchArr[i] = false;
            }
        }
    }
}
