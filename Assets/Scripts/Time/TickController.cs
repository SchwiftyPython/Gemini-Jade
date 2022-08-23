using System;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Utilities;
using Time.TickerTypes;
using Time.TimeSpeeds;
using UnityEngine;
using World.Pawns;
using World.Things;

namespace Time
{
    /// <summary>
    /// The tick controller class
    /// </summary>
    /// <seealso cref="MonoBehaviour"/>
    public class TickController : MonoBehaviour
    {
        /// <summary>
        /// The rare tick interval
        /// </summary>
        public const int RareTickInterval = 250;
        
        /// <summary>
        /// The num ticks
        /// </summary>
        private int _numTicks;

        /// <summary>
        /// The starting year
        /// </summary>
        private int _startingYear = 0;

        /// <summary>
        /// The clock
        /// </summary>
        private Stopwatch _clock;

        /// <summary>
        /// The test pawns
        /// </summary>
        private List<Pawn> _testPawns;

        /// <summary>
        /// The game
        /// </summary>
        private Game _game;
        
        //todo need collapsible header
        /// <summary>
        /// The paused
        /// </summary>
        public TimeSpeed paused;
        /// <summary>
        /// The normal speed
        /// </summary>
        public TimeSpeed normalSpeed;
        /// <summary>
        /// The fast
        /// </summary>
        public TimeSpeed fast;
        /// <summary>
        /// The ultra
        /// </summary>
        public TimeSpeed ultra;

        //todo need collapsible header
        /// <summary>
        /// The normal tick
        /// </summary>
        public TickerType normalTick;
        /// <summary>
        /// The rare tick
        /// </summary>
        public TickerType rareTick;
        /// <summary>
        /// The long tick
        /// </summary>
        public TickerType longTick;

        /// <summary>
        /// Gets the value of the current time per tick
        /// </summary>
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

        /// <summary>
        /// The settle tick
        /// </summary>
        private int _settleTick;

        /// <summary>
        /// The real time to tick through
        /// </summary>
        private float _realTimeToTickThrough;

        /// <summary>
        /// The normal ticks
        /// </summary>
        private TickList normalTicks;
        /// <summary>
        /// The rare ticks
        /// </summary>
        private TickList rareTicks;
        /// <summary>
        /// The long ticks
        /// </summary>
        private TickList longTicks;
    
        /// <summary>
        /// The game start tick
        /// </summary>
        public int gameStartTick;

        /// <summary>
        /// The current speed
        /// </summary>
        public TimeSpeed currentSpeed;

        /// <summary>
        /// The pre pause speed
        /// </summary>
        public TimeSpeed prePauseSpeed;
    
        /// <summary>
        /// Gets the value of the num ticks
        /// </summary>
        public int NumTicks => _numTicks;

        /// <summary>
        /// Gets the value of the ticks since settled
        /// </summary>
        public int TicksSinceSettled => _numTicks - _settleTick;

        /// <summary>
        /// Gets the value of the settle tick
        /// </summary>
        public int SettleTick => _settleTick;

        /// <summary>
        /// Gets the value of the starting year
        /// </summary>
        public int StartingYear => _startingYear;

        /// <summary>
        /// Gets the value of the tick rate multiplier
        /// </summary>
        public float TickRateMultiplier
        {
            get
            {
                if (currentSpeed == paused)
                {
                    return paused.tickRateMultiplier;
                }
            
                if (currentSpeed == normalSpeed)
                {
                    return normalSpeed.tickRateMultiplier;
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

        /// <summary>
        /// Gets the value of the paused
        /// </summary>
        public bool Paused => currentSpeed == paused;

        /// <summary>
        /// Gets or sets the value of the current time speed
        /// </summary>
        public TimeSpeed CurrentTimeSpeed
        {
            get => currentSpeed;

            set => currentSpeed = value;
        }

        /// <summary>
        /// Gets the value of the colony settled
        /// </summary>
        public bool ColonySettled => _settleTick > 0;

        /// <summary>
        /// Lates the update
        /// </summary>
        private void LateUpdate()
        {
            if (Paused)
            {
                return;
            }

            var currentTimePerTick = CurrentTimePerTick;

            if (Mathf.Abs(UnityEngine.Time.deltaTime - currentTimePerTick) < currentTimePerTick * .01f)
            {
                _realTimeToTickThrough += currentTimePerTick;
            }
            else
            {
                _realTimeToTickThrough += UnityEngine.Time.deltaTime;
            }

            if (_clock == null)
            {
                _clock = new Stopwatch();
            }
       
            _clock.Reset();
            _clock.Start();
        
            var tickRateMultiplier = TickRateMultiplier;

            var numTicks = 0;

            while (_realTimeToTickThrough > 0f && numTicks < tickRateMultiplier * 2f)
            {
                DoSingleTick();
                _realTimeToTickThrough -= currentTimePerTick;
                numTicks++;
                if (Paused) //todo or frame rate is bad
                {
                    break;
                }
            }

            if (_realTimeToTickThrough > 0f)
            {
                _realTimeToTickThrough = 0;
            }
        }

        /// <summary>
        /// Inits this instance
        /// </summary>
        public void Init()
        {
            _clock = new Stopwatch();

            normalTicks = new TickList(normalTick);
            rareTicks = new TickList(rareTick);
            longTicks = new TickList(longTick);

            _game = FindObjectOfType<Game>();
            
            _testPawns = new List<Pawn>();
            
            //TESTING Health Debug

            HealthDebug.OnPawnSelected += AddTestPawn;

            //END TESTING Health Debug 
        }

        /// <summary>
        /// Does the single tick
        /// </summary>
        public void DoSingleTick()
        {
            //todo if this is slowing us down maybe look into adding all tick lists to scheduler
            
            //todo find all maps and tick through them

            _numTicks += 2000;
        
            //Shader.SetGlobalFloat() called here. Not sure if needed later
        
            normalTicks.Tick();
            rareTicks.Tick();
            longTicks.Tick();
        
            _game.jobGiver.Tick();
        
            //todo Date Notifier Tick
        
            //todo Scenario tick
        
            //todo World Tick -- Pawns are ticked here. Bypassing for now to test health boi

            foreach (var pawn in _testPawns.ToArray())
            {
                pawn.Tick();
            }
        
            //todo Game End Tick
        
            //todo StoryTeller Tick
        
            //todo Quest Manager Tick
        
            //todo Auto Saver Tick
        }

        /// <summary>
        /// Toggles the pause
        /// </summary>
        public void TogglePause()
        {
            if (currentSpeed != paused)
            {
                prePauseSpeed = currentSpeed;
                currentSpeed = paused;
            }
            else if (prePauseSpeed != currentSpeed)
            {
                currentSpeed = prePauseSpeed;
            }
            else
            {
                currentSpeed = normalSpeed;
            }
        }

        /// <summary>
        /// Pauses this instance
        /// </summary>
        public void Pause()
        {
            if (currentSpeed != paused)
            {
                TogglePause();
            }
        }

        /// <summary>
        /// Registers the ticks for using the specified thing
        /// </summary>
        /// <param name="thing">The thing</param>
        public void RegisterTicksFor(Thing thing)
        {
            GetTickListFor(thing).Register(thing);
        }
    
        /// <summary>
        /// Uns the register ticks for using the specified thing
        /// </summary>
        /// <param name="thing">The thing</param>
        public void UnRegisterTicksFor(Thing thing)
        {
            GetTickListFor(thing).UnRegister(thing);
        }
        
        /// <summary>
        /// Resets the settlement ticks
        /// </summary>
        public void ResetSettlementTicks()
        {
            _settleTick = _numTicks;
        }

        /// <summary>
        /// Adds the test pawn using the specified pawn
        /// </summary>
        /// <param name="pawn">The pawn</param>
        public void AddTestPawn(Pawn pawn)
        {
            if (_testPawns == null)
            {
                _testPawns = new List<Pawn>();
            }
            
            _testPawns.Add(pawn);
        }

        /// <summary>
        /// Gets the tick list for using the specified thing
        /// </summary>
        /// <param name="thing">The thing</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns>The tick list</returns>
        private TickList GetTickListFor(Thing thing)
        {
            var thingTickerType = thing.template.TickerType;

            if (thingTickerType == normalTick)
            {
                return normalTicks;
            }

            if (thingTickerType == rareTick)
            {
                return rareTicks;
            }

            if(thingTickerType == longTick)
            {
                return longTicks;
            }

            throw new InvalidOperationException();
        }
    }
}


