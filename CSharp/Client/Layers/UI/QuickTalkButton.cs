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
  public class QuickTalkButton : CUICompositeButton
  {
    public static Color GetButtonColor(Character character)
    {
      return character.CampaignInteractionType switch
      {
        CampaignMode.InteractionType.Talk => new Color(200, 200, 200),
        CampaignMode.InteractionType.Examine => new Color(200, 200, 200),
        CampaignMode.InteractionType.Crew => new Color(0, 128, 128),
        CampaignMode.InteractionType.Store => new Color(128, 128, 0),
        CampaignMode.InteractionType.Upgrade => new Color(0, 128, 0),
        CampaignMode.InteractionType.PurchaseSub => new Color(128, 128, 128),
        CampaignMode.InteractionType.MedicalClinic => new Color(200, 0, 0),
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

    public Character character { get; set; }

    public string Text
    {
      get => this.Get<CUITextBlock>("text").Text;
      set => this.Get<CUITextBlock>("text").Text = value;
    }

    public QuickTalkButton(Character character) : base()
    {
      FitContent = new CUIBool2(true, true);

      Layout = new CUILayoutHorizontalList();

      this.Append(new CUIButton()
      {
        Text = "",
        Border = new CUIBorder(),
        BackgroundSprite = GetIcon(character),
        ResizeToSprite = true,
      });
      this["text"] = new CUITextBlock("bebebebeb")
      {
        FillEmptySpace = new CUIBool2(true, false),
        TextAlign = CUIAnchor.CenterLeft,
        Border = new CUIBorder(),
      };

      this.character = character;
      MasterColorOpaque = GetButtonColor(character);
    }

  }
}