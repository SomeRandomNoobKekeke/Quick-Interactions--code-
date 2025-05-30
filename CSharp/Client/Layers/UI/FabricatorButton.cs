using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using QICrabUI;
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

    public static CUISprite GetIcon(int x, int y) => QuickTalkButton.GetIcon(x, y);

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
          Text.Absolute = new CUINullRect(null, null, null, null);
          Text.Ghost = new CUIBool2(false, false);
          Text.Revealed = true;
          //if (Text.Parent == null) Append(Text);
        }
        else
        {
          //if (Text.Parent != null) RemoveChild(Text);
          //Text.GhostText = true;
          Text.Revealed = false;
          Text.Ghost = new CUIBool2(true, false);
          Text.Absolute = new CUINullRect(null, null, null, 0);
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
        Absolute = QuickTalkButton.IconSize,
        //ResizeToSprite = true,
      };

      Icon.OnMouseDown += (e) =>
      {
        DispatchUp(new CUICommand("interact", item));
      };

      this["text"] = Text = new CUITextBlock("")
      {
        TextAlign = CUIAnchor.CenterLeft,
        Text = GetInteractionText(item),
        TextScale = QuickTalkButton.TextScale,
        Revealed = false,
        Ghost = new CUIBool2(true, false),
        Absolute = new CUINullRect(null, null, null, 0),
      };


      this.item = item;
    }

  }
}