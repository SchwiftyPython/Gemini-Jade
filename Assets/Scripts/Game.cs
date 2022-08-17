using Generators;
using GoRogue;
using Repos;
using Time;
using UnityEngine;
using World;
using World.Pawns.Jobs;

public class Game : MonoBehaviour
{
    public JobGiver jobGiver = new JobGiver();
    
    private void Awake()
    {
        var tickController = FindObjectOfType<TickController>();
        
        tickController.Init();
        
        //testing map gen

        var mapGen = new LocalMapGenerator();
            
        var map = mapGen.GenerateMap(50, 50);

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
}
