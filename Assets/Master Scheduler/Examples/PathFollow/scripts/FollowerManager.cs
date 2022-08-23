using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Updates FollowPath objects' velocities. The goal of this class is to call Update() once and process all objects, instead of 10,000 Update() calls
/// </summary>
public class FollowerManager : MonoBehaviour
{
    /// <summary>
    /// The follow path
    /// </summary>
    [HideInInspector]
    public List<FollowPath> Followers = new List<FollowPath>();
    /// <summary>
    /// The tmp vec
    /// </summary>
    Vector3 TmpVec;

    /// <summary>
    /// The instance
    /// </summary>
    private static FollowerManager instance;

    /// <summary>
    /// Gets the value of the instance
    /// </summary>
    public static FollowerManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject tmp = new GameObject("Follower Manager");
                instance = tmp.AddComponent<FollowerManager>();
                return instance;
            }
            else return instance;
        }
    }

    // Use this for initialization
    /// <summary>
    /// Starts this instance
    /// </summary>
    void Start()
    {

    }

    // Update is called once per frame
    /// <summary>
    /// Updates this instance
    /// </summary>
    void Update()
    {
        for(int i = 0; i < Followers.Count; i++)
        {
            Followers[i].UpdateVelocity();
            //TmpVec = Followers[i].SelfTransform.position;
            //TmpVec.x += Followers[i].velocity.x;
            //TmpVec.y += Followers[i].velocity.y;
            //TmpVec.z += Followers[i].velocity.z;
            //Followers[i].SelfTransform.position = TmpVec;
        }
    }
}
