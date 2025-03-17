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
  [PatchClass]
  public class CanInteractWith
  {
    [Dependency] public static Logger Logger { get; set; }
    [Dependency] public static Debugger Debugger { get; set; }
    [Dependency] public static Fabricators Fabricators { get; set; }

    public static void Initialize()
    {
      try
      {
        Mod.Harmony.Patch(
        original: typeof(Character).GetMethod("CanInteractWith", AccessTools.all, new Type[]{
          typeof(Item),
          typeof(float).MakeByRefType(),
          typeof(bool),
        }),
        postfix: new HarmonyMethod(typeof(CanInteractWith).GetMethod("Character_CanInteractWith_Postfix"))
      );

        Mod.Harmony.Patch(
          original: typeof(Character).GetMethod("CanInteractWith", AccessTools.all, new Type[]{
          typeof(Character),
          typeof(float),
          typeof(bool),
          typeof(bool),
          }),
          prefix: new HarmonyMethod(typeof(CanInteractWith).GetMethod("Character_CanInteractWith_Prefix"))
        );
      }
      catch (Exception e)
      {
        Logger.Log(e);
      }
    }

    public static bool Character_CanInteractWith_Prefix(Character __instance, ref bool __result, Character c, float maxDist = 200.0f, bool checkVisibility = true, bool skipDistanceCheck = false)
    {
      if (c == __instance || __instance.Removed || !c.Enabled || !c.CanBeSelected || c.InvisibleTimer > 0.0f)
      {
        __result = false; return false;
      }
      __result = true; return false;
    }

    public static void Character_CanInteractWith_Postfix(Character __instance, ref bool __result, Item item)
    {
      if (item == Fabricators?.OutpostFabricator) __result = true;
      if (item == Fabricators?.OutpostDeconstructor) __result = true;
      if (item == Fabricators?.OutpostMedFabricator) __result = true;

      // if (item == Fabricators?.OutpostFabricator) Logger.Log($"{__instance} {item}");
      // if (item == Fabricators?.OutpostDeconstructor) Logger.Log($"{__instance} {item}");
      // if (item == Fabricators?.OutpostMedFabricator) Logger.Log($"{__instance} {item}");
    }
  }
}