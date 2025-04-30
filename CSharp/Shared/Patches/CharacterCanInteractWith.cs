using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using QIDependencyInjection;

using Barotrauma.Abilities;
using Barotrauma.Extensions;
using Barotrauma.IO;
using Barotrauma.Items.Components;
using Barotrauma.Networking;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using System.Diagnostics;
using System.Xml.Linq;
#if SERVER
using System.Text;
#endif

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
          prefix: new HarmonyMethod(typeof(CanInteractWith).GetMethod("Character_CanInteractWith_Replace"))
        );
      }
      catch (Exception e)
      {
        Logger.Log(e);
      }
    }

    public static bool Character_CanInteractWith_Replace(Character __instance, ref bool __result, Character c, float maxDist = 200.0f, bool checkVisibility = true, bool skipDistanceCheck = false)
    {
      if (GhostDetector.Check()) return true;

      if (c == __instance || __instance.Removed || !c.Enabled || !c.CanBeSelected || c.InvisibleTimer > 0.0f)
      {
        __result = false; return false;
      }

      if (!c.CharacterHealth.UseHealthWindow && !c.IsDraggable && (c.onCustomInteract == null || !c.AllowCustomInteract))
      {
        __result = false; return false;
      }

      if (__instance.IsPlayer && c.IsHuman && !c.IsOnPlayerTeam) { __result = true; return false; }

      if (!skipDistanceCheck)
      {
        maxDist = Math.Max(ConvertUnits.ToSimUnits(maxDist), c.AnimController.Collider.GetMaxExtent());
        if (Vector2.DistanceSquared(__instance.SimPosition, c.SimPosition) > maxDist * maxDist &&
            Vector2.DistanceSquared(__instance.SimPosition, c.AnimController.MainLimb.SimPosition) > maxDist * maxDist)
        {
          __result = false; return false;
        }
      }

      __result = !checkVisibility || __instance.CanSeeTarget(c);
      return false;
    }

    public static void Character_CanInteractWith_Postfix(Character __instance, ref bool __result, Item item)
    {
      if (GhostDetector.Check()) return;

      if (!__instance.IsPlayer) return;

      if (item == Fabricators?.OutpostFabricator) __result = true;
      if (item == Fabricators?.OutpostDeconstructor) __result = true;
      if (item == Fabricators?.OutpostMedFabricator) __result = true;

      // if (item == Fabricators?.OutpostFabricator) Logger.Log($"{__instance} {item}");
      // if (item == Fabricators?.OutpostDeconstructor) Logger.Log($"{__instance} {item}");
      // if (item == Fabricators?.OutpostMedFabricator) Logger.Log($"{__instance} {item}");
    }
  }
}