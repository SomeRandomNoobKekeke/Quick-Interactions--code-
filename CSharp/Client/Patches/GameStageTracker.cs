using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using QIDependencyInjection;

namespace QuickInteractions
{
  [PatchClass]
  public class GameStageTracker
  {
    [Singleton] public static GameStageTracker Instance { get; set; }
    [Dependency] public Debugger Debugger { get; set; }

    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action OnRoundStartOrInitialize;
    public void InvokeOnRoundStartOrInitialize() => OnRoundStartOrInitialize?.Invoke();

    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(GameSession).GetMethod("StartRound", AccessTools.all, new Type[]{
            typeof(LevelData),
            typeof(bool),
            typeof(SubmarineInfo),
            typeof(SubmarineInfo),
          }
        ),
        postfix: new HarmonyMethod(typeof(GameStageTracker).GetMethod("GameSession_StartRound_Postfix"))
      );

      Mod.Harmony.Patch(
        original: typeof(GameSession).GetMethod("EndRound", AccessTools.all),
        postfix: new HarmonyMethod(typeof(GameStageTracker).GetMethod("GameSession_EndRound_Postfix"))
      );
    }

    public static void GameSession_StartRound_Postfix()
    {
      Instance?.OnRoundStart?.Invoke();
      Instance?.OnRoundStartOrInitialize?.Invoke();
    }

    public static void GameSession_EndRound_Postfix()
    {
      Instance?.OnRoundEnd?.Invoke();
    }
  }

}