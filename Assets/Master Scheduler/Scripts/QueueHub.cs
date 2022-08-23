using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Container that handles all queues.<br></br>
/// This should be used to manupilate all available queues.
/// </summary>
public class QueueHub : MonoBehaviour
{

    /// <summary>
    /// The instance
    /// </summary>
    private static QueueHub instance;

    /// <summary>
    /// Gets the value of the instance
    /// </summary>
    public static QueueHub Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject tmp = new GameObject("Queue Hub");
                instance = tmp.AddComponent<QueueHub>();
                return instance;
            }
            else return instance;
        }
    }
    
    /// <summary>
    /// The list of queues
    /// </summary>
    public Dictionary<string, Scheduler> Queues = new Dictionary<string, Scheduler>();

    /// <summary>
    /// Creates a queue that executes a number of jobs per frame
    /// </summary>
    /// <param name="queueName">name of the que</param>
    /// <param name="jobsPerFrame">number of jobs per frame</param>
    /// <param name="bIsLooping">does queue loop or remove jobs after execution ?</param>
    /// <returns>true on success, false if queue already exists</returns>
    public static bool CreateQueue(string queueName, int jobsPerFrame, bool bIsLooping)
    {
        if (Instance.Queues.ContainsKey(queueName))
        {
            Debug.Log("Queue already exists");
            return false;
        }
        else
        {
            Scheduler newQueue = Instance.gameObject.AddComponent<Scheduler>();
            newQueue.InitialiazeQueue(jobsPerFrame, bIsLooping);
            Instance.Queues.Add(queueName, newQueue);
            return true;
        }
    }

    /// <summary>
    /// Creates a queue that will execute all jobs within the given maxFrames
    /// </summary>
    /// <param name="queueName">name of the queue</param>
    /// <param name="bIsLooping">does queue loop or remove jobs after execution ?</param>
    /// <param name="maxFrames">number frames for the queue to execute all jobs</param>
    /// <returns>true on success, false if queue already exists</returns>
    public static bool CreateQueue(string queueName, bool bIsLooping, int maxFrames)
    {
        if (Instance.Queues.ContainsKey(queueName))
        {
            Debug.Log("Queue already exists");
            return false;
        }
        else
        {
            Scheduler newQueue = Instance.gameObject.AddComponent<Scheduler>();
            newQueue.InitialiazeQueue(bIsLooping, maxFrames);
            Instance.Queues.Add(queueName, newQueue);
            return true;
        }
    }

    /// <summary>
    /// sets the size the size of a job batch (number of jobs per queue tick)
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="newSize"></param>
    public static void SetJobBatchSize(string queueName, int newSize)
    {
        Scheduler OutTmpQueue = null;

        if (Instance.Queues.TryGetValue(queueName, out OutTmpQueue))
        {
            OutTmpQueue.SetJobBatchSize(newSize);
        }
    }

    /// <summary>
    /// constraints the queue to finish all jobs in the given number of frames
    /// </summary>
    /// <param name="queueName"></param>
    /// <param name="maxFrames"></param>
    public static void SetMaxFrames(string queueName, int maxFrames)
    {
        Scheduler OutTmpQueue = null;

        if (Instance.Queues.TryGetValue(queueName, out OutTmpQueue))
        {
            OutTmpQueue.SetMaxFrames(maxFrames);
        }
    }

    /// <summary>
    /// Adds job to the queue.
    /// </summary>
    /// <param name="queueName">name of the queue</param>
    /// <param name="Instigator">object responsible for the fob</param>
    /// <param name="newDelegate"></param>
    /// <returns>true on success, false if queue doesn't exist</returns>
    public static bool AddJobToQueue(string queueName, GameObject Instigator, QueueSpotDelegate newDelegate)
    {
        Scheduler OutTmpQueue = null;

        if (Instance.Queues.TryGetValue(queueName, out OutTmpQueue))
        {
            return OutTmpQueue.AddJobToQueue(Instigator, newDelegate);
        }

        else
        {
            Debug.Log("couldn't find queue");
            return false;
        }
    }

    /// <summary>
    /// Removes all jobs in the queue from the given game object
    /// </summary>
    /// <param name="queueName">name of the queue</param>
    /// <param name="Instigator"></param>
    public static void RemoveJobFromQueue(string queueName, GameObject Instigator)
    {
        Scheduler OutTmpQueue = null;

        if(Instance.Queues.TryGetValue(queueName, out OutTmpQueue))
        {
            OutTmpQueue.RemoveJobFromQueue(Instigator);
        }

        else
        {
            Debug.Log("couldn't find queue");
        }
    }

    /// <summary>
    /// Sets the frequency at which the queue is processed.
    /// </summary>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="UpdateRate">will update everyframe if <= 0.</param>
    public static void SetQueueUpdateRate(string queueName, float UpdateRate)
    {
        Scheduler OutTmpQueue = null;

        if (Instance.Queues.TryGetValue(queueName, out OutTmpQueue))
        {
            OutTmpQueue.SetUpdateRate(UpdateRate);
        }

        else
        {
            Debug.Log("couldn't find queue");
        }
    }

    /// <summary>
    /// Destroys the queue
    /// </summary>
    /// <param name="queueName">the name of the queue</param>
    /// <param name="bImmediate">if false, the queue will run all its jobs and then destroy itself, otherwise the destruction is immediate</param>
    public static void DestroyQueue(string queueName, bool bImmediate)
    {
        Scheduler OutTmpQueue = null;

        if (Instance.Queues.TryGetValue(queueName, out OutTmpQueue))
        {
            OutTmpQueue.DestroyQueue(bImmediate);
        }
    }

    /// <summary>
    /// Checks if given queue exists
    /// </summary>
    /// <param name="queueName"></param>
    /// <returns>true if queue exists, false otherwise</returns>
    public static bool DoesQueueExist(string queueName)
    {
        return Instance.Queues.ContainsKey(queueName);
    }

}
