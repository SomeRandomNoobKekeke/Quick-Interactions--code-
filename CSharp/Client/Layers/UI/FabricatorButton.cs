using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using CrabUI;
using QIDependencyInjection;

namespace QuickInteractions
{
  public class FabricatorButton : CUIHorizontalList
  {
    public static Color GetButtonColor(Item item)
    {
      return item.Prefab.Identifier.Value switch
      {
        "fabricator" => new Color(255, 255, 255),
        "medicalfabricator" => new Color(255, 130, 130),
        "deconstructor" => new Color(255, 255, 130),
        _ => new Color(255, 255, 255),
      };
    }

    public static CUISprite GetIcon(Item item)
    {
      return item.Prefab.Identifier.Value switch
      {
        "fabricator" => GetIcon(0, 1),
        "medicalfabricator" => GetIcon(0, 1),
        "deconstructor" => GetIcon(1, 1),
        _ => GetIcon(0, 1),
      };
    }

    public static CUISprite GetIcon(int x, int y)
    {
      return new CUISprite("Interaction icons.png", new Rectangle(x * 34, y * 19, 34, 19));
    }

    public static string GetInteractionText(Item item)
    {
      return $"{item.Prefab.Name}";
    }

    public Item item { get; set; }

    public bool TextVisible
    {
      get => Text.Parent != null;
      set
      {
        if (value)
        {
          if (Text.Parent == null) Append(Text);
        }
        else
        {
          if (Text.Parent != null) RemoveChild(Text);
        }
      }
    }

    public CUIButton Icon;
    public CUITextBlock Text;

    public FabricatorButton(Item item, CUIDirection direction) : base()
    {
      FitContent = new CUIBool2(true, true);
      Direction = direction;

      this["icon"] = Icon = new CUIButton()
      {
        Text = "",
        Border = new CUIBorder(),
        BackgroundSprite = GetIcon(item),
        MasterColorOpaque = GetButtonColor(item),
        ResizeToSprite = true,
      };

      Icon.OnMouseDown += (e) =>
      {
        DispatchUp(new CUICommand("interact", item));
      };

      Text = new CUITextBlock("")
      {
        TextAlign = CUIAnchor.CenterLeft,
        Text = GetInteractionText(item),
      };

      this.item = item;
    }

  }
}