using Time;
using UnityEngine;

public class Game : MonoBehaviour
{
    private void Awake()
    {
        var tickController = FindObjectOfType<TickController>();
        
        tickController.Init();
    }
}
