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
  public partial class Fabricators
  {
    [Dependency] public FakeInput FakeInput { get; set; }

    // Dirty, but i don't see another way, this grabbing logic is defined in the middle of https://github.com/evilfactory/LuaCsForBarotrauma/blob/6da26ffa93eb1d94b8fec4add1847879e6b1c75d/Barotrauma/BarotraumaShared/SharedSource/Characters/Animation/HumanoidAnimController.cs#L428
    public void MakeUngrabbable(Item item)
    {
      item.Prefab.GrabWhenSelected = false;
    }

    public void SelectItem(Item item)
    {
      if (Character.Controlled != null)
      {
        Character.Controlled.SelectedItem = item;
        if (GameMain.IsMultiplayer)
        {
          FakeInput.SendInteractPackage(item);
        }
      }
    }
  }
}