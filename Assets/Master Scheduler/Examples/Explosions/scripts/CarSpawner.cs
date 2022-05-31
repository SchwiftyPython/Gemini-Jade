using UnityEngine;
using System.Collections;


public class CarSpawner : MonoBehaviour
{

    public GameObject carPrefab;
    public int n = 10;

    Vector3 tmpPosition = Vector3.zero;

    // Use this for initialization
    void Start()
    {
        tmpPosition.y = 0.5f;
        Spawn();
    }

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
