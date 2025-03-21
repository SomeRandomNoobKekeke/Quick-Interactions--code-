using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using QIDependencyInjection;
namespace QuickInteractions
{
  public static partial class Utils
  {
    [Dependency] public static Logger Logger { get; set; }

    public static bool IsThisAnOutpost => GameMain.GameSession?.GameMode is CampaignMode && Level.IsLoadedFriendlyOutpost;// && Level.Loaded is { Type: LevelData.LevelType.Outpost };
    public static bool IsThisAConnection => Level.Loaded is { Type: LevelData.LevelType.LocationConnection };
    public static bool RoundIsLive => GameMain.GameSession?.IsRunning ?? false;

#if CLIENT
    public static bool IsThisASinglePlayer => GameMain.IsSingleplayer;
    public static bool IsThisASinglePlayerCampaign => GameMain.GameSession?.GameMode is SinglePlayerCampaign;
#endif

    public static void PrintMethodParams(MethodInfo mi)
    {
      foreach (ParameterInfo pi in mi.GetParameters())
      {
        Logger.Log(pi.ParameterType);
      }
    }

  }
}