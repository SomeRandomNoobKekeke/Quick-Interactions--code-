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
using Barotrauma.Items.Components;

namespace QuickInteractions
{
  [Singleton]
  public class Fabricators
  {
    [Dependency] public Logger Logger { get; set; }
    [Dependency] public GameStageTracker GameStageTracker { get; set; }

    // Dirty, but i don't see another way, this grabbing logic is defined in the middle of https://github.com/evilfactory/LuaCsForBarotrauma/blob/6da26ffa93eb1d94b8fec4add1847879e6b1c75d/Barotrauma/BarotraumaShared/SharedSource/Characters/Animation/HumanoidAnimController.cs#L428
    public void MakeUngrabbable(Item item)
    {
      item.Prefab.GrabWhenSelected = false;
    }

    private bool searchedThisRound = false;
    // Too lazy to dry it
    public void FindFabricators()
    {
      if (!Utils.RoundIsLive)
      {
        Logger.Warning($"Tried to FindFabricators before round start");
        return;
      }

      if (searchedThisRound) return;
      searchedThisRound = true;

      if (!Utils.IsThisAnOutpost || Level.Loaded.StartOutpost == null) return;

      foreach (Item item in Item.ItemList)
      {
        if (item.Prefab.Identifier.Value == "fabricator")
        {
          if (item.Submarine == Level.Loaded.StartOutpost)
          {
            OutpostFabricator = item;
            MakeUngrabbable(item);
          }
        }

        if (item.Prefab.Identifier.Value == "medicalfabricator")
        {
          if (item.Submarine == Level.Loaded.StartOutpost)
          {
            outpostMedFabricator = item;
            MakeUngrabbable(item);
          }
        }

        if (item.Prefab.Identifier.Value == "deconstructor")
        {
          if (item.Submarine == Level.Loaded.StartOutpost)
          {
            outpostDeconstructor = item;
            MakeUngrabbable(item);
          }
        }
      }
    }

    public List<string> ItemsToFind = new List<string>()
    {
      "fabricator",
      "medicalfabricator",
      "deconstructor",
    };

    private Item outpostDeconstructor;
    public Item OutpostDeconstructor
    {
      get
      {
        FindFabricators();
        return outpostDeconstructor;
      }
      set => outpostDeconstructor = value;
    }

    private Item outpostFabricator;
    public Item OutpostFabricator
    {
      get
      {
        FindFabricators();
        return outpostFabricator;
      }
      set => outpostFabricator = value;
    }

    private Item outpostMedFabricator;
    public Item OutpostMedFabricator
    {
      get
      {
        FindFabricators();
        return outpostMedFabricator;
      }
      set => outpostMedFabricator = value;
    }

    public void AfterInject()
    {
      Mod.Instance.OnPluginLoad += FindFabricators;
      GameStageTracker.OnRoundStart += FindFabricators;
      GameStageTracker.OnRoundEnd += () =>
      {
        searchedThisRound = false;
        OutpostFabricator = null;
        OutpostDeconstructor = null;
      };
    }
  }
}