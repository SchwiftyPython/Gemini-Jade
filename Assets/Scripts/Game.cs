using System;
using Controllers;
using Generators;
using Repos;
using Time;
using UnityEngine;
using UnityEngine.Serialization;
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

    public StackCountLabelController stackCountLabelController;

    public LocalMap CurrentLocalMap { get; private set; }

    public Action<LocalMap> onNewLocalMap;

    private void Awake()
    {
        var tickController = FindObjectOfType<TickController>();
        
        tickController.Init();

        //temp map gen

        var mapGen = new LocalMapGenerator();
            
        CurrentLocalMap = mapGen.GenerateMap(100, 100);
        
        onNewLocalMap?.Invoke(CurrentLocalMap);
        
        CurrentLocalMap.BuildAllMeshes();

        var gridBuildingSystem = FindObjectOfType<GridBuildingSystem>();
        
        gridBuildingSystem.SetLocalMap(CurrentLocalMap);
        
        jobGiver = new JobGiver();
        
        //temp pawn stuff

        var pawnRepo = FindObjectOfType<PawnRepo>();

        var numPawns = Random.Range(2, 6);

        for (int i = 0; i < numPawns; i++)
        {
            var testPawn = PawnRepo.CreatePawn(pawnRepo.GetHumanTemplate());
            
            var spawnPoint = CurrentLocalMap.GetRandomTile(true);

            gridBuildingSystem.LocalMap.PlacePawn(testPawn, spawnPoint.Position);
        
            tickController.AddTestPawn(testPawn);
            
            jobGiver.RegisterPawn(testPawn); //probably move to pawn constructor
            
            jobGiver.RegisterMap(CurrentLocalMap); //probably move to map constructor
        }

        var localMapHolder = FindObjectOfType<LocalMapHolder>();
            
        localMapHolder.Build(CurrentLocalMap);

        stackCountLabelController = GetComponentInChildren<StackCountLabelController>();
    }

    private void Update()
    {
        //todo not sure if I want to keep this map stuff here
        
        if (CurrentLocalMap == null)
        {
            return;
        }
        
        CurrentLocalMap.UpdateVisibleBuckets();
        
        CurrentLocalMap.DrawBuckets();
    }

    private void LateUpdate()
    {
        CurrentLocalMap.CheckAllMatrices();
    }
}
