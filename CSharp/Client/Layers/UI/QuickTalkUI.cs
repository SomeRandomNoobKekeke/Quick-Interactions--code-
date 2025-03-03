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
  public class QuickTalkUI : CUIFrame
  {

    [Dependency] public GameStageTracker GameStageTracker { get; set; }
    [Dependency] public Logger Logger { get; set; }
    [Dependency] public QuickTalk QuickTalk { get; set; }

    private bool textVisible;
    public bool TextVisible
    {
      get => textVisible;
      set
      {
        if (textVisible == value) return;
        textVisible = value;

        this["layout"].Children.ForEach(c =>
        {
          if (c is QuickTalkButton button) button.TextVisible = value;
        });
      }
    }

    public void CreateUI()
    {
      OutlineColor = new Color(0, 0, 0, 200);
      //BackgroundColor = new Color(0, 0, 0, 100);
      Anchor = CUIAnchor.CenterLeft;
      Relative = new CUINullRect(-0.5f, 0);
      Resizible = false;
      FitContent = new CUIBool2(true, true);
      //DragHandle.DragRelative = true;

      this["layout"] = new CUIVerticalList()
      {
        Relative = new CUINullRect(0, 0, 1, 1),
        FitContent = new CUIBool2(true, true),
        Scrollable = true,
      };

      Hydrate();
    }

    public override void Hydrate()
    {
      OnDrag += (x, y) =>
      {
        bool onTheLeft = x < CUI.GameScreenSize.X / 2.0f;

        this["layout"].Children.ForEach(c =>
        {
          if (c is QuickTalkButton button)
          {
            button.Direction = onTheLeft ? CUIDirection.Straight : CUIDirection.Reverse;
          }
        });
      };

      OnMouseOn += (e) => TextVisible = MouseOver;
      OnMouseOff += (e) => TextVisible = MouseOver;
    }

    public void AfterInject()
    {
      Mod.Instance.OnPluginLoad += () => { Revealed = Utils.IsThisAnOutpost; Refresh(); };
      GameStageTracker.OnRoundStart += () => { Revealed = Utils.IsThisAnOutpost; Refresh(); };
      GameStageTracker.OnRoundEnd += () => Revealed = false;

      CreateUI();
    }

    public void Refresh()
    {
      if (!Revealed) return;

      this["layout"].RemoveAllChildren();
      foreach (Character character in QuickTalk.Interactable)
      {
        this["layout"].Append(new QuickTalkButton(character));
      }
    }
  }
}