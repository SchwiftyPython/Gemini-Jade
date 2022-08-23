using UnityEngine;
using System.Collections;

/// <summary>

/// The moving target class

/// </summary>

/// <seealso cref="MonoBehaviour"/>

public class MovingTarget : MonoBehaviour
{
    /// <summary>
    /// The desired position
    /// </summary>
    Vector3 desiredPosition;
    /// <summary>
    /// The new position
    /// </summary>
    Vector3 newPosition;
    /// <summary>
    /// The path
    /// </summary>
    UnityEngine.AI.NavMeshPath path;
    /// <summary>
    /// The self transform
    /// </summary>
    Transform SelfTransform;

    /// <summary>
    /// The speed
    /// </summary>
    public float speed = 9;
    /// <summary>
    /// The targets
    /// </summary>
    public Transform[] targets;
    /// <summary>
    /// The current
    /// </summary>
    int currentT = 0;
    /// <summary>
    /// The zero
    /// </summary>
    Vector3 direction = Vector3.zero;

    // Use this for initialization
    /// <summary>
    /// Starts this instance
    /// </summary>
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        SelfTransform = transform;

        GetNewDesiredPosition();
    }

    // Update is called once per frame
    /// <summary>
    /// Updates this instance
    /// </summary>
    void Update()
    {
        direction = targets[currentT].position - SelfTransform.position;
        if (direction.sqrMagnitude > 100)
            SelfTransform.position += direction.normalized * speed * UnityEngine.Time.deltaTime;
        else currentT++;
        if (currentT >= targets.Length)
            currentT = 0;

        //if (path.corners.Length > 1)
        //{
        //    newPosition = SelfTransform.position + (path.corners[1] - SelfTransform.position).normalized * Time.deltaTime * speed;
        //    newPosition.y = SelfTransform.position.y;
        //    SelfTransform.position = newPosition;

        //    if ((path.corners[1] - SelfTransform.position).magnitude < 1)
        //        GetNewDesiredPosition();
        //}

    }

    /// <summary>
    /// Gets the new desired position
    /// </summary>
    void GetNewDesiredPosition()
    {
        Vector3 Tmp = SelfTransform.position;
        Tmp.x += Random.Range(-40, 40);
        Tmp.y = 1;
        Tmp.z += Random.Range(-40, 40);
        if (Tmp.x > 40 || Tmp.x < -40)
            Tmp.x = 0;

        if (Tmp.z > 40 || Tmp.z < -40)
            Tmp.z = 0;

        UnityEngine.AI.NavMesh.CalculatePath(SelfTransform.position, Tmp, UnityEngine.AI.NavMesh.AllAreas, path);
        //Debug.Log("Getting new point");
    }
}
