using System;
using System.Collections.Generic;
using Time.TickerTypes;
using World.Things;
using Object = UnityEngine.Object;

namespace Time
{
   public class TickList
   {
      private TickerType _tickerType;
   
      private List<List<Thing>> _things = new List<List<Thing>>();
      private List<Thing> _thingsToRegister = new List<Thing>();
      private List<Thing> _thingsToUnRegister = new List<Thing>();

      private int TickInterval => _tickerType.tickInterval;

      public TickList(TickerType tickerType)
      {
         _tickerType = tickerType;

         for (var i = 0; i < TickInterval; i++)
         {
            _things.Add(new List<Thing>());
         }
      }

      public void Reset()
      {
         foreach (var thingList in _things)
         {
            thingList.Clear();   
         }
      
         _thingsToRegister.Clear();
         _thingsToUnRegister.Clear();
      }

      public void RemoveWhere(Predicate<Thing> predicate)
      {
         foreach (var thingList in _things)
         {
            thingList.RemoveAll(predicate);
         }
      
         _thingsToRegister.RemoveAll(predicate);
         _thingsToUnRegister.RemoveAll(predicate);
      }

      public void Register(Thing thing)
      {
         _thingsToRegister.Add(thing);
      }

      public void UnRegister(Thing thing)
      {
         _thingsToUnRegister.Add(thing);
      }

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

      private void RegisterThings()
      {
         foreach (var thingToRegister in _thingsToRegister)
         {
            BagOf(thingToRegister).Add(thingToRegister);
         }
      
         _thingsToRegister.Clear();
      }

      private void UnRegisterThings()
      {
         foreach (var thingToUnRegister in _thingsToUnRegister)
         {
            BagOf(thingToUnRegister).Remove(thingToUnRegister);
         }
      
         _thingsToUnRegister.Clear();
      }

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
