using UnityEngine;
using System.Collections;

/// <summary>

/// The spawner class

/// </summary>

/// <seealso cref="MonoBehaviour"/>

public class Spawner : MonoBehaviour
{
    /// <summary>
    /// The object to spawn
    /// </summary>
    public FollowPath objectToSpawn;
    /// <summary>
    /// The target
    /// </summary>
    public Transform target;

    /// <summary>
    /// The number of spawns
    /// </summary>
    public int numberOfSpawns = 100;
    /// <summary>
    /// The job batch size
    /// </summary>
    public int jobBatchSize = 300;
    /// <summary>
    /// The current job batch size
    /// </summary>
    int currentJobBatchSize;

    /// <summary>
    /// The spawning queue
    /// </summary>
    Scheduler spawningQueue;
    
    /// <summary>
    /// The new position
    /// </summary>
    private Vector3 NewPosition;
    /// <summary>
    /// The new spawn
    /// </summary>
    private FollowPath NewSpawn;


    // Use this for initialization
    /// <summary>
    /// Starts this instance
    /// </summary>
    void Start()
    {
        //SpawnAllInstancesQueued();
        SpawnAllInstances();
        
        currentJobBatchSize = jobBatchSize;
    }

    /// <summary>
    /// Updates this instance
    /// </summary>
    void Update()
    {

        if(currentJobBatchSize != jobBatchSize)
        {
            currentJobBatchSize = jobBatchSize;
            QueueHub.SetJobBatchSize("Pathing", jobBatchSize);
        }
    }
    /// <summary>
    /// Spawns the all instances
    /// </summary>
    void SpawnAllInstances()
    {
        for(int i = 0; i < numberOfSpawns; i++)
        {
            FollowPath NewSpawn = Instantiate<FollowPath>(objectToSpawn);
            NewSpawn.target = target;
            Vector3 NewPosition;
            NewPosition.x = Random.Range(-100, 100);
            NewPosition.y = 1;
            NewPosition.z = Random.Range(-100, 100);
            NewSpawn.transform.position = NewPosition;
            NewSpawn.speed = Random.Range(7, 20);
        }
    }

    //Spawns all instances using a queue
    /// <summary>
    /// Spawns the all instances queued
    /// </summary>
    void SpawnAllInstancesQueued()
    {
        //todo : use QueueHub to create the queue and add jobs to it
        QueueHub.CreateQueue("Spawning", 1000, false);
        for (int i = 0; i < numberOfSpawns; i++)
            QueueHub.AddJobToQueue("Spawning", gameObject, SpawnInstance);

        //destroy queue once all jobs are done
        QueueHub.DestroyQueue("Spawning", false);
        //spawningQueue = gameObject.AddComponent<Scheduler>();

        //spawningQueue.InitialiazeQueue(false, 60);
        //for (int i = 0; i < numberOfSpawns; i++)
        //    spawningQueue.AddJobToQueue(gameObject, SpawnInstance);
    }

    /// <summary>
    /// Spawns 1 instance.
    /// </summary>
    void SpawnInstance()
    {
        NewSpawn = Instantiate<FollowPath>(objectToSpawn);
        NewSpawn.target = target;
        
        NewPosition.x = Random.Range(-40, 40);
        NewPosition.y = 1;
        NewPosition.z = Random.Range(-40, 40);
        NewSpawn.transform.position = NewPosition;
        NewSpawn.speed = Random.Range(1, 80);
        //Debug.Log("Spawned 1 instance");
    }
}
