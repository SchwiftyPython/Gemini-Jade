using Generators;
using Time;
using UnityEngine;
using World;

public class Game : MonoBehaviour
{
    private void Awake()
    {
        var tickController = FindObjectOfType<TickController>();
        
        tickController.Init();
        
        //testing map gen

        var mapGen = new LocalMapGenerator();
            
        var map = mapGen.GenerateMap(50, 50);

        var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
        
        gridBuildingSystem.SetLocalMap(map);
        
        var localMapHolder = FindObjectOfType<LocalMapHolder>();
            
        localMapHolder.Build(map);
    }
}
