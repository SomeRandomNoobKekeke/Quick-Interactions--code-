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
  public class MakeOutpostFabricatorsAlwaysInteractable
  {
    [Dependency] public static Debugger Debugger { get; set; }
    [Dependency] public static Fabricators Fabricators { get; set; }

    public static void Initialize()
    {
      Mod.Harmony.Patch(
        original: typeof(Character).GetMethod("CanInteractWith", AccessTools.all, new Type[]{
          typeof(Item),
          typeof(float).MakeByRefType(),
          typeof(bool),
        }),
        postfix: new HarmonyMethod(typeof(MakeOutpostFabricatorsAlwaysInteractable).GetMethod("Character_CanInteractWith_Postfix"))
      );
    }

    public static void Character_CanInteractWith_Postfix(Character __instance, ref bool __result, Item item)
    {
      if (item == Fabricators?.OutpostFabricator) __result = true;
      if (item == Fabricators?.OutpostDeconstructor) __result = true;
      if (item == Fabricators?.OutpostMedFabricator) __result = true;
    }


  }
}