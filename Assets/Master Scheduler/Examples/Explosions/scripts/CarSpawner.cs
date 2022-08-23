using UnityEngine;
using System.Collections;


/// <summary>

/// The car spawner class

/// </summary>

/// <seealso cref="MonoBehaviour"/>

public class CarSpawner : MonoBehaviour
{

    /// <summary>
    /// The car prefab
    /// </summary>
    public GameObject carPrefab;
    /// <summary>
    /// The 
    /// </summary>
    public int n = 10;

    /// <summary>
    /// The zero
    /// </summary>
    Vector3 tmpPosition = Vector3.zero;

    // Use this for initialization
    /// <summary>
    /// Starts this instance
    /// </summary>
    void Start()
    {
        tmpPosition.y = 0.5f;
        Spawn();
    }

    /// <summary>
    /// Spawns this instance
    /// </summary>
    void Spawn()
    {
        for (int i = -n / 2; i < 2 * n; i++)
        {
            for (int j = -n / 2; j < 2 * n; j++)
            {
                GameObject NewCar = Instantiate(carPrefab);
                tmpPosition.x = i * 3.5f;
                tmpPosition.z = j * 2f;
                NewCar.transform.position = tmpPosition;
            }
        }
    }

}
