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
          this["textWrapper"].Absolute = new CUINullRect(null, null, null, null);
          this["textWrapper"].Ghost = new CUIBool2(false, false);
          this["textWrapper"].Revealed = true;
          //if (Text.Parent == null) Append(Text);
        }
        else
        {
          //if (Text.Parent != null) RemoveChild(Text);
          //Text.GhostText = true;
          this["textWrapper"].Revealed = false;
          this["textWrapper"].Ghost = new CUIBool2(true, false);
          this["textWrapper"].Absolute = new CUINullRect(null, null, null, 0);
        }
      }
    }

    public CUIButton Icon;
    public CUITextBlock Text;

    public FabricatorButton(Item item, CUIDirection direction) : base()
    {
      FitContent = new CUIBool2(true, true);
      Direction = direction;

      //To place it in center and prevent rescale
      this["iconWrapper"] = new CUIComponent()
      {
        FitContent = new CUIBool2(true, false),
      };

      this["icon"] = Icon = new CUIButton()
      {
        Text = "",
        Border = new CUIBorder(),
        BackgroundSprite = GetIcon(item),
        MasterColorOpaque = GetButtonColor(item),
        Absolute = QuickTalkButton.IconSize,
        //ResizeToSprite = true,
        Anchor = CUIAnchor.Center,
      };

      Icon.OnMouseDown += (e) =>
      {
        DispatchUp(new CUICommand("interact", item));
      };

      Text = new CUITextBlock("")
      {
        TextAlign = CUIAnchor.CenterLeft,
        Text = GetInteractionText(item),
        TextScale = QuickTalkButton.TextScale,
        Anchor = CUIAnchor.Center,
      };

      this["textWrapper"] = new CUIComponent()
      {
        FitContent = new CUIBool2(true, true),
      };

      this["textWrapper"]["text"] = Text;
      // this["text"] = Text;

      this.item = item;
    }

  }
}