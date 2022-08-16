using System.Collections;
using System.Collections.Generic;
using GoRogue;
using UnityEngine;
using World;
using World.Pawns.Skills;

public class Job
{
    private Coord _location;

    public Skill SkillNeeded { get; }

    public int SkillLevelNeeded { get; }
    
    //todo resources needed

    public Job()
    {
        
    }
}
