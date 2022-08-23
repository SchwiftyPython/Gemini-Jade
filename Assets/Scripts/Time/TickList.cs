using System;
using System.Collections.Generic;
using Time.TickerTypes;
using World.Things;
using Object = UnityEngine.Object;

namespace Time
{
   /// <summary>
   /// The tick list class
   /// </summary>
   public class TickList
   {
      /// <summary>
      /// The ticker type
      /// </summary>
      private TickerType _tickerType;
   
      /// <summary>
      /// The thing
      /// </summary>
      private List<List<Thing>> _things = new List<List<Thing>>();
      /// <summary>
      /// The thing
      /// </summary>
      private List<Thing> _thingsToRegister = new List<Thing>();
      /// <summary>
      /// The thing
      /// </summary>
      private List<Thing> _thingsToUnRegister = new List<Thing>();

      /// <summary>
      /// Gets the value of the tick interval
      /// </summary>
      private int TickInterval => _tickerType.tickInterval;

      /// <summary>
      /// Initializes a new instance of the <see cref="TickList"/> class
      /// </summary>
      /// <param name="tickerType">The ticker type</param>
      public TickList(TickerType tickerType)
      {
         _tickerType = tickerType;

         for (var i = 0; i < TickInterval; i++)
         {
            _things.Add(new List<Thing>());
         }
      }

      /// <summary>
      /// Resets this instance
      /// </summary>
      public void Reset()
      {
         foreach (var thingList in _things)
         {
            thingList.Clear();   
         }
      
         _thingsToRegister.Clear();
         _thingsToUnRegister.Clear();
      }

      /// <summary>
      /// Removes the where using the specified predicate
      /// </summary>
      /// <param name="predicate">The predicate</param>
      public void RemoveWhere(Predicate<Thing> predicate)
      {
         foreach (var thingList in _things)
         {
            thingList.RemoveAll(predicate);
         }
      
         _thingsToRegister.RemoveAll(predicate);
         _thingsToUnRegister.RemoveAll(predicate);
      }

      /// <summary>
      /// Registers the thing
      /// </summary>
      /// <param name="thing">The thing</param>
      public void Register(Thing thing)
      {
         _thingsToRegister.Add(thing);
      }

      /// <summary>
      /// Uns the register using the specified thing
      /// </summary>
      /// <param name="thing">The thing</param>
      public void UnRegister(Thing thing)
      {
         _thingsToUnRegister.Add(thing);
      }

      /// <summary>
      /// Ticks this instance
      /// </summary>
      public void Tick()
      {
         RegisterThings();
      
         UnRegisterThings();

         var tickController = Object.FindObjectOfType<TickController>();

         var thingsToTick = _things[tickController.NumTicks % TickInterval];

         foreach (var thing in thingsToTick)
         {
            TickThing(thing, tickController);
         }
      }

      /// <summary>
      /// Ticks the thing using the specified thing
      /// </summary>
      /// <param name="thing">The thing</param>
      /// <param name="tickController">The tick controller</param>
      private void TickThing(Thing thing, TickController tickController)
      {
         if (thing.Destroyed)
         {
            return;
         }

         if (_tickerType == tickController.normalTick)
         {
            thing.Tick();
         }
         else if (_tickerType == tickController.rareTick)
         {
            thing.TickRare();
         }
         else if (_tickerType == tickController.longTick)
         {
            thing.TickLong();
         }
      }

      /// <summary>
      /// Registers the things
      /// </summary>
      private void RegisterThings()
      {
         foreach (var thingToRegister in _thingsToRegister)
         {
            BagOf(thingToRegister).Add(thingToRegister);
         }
      
         _thingsToRegister.Clear();
      }

      /// <summary>
      /// Uns the register things
      /// </summary>
      private void UnRegisterThings()
      {
         foreach (var thingToUnRegister in _thingsToUnRegister)
         {
            BagOf(thingToUnRegister).Remove(thingToUnRegister);
         }
      
         _thingsToUnRegister.Clear();
      }

      /// <summary>
      /// Bags the of using the specified thing
      /// </summary>
      /// <param name="thing">The thing</param>
      /// <returns>A list of thing</returns>
      private List<Thing> BagOf(Thing thing)
      {
         var hash = thing.GetHashCode();

         if (hash < 0)
         {
            hash *= -1;
         }

         var index = hash % TickInterval;
         return _things[index];
      }
   }
}
