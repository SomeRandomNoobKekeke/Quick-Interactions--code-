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

namespace QuickInteractions
{

  public class HookAttribute : System.Attribute { }

  public class HookConverter : IAssemblyPlugin
  {
    public static string HarmonyID = "AdditionalHooks";
    public static Harmony harmony = new Harmony(HarmonyID);



    public static void GUI_Draw_Postfix(SpriteBatch spriteBatch)
    {
      GameMain.LuaCs.Hook.Call("GUI_Draw_Postfix", spriteBatch);
    }


    public void ConvertPatch(MethodInfo mi)
    {
      Log(mi);

    }

    public void ConvertAll()
    {
      Assembly assembly = Assembly.GetAssembly(typeof(HookConverter));

      foreach (Type T in assembly.GetTypes())
      {
        foreach (MethodInfo mi in T.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (Attribute.IsDefined(mi, typeof(HookAttribute)))
          {
            ConvertPatch(mi);
          }
        }
      }
    }

    public void Initialize()
    {
      if (Harmony.GetPatchInfo(typeof(GUI).GetMethod("Draw", AccessTools.all)).Postfixes.Any(patch => patch.owner == HarmonyID))
      {
        Log("already patched");
        return;
      }

      Log("Not patched");

      // harmony.Patch(
      //   original: typeof(GUI).GetMethod("Draw", AccessTools.all),
      //   postfix: new HarmonyMethod(typeof(AdditionalHooks).GetMethod("GUI_Draw_Postfix", AccessTools.all))
      // );

      ConvertAll();
    }

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