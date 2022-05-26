using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public FollowPath objectToSpawn;
    public Transform target;

    public int numberOfSpawns = 100;
    public int jobBatchSize = 300;
    int currentJobBatchSize;

    Scheduler spawningQueue;
    
    private Vector3 NewPosition;
    private FollowPath NewSpawn;


    // Use this for initialization
    void Start()
    {
        //SpawnAllInstancesQueued();
        SpawnAllInstances();
        
        currentJobBatchSize = jobBatchSize;
    }

    void Update()
    {

        if(currentJobBatchSize != jobBatchSize)
        {
            currentJobBatchSize = jobBatchSize;
            QueueHub.SetJobBatchSize("Pathing", jobBatchSize);
        }
    }
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
