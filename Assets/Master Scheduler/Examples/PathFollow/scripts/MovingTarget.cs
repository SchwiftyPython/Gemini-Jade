using UnityEngine;
using System.Collections;

public class MovingTarget : MonoBehaviour
{
    Vector3 desiredPosition;
    Vector3 newPosition;
    UnityEngine.AI.NavMeshPath path;
    Transform SelfTransform;

    public float speed = 9;
    public Transform[] targets;
    int currentT = 0;
    Vector3 direction = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        path = new UnityEngine.AI.NavMeshPath();
        SelfTransform = transform;

        GetNewDesiredPosition();
    }

    // Update is called once per frame
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
