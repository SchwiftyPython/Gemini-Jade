using System;
using Generators;
using Repos;
using Time;
using UnityEngine;
using World;
using World.Pawns.Jobs;
using Random = UnityEngine.Random;

/// <summary>
/// The game class
/// </summary>
/// <seealso cref="MonoBehaviour"/>
public class Game : MonoBehaviour
{
    /// <summary>
    /// The job giver
    /// </summary>
    public JobGiver jobGiver;

    private LocalMap map;

    public Action<LocalMap> onNewLocalMap;

    private void Awake()
    {
        var tickController = FindObjectOfType<TickController>();
        
        tickController.Init();

        //temp map gen

        var mapGen = new LocalMapGenerator();
            
        map = mapGen.GenerateMap(100, 100);
        
        onNewLocalMap?.Invoke(map);
        
        map.BuildAllMeshes();

        var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
        
        gridBuildingSystem.SetLocalMap(map);
        
        jobGiver = new JobGiver();
        
        //temp pawn stuff

        var pawnRepo = FindObjectOfType<PawnRepo>();

        var numPawns = Random.Range(2, 6);

        for (int i = 0; i < numPawns; i++)
        {
            var testPawn = PawnRepo.CreatePawn(pawnRepo.GetHumanTemplate());
            
            var spawnPoint = map.GetRandomTile(true);

            gridBuildingSystem.LocalMap.PlacePawn(testPawn, spawnPoint.Position);
        
            tickController.AddTestPawn(testPawn);
            
            jobGiver.RegisterPawn(testPawn); //probably move to pawn constructor
            
            jobGiver.RegisterMap(map); //probably move to map constructor
        }

        var localMapHolder = FindObjectOfType<LocalMapHolder>();
            
        localMapHolder.Build(map);
    }

    private void Update()
    {
        //todo not sure if I want to keep this map stuff here
        
        if (map == null)
        {
            return;
        }
        
        map.UpdateVisibleBuckets();
        
        map.DrawBuckets();
    }

    private void LateUpdate()
    {
        map.CheckAllMatrices();
    }
}
