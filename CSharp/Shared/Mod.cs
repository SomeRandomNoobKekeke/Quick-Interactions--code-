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
using QICrabUI;
using QIDependencyInjection;

using System.Runtime.CompilerServices;
[assembly: IgnoresAccessChecksTo("Barotrauma")]
[assembly: IgnoresAccessChecksTo("DedicatedServer")]
[assembly: IgnoresAccessChecksTo("BarotraumaCore")]

namespace QuickInteractions
{
  public class PatchClassAttribute : System.Attribute { }
  public partial class Mod : IAssemblyPlugin
  {
    public static string Name = "Quick Interactions";
    public static Harmony Harmony = new Harmony("quick.interactions");

    [EntryPoint] public static Mod Instance { get; set; }

    [Singleton] public Debugger Debugger { get; set; }
    [Singleton] public Logger Logger { get; set; }
    [Singleton] public LogicLevel Logic { get; set; }


    public ModPaths Paths { get; set; }

    public event Action OnPluginLoad;
    public event Action OnPluginUnload;

    public ServiceCollection Services = new ServiceCollection() { Debug = false };

    public void SetupDependencies()
    {

    }

    public void Initialize()
    {
      Instance = this;
      AddCommands();

      Paths = new ModPaths(Name);


#if CLIENT
      CUI.ModDir = Paths.ModDir;
      CUI.AssetsPath = Paths.AssetsFolder;
      CUI.Initialize();
#endif

      SetupDependencies();
      Services.InjectEverything();

      Debugger.Debug = Paths.IsInLocalMods;

      PatchAll();

#if CLIENT
      InitializeClient();
#endif

      OnPluginLoad?.Invoke();

      Logger.Info($"{Name} initialized");
    }

    public void PatchAll()
    {
      Assembly CallingAssembly = Assembly.GetCallingAssembly();

      foreach (Type type in CallingAssembly.GetTypes())
      {
        if (Attribute.IsDefined(type, typeof(PatchClassAttribute)))
        {
          MethodInfo init = type.GetMethod("Initialize", AccessTools.all);
          if (init != null)
          {
            init.Invoke(null, new object[] { });
          }
        }
      }
    }

    public void OnLoadCompleted() { }
    public void PreInitPatching() { }
    public void Dispose()
    {
      OnPluginUnload?.Invoke();
      CUI.Dispose();
      RemoveCommands();

      Mod.Harmony.UnpatchAll(Mod.Harmony.Id);
    }
  }
}