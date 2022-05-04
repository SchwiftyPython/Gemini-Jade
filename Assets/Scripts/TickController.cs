using System.Diagnostics;
using TimeSpeeds;
using UnityEngine;

public class TickController : MonoBehaviour
{
    [SerializeField] private TimeSpeed paused;
    [SerializeField] private TimeSpeed normal;
    [SerializeField] private TimeSpeed fast;
    [SerializeField] private TimeSpeed ultra;

    private int _numTicks;

    private int _startingYear = 0;

    private Stopwatch clock;

    private float CurrentTimePerTick
    {
        get
        {
            if (TickRateMultiplier == 0)
            {
                return 0f;
            }

            return 1f / (60f * TickRateMultiplier);
        }
    }

    private int _settleTick;

    public int gameStartTick;

    public TimeSpeed currentSpeed;

    public TimeSpeed prePauseSpeed;
    
    //todo TickLists

    public int NumTicks => _numTicks;

    public int TicksSinceSettled => _numTicks - _settleTick;

    public int SettleTick => _settleTick;

    public int StartingYear => _startingYear;

    public float TickRateMultiplier
    {
        get
        {
            if (currentSpeed == paused)
            {
                return paused.tickRateMultiplier;
            }
            
            if (currentSpeed == normal)
            {
                return normal.tickRateMultiplier;
            }
            
            if (currentSpeed == fast)
            {
                return fast.tickRateMultiplier;
            }
            
            if (currentSpeed == ultra)
            {
                return ultra.tickRateMultiplier;
            }

            return -1;
        }
    }

    public bool Paused => currentSpeed == paused;

    public TimeSpeed CurrentTimeSpeed
    {
        get => currentSpeed;

        set => currentSpeed = value;
    }

    public bool ColonySettled => _settleTick > 0;
    
    //todo methods
}


