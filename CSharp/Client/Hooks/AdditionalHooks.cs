using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using EventInput;

namespace QuickInteractions
{
  public static class HarmonyExtensions
  {
    /// <summary>
    /// Patch only if not patched already
    /// </summary>
    public static void EnsurePatch(this Harmony harmony, MethodBase original, MethodInfo prefix = null, MethodInfo postfix = null)
    {
      Patches patches = Harmony.GetPatchInfo(original);
      if (patches != null && patches.Postfixes.Any(patch => patch.owner == harmony.Id)) return;

      harmony.Patch(original,
        prefix is null ? null : new HarmonyMethod(prefix),
        postfix is null ? null : new HarmonyMethod(postfix)
      );
    }
  }

  public class AdditionalHooks : IAssemblyPlugin
  {
    public static string HarmonyID = "AdditionalHooks";
    public static Harmony harmony = new Harmony(HarmonyID);

    public void PatchAll()
    {
#if CLIENT
      // ----------- CUI Patches -----------
      harmony.EnsurePatch(
        original: typeof(GUI).GetMethod("Draw", AccessTools.all),
        postfix: typeof(AdditionalHooks).GetMethod("GUI_Draw_Postfix", AccessTools.all)
      );

      harmony.EnsurePatch(
        original: typeof(GUI).GetMethod("DrawCursor", AccessTools.all),
        prefix: typeof(AdditionalHooks).GetMethod("GUI_DrawCursor_Prefix", AccessTools.all)
      );

      harmony.EnsurePatch(
        original: typeof(Camera).GetMethod("MoveCamera", AccessTools.all),
        prefix: typeof(AdditionalHooks).GetMethod("Camera_MoveCamera_Prefix", AccessTools.all)
      );

      harmony.EnsurePatch(
        original: typeof(KeyboardDispatcher).GetMethod("set_Subscriber", AccessTools.all),
        prefix: typeof(AdditionalHooks).GetMethod("KeyboardDispatcher_set_Subscriber_Prefix", AccessTools.all)
      );

      harmony.EnsurePatch(
        original: typeof(GUI).GetMethod("TogglePauseMenu", AccessTools.all, new Type[] { }),
        postfix: typeof(AdditionalHooks).GetMethod("GUI_TogglePauseMenu_Postfix", AccessTools.all)
      );

      harmony.EnsurePatch(
        original: typeof(GUI).GetMethod("get_InputBlockingMenuOpen", AccessTools.all),
        postfix: typeof(AdditionalHooks).GetMethod("GUI_InputBlockingMenuOpen_Postfix", AccessTools.all)
      );
      // ----------- CUI Patches -----------
#endif
    }

#if CLIENT
    public static void GUI_Draw_Postfix(SpriteBatch spriteBatch)
    {
      GameMain.LuaCs.Hook.Call("GUI_Draw_Postfix", spriteBatch);
    }

    public static void GUI_DrawCursor_Prefix(SpriteBatch spriteBatch)
    {
      GameMain.LuaCs.Hook.Call("GUI_DrawCursor_Prefix", spriteBatch);
    }

    public static void Camera_MoveCamera_Prefix(float deltaTime, ref bool allowMove, ref bool allowZoom, bool allowInput, bool? followSub)
    {
      try
      {
        Dictionary<string, bool> result = (Dictionary<string, bool>)GameMain.LuaCs.Hook.Call("Camera_MoveCamera_Prefix", deltaTime, allowMove, allowZoom, allowInput, followSub);

        if (result == null) return;
        if (result.ContainsKey("allowMove")) allowZoom = result["allowMove"];
        if (result.ContainsKey("allowZoom")) allowZoom = result["allowZoom"];
      }
      catch (Exception e)
      {
        Log($"Hook Camera_MoveCamera_Prefix failed:", Color.Orange);
        Log(e, Color.Orange);
      }
    }

    public static void KeyboardDispatcher_set_Subscriber_Prefix(KeyboardDispatcher __instance, IKeyboardSubscriber value)
    {
      GameMain.LuaCs.Hook.Call("KeyboardDispatcher_set_Subscriber_Prefix", __instance, value);
    }

    public static void GUI_TogglePauseMenu_Postfix()
    {
      GameMain.LuaCs.Hook.Call("GUI_TogglePauseMenu_Postfix");
    }

    public static void GUI_InputBlockingMenuOpen_Postfix(ref bool __result)
    {
      try
      {
        bool result = (bool)GameMain.LuaCs.Hook.Call("GUI_InputBlockingMenuOpen_Postfix");
        __result = __result || result;
      }
      catch (Exception e)
      {
        Log($"Hook GUI_InputBlockingMenuOpen_Postfix failed:", Color.Orange);
        Log(e, Color.Orange);
      }
    }
#endif


    public void Initialize() => PatchAll();
    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose() { }

    public static void Log(object msg, Color? color = null)
    {
      color ??= Color.Cyan;
      LuaCsLogger.LogMessage($"{msg ?? "null"}", color * 0.8f, color);
    }
  }

}