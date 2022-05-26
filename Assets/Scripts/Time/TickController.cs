using System;
using System.Diagnostics;
using Assets.Scripts.Utilities;
using Time.TickerTypes;
using Time.TimeSpeeds;
using UnityEngine;
using World.Pawns;
using World.Things;

namespace Time
{
    public class TickController : MonoBehaviour
    {
        public const int RareTickInterval = 250;
        
        private int _numTicks;

        private int _startingYear = 0;

        private Stopwatch _clock;

        private Pawn _healthDebugPawn;
        
        //todo need collapsible header
        public TimeSpeed paused;
        public TimeSpeed normalSpeed;
        public TimeSpeed fast;
        public TimeSpeed ultra;

        //todo need collapsible header
        public TickerType normalTick;
        public TickerType rareTick;
        public TickerType longTick;

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

        private float _realTimeToTickThrough;

        private TickList normalTicks;
        private TickList rareTicks;
        private TickList longTicks;
    
        public int gameStartTick;

        public TimeSpeed currentSpeed;

        public TimeSpeed prePauseSpeed;
    
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

        public bool Paused => currentSpeed == paused;

        public TimeSpeed CurrentTimeSpeed
        {
            get => currentSpeed;

            set => currentSpeed = value;
        }

        public bool ColonySettled => _settleTick > 0;

        private void Update()
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

        public void Init()
        {
            _clock = new Stopwatch();

            normalTicks = new TickList(normalTick);
            rareTicks = new TickList(rareTick);
            longTicks = new TickList(longTick);
            
            //TESTING Health Debug

            HealthDebug.OnPawnSelected += SetHealthDebugPawn;

            //END TESTING Health Debug 
        }

        public void DoSingleTick()
        {
            //todo if this is slowing us down maybe look into adding all tick lists to scheduler
            
            //todo find all maps and tick through them

            _numTicks += 2000;
        
            //Shader.SetGlobalFloat() called here. Not sure if needed later
        
            normalTicks.Tick();
            rareTicks.Tick();
            longTicks.Tick();
        
            //todo they have a static Find class that basically does FindObjectOfType for everyone
        
            //todo Date Notifier Tick
        
            //todo Scenario tick
        
            //todo World Tick -- Pawns are ticked here. Bypassing for now to test health boi
            _healthDebugPawn?.Tick();
        
            //todo Game End Tick
        
            //todo StoryTeller Tick
        
            //todo Quest Manager Tick
        
            //todo Auto Saver Tick
        }

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

        public void Pause()
        {
            if (currentSpeed != paused)
            {
                TogglePause();
            }
        }

        public void RegisterTicksFor(Thing thing)
        {
            GetTickListFor(thing).Register(thing);
        }
    
        public void UnRegisterTicksFor(Thing thing)
        {
            GetTickListFor(thing).UnRegister(thing);
        }
        
        public void ResetSettlementTicks()
        {
            _settleTick = _numTicks;
        }

        public void SetHealthDebugPawn(Pawn pawn)
        {
            _healthDebugPawn = pawn;
        }

        private TickList GetTickListFor(Thing thing)
        {
            var thingTickerType = thing.template.tickerType;

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


