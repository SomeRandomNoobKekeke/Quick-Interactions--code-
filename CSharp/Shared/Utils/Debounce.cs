using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using QIDependencyInjection;

namespace QuickInteractions
{
  // Idk, mb it should be just an extension to LuaCsTimer
  [Singleton]
  public class Debouncer : IDisposable
  {
    public static LuaCsTimer Timer => GameMain.LuaCs.Timer;
    [Dependency] public Logger Logger { get; set; }

    private Dictionary<string, LuaCsTimer.TimedAction> Scheduled = new();

    public void Debounce(string name, int millisecondDelay, Action action)
    {
      LuaCsTimer.TimedAction timedAction = new LuaCsTimer.TimedAction((object[] args) =>
        {
          action();
          Scheduled.Remove(name);
        },
        millisecondDelay
      );

      if (Scheduled.ContainsKey(name))
      {
        Timer.timedActions.Remove(Scheduled[name]);
        Scheduled[name] = timedAction;
        Timer.AddTimer(timedAction);
      }
      else
      {
        Timer.AddTimer(timedAction);
        Scheduled[name] = timedAction;
      }

    }

    public void Dispose()
    {
      foreach (var timedAction in Scheduled.Values)
      {
        Timer.timedActions.Remove(timedAction);
      }
      Scheduled.Clear();
    }
  }
}