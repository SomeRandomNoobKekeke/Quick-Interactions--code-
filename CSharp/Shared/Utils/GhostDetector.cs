using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace QuickInteractions
{
  public static class GhostDetector
  {
    public static int MaxDetections = 3;
    public static bool testing = false;
    public static Dictionary<string, int> Detections = new();
    /// <summary>
    /// Keeps link to disposed mod instance  
    /// Don't set if you don't want memory leaks
    /// </summary>
    public static Mod Instance;
    public static bool AmIDead([CallerMemberName] string memberName = "")
    {
      if (testing || Mod.Instance?.Disposed == true)
      {
        if (!Detections.ContainsKey(memberName)) Detections[memberName] = 0;
        Detections[memberName] += 1;

        if (Detections[memberName] <= MaxDetections)
        {
          Mod.Log($"{Mod.Name}:", Color.Yellow);
          Mod.Log($"A proximity detector has detected an invisible creature at {memberName}", Color.Orange);
        }

        return true;
      }

      return false;
    }

  }
}