using System;
using Generators;
using GoRogue;
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
    public static int BucketSize = 5;
    
    /// <summary>
    /// The job giver
    /// </summary>
    public JobGiver jobGiver;

    public LocalMap map;
    
    /// <summary>
    /// Awakes this instance
    /// </summary>
    private void Awake()
    {
        var tickController = FindObjectOfType<TickController>();
        
        tickController.Init();
        
        jobGiver = new JobGiver();
        
        //testing map gen

        var mapGen = new LocalMapGenerator();
            
        map = mapGen.GenerateMap(50, 50);
        
        map.BuildAllMeshes();

        var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
        
        gridBuildingSystem.SetLocalMap(map);
        
        //testing pawn stuff

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
        
        map.CheckAllMatrices();
    }
}
