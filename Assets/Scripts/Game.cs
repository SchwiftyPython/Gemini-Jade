using Generators;
using GoRogue;
using Repos;
using Time;
using UnityEngine;
using World;
using World.Pawns;

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
        
        //testing pawn stuff

        var pawnRepo = FindObjectOfType<PawnRepo>();

        var testPawn = PawnRepo.CreatePawn(pawnRepo.GetHumanTemplate());

        gridBuildingSystem.LocalMap.PlacePawn(testPawn, new Coord(25, 25));
        
        var localMapHolder = FindObjectOfType<LocalMapHolder>();
            
        localMapHolder.Build(map);
    }
}
