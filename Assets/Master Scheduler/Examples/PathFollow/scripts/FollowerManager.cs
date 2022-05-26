using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Updates FollowPath objects' velocities. The goal of this class is to call Update() once and process all objects, instead of 10,000 Update() calls
/// </summary>
public class FollowerManager : MonoBehaviour
{
    [HideInInspector]
    public List<FollowPath> Followers = new List<FollowPath>();
    Vector3 TmpVec;

    private static FollowerManager instance;

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
    void Start()
    {

    }

    // Update is called once per frame
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
