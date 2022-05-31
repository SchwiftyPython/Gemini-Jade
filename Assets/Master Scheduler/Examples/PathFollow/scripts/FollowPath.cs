using UnityEngine;
using System.Collections;

public class FollowPath : MonoBehaviour
{
    public Transform target;
    public float speed;

    //If true, the scheduler will be used
    public bool bUseQueue = true;

    

    UnityEngine.AI.NavMeshPath Path;
    [HideInInspector]
    public Transform SelfTransform;
    Vector3 NewPosition;
    [HideInInspector]
    public Vector3 velocity = Vector3.zero;
    Vector3 NoiseVector;
    Vector3 tmpVec = Vector3.zero;
    bool pathResult = false;
    

    // Use this for initialization
    void Start()
    {
        SelfTransform = GetComponent<Transform>();
        Path = new UnityEngine.AI.NavMeshPath();
        speed *= Random.Range(1, 3);    // randomize speed
        velocity = Vector3.zero;
        NoiseVector = Vector3.zero;     // random noise added to the follower's direction
        NoiseVector.x = Random.Range(-20, 20);
        NoiseVector.z = Random.Range(-20, 20);

        FollowerManager.Instance.Followers.Add(this);

        //joins the pathing queue
        if (bUseQueue)
        {
            
            JoinQueue();
            UpdateVelocity();
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //if not using the Scheduler, update path directly
    //    if(!bUseQueue)
    //        UpdatePath();

    //    SelfTransform.position += velocity;
    //}

    // Does same work as Update() but is called from FollowerManager
    public void UpdateVelocity()
    {
        if (!bUseQueue)
            UpdatePath();

        SelfTransform.position += velocity;
    }
    //updates the path to the target
    void UpdatePath()
    {
        //Get path to target and calculate velocity toward it
        pathResult = UnityEngine.AI.NavMesh.CalculatePath(SelfTransform.position, target.position/* + NoiseVector*/, UnityEngine.AI.NavMesh.AllAreas, Path);
        if(!pathResult || Path.corners.Length <= 1) 
        {
            velocity = Vector3.zero;
        }
        else
        {
            tmpVec = Path.corners[1] - SelfTransform.position;
            if (tmpVec.sqrMagnitude < 10)
                velocity = Vector3.zero;
            else
            {
                velocity = tmpVec.normalized * /*Time.deltaTime **/  speed *0.016f;
                velocity.y = 0;
            }
        }
    }

    void JoinQueue()
    {
        //If Pathing queue exists, join it
        if (QueueHub.DoesQueueExist("Pathing"))
            QueueHub.AddJobToQueue("Pathing", gameObject, UpdatePath);
        else
        {
            //create a looping queue that runs 100 jobs per frames
            QueueHub.CreateQueue("Pathing", 300, true);
            QueueHub.AddJobToQueue("Pathing", gameObject, UpdatePath);
        }
    }
}
