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
  public class QuickTalkButton : CUIHorizontalList
  {
    public static Color GetButtonColor(Character character)
    {
      if (character.IsDead) return new Color(255, 0, 0);

      return character.CampaignInteractionType switch
      {
        CampaignMode.InteractionType.Talk => new Color(255, 255, 255),
        CampaignMode.InteractionType.Examine => new Color(255, 255, 255),
        CampaignMode.InteractionType.Crew => new Color(198, 211, 242),
        CampaignMode.InteractionType.Store => new Color(206, 162, 138),
        CampaignMode.InteractionType.Upgrade => new Color(106, 250, 115),
        CampaignMode.InteractionType.PurchaseSub => new Color(169, 212, 187),
        CampaignMode.InteractionType.MedicalClinic => new Color(245, 105, 105),
        _ => Color.White,
      };
    }

    public static CUISprite GetIcon(Character character)
    {
      return character.CampaignInteractionType switch
      {
        CampaignMode.InteractionType.Talk => GetIcon(2),
        CampaignMode.InteractionType.Examine => GetIcon(2),
        CampaignMode.InteractionType.Crew => GetIcon(3),
        CampaignMode.InteractionType.Store => GetIcon(0),
        CampaignMode.InteractionType.Upgrade => GetIcon(4),
        CampaignMode.InteractionType.PurchaseSub => GetIcon(5),
        CampaignMode.InteractionType.MedicalClinic => GetIcon(1),
        _ => GetIcon(2),
      };
    }

    public static CUISprite GetIcon(int i)
    {
      return new CUISprite("Interaction icons.png", new Rectangle(i * 34, 0, 34, 19));
    }

    public static string GetInteractionText(Character character)
    {
      string InteractionText = TextManager.Get("CampaignInteraction." + character.CampaignInteractionType).ToString().Replace("[[key]]", "");

      bool isAManager = false;
      if (character.Info?.Job?.Prefab.Identifier.Value is string s)
      {
        if (s.Contains("outpostmanager")) isAManager = true;
      }

      LocalizedString name = character.Info?.DisplayName;
      if (character.Info?.Title != "") name = character.Info?.Title;
      if (isAManager) name = "Outpost Manager";

      return $"{name} - {InteractionText}";
    }

    public Character character { get; set; }

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

    public QuickTalkButton(Character character, CUIDirection direction) : base()
    {
      FitContent = new CUIBool2(true, true);
      Direction = direction;




      this["icon"] = Icon = new CUIButton()
      {
        Text = "",
        Border = new CUIBorder(),
        BackgroundSprite = GetIcon(character),
        MasterColorOpaque = GetButtonColor(character),
        ResizeToSprite = true,
      };

      Icon.OnMouseDown += (e) =>
      {
        DispatchUp(new CUICommand("interact", character));
      };

      Text = new CUITextBlock("")
      {
        TextAlign = CUIAnchor.CenterLeft,
        Text = GetInteractionText(character),
      };

      this.character = character;

    }

  }
}