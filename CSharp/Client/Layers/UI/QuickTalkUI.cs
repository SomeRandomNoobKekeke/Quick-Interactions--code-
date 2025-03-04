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
    [Dependency] public Debugger Debugger { get; set; }
    [Dependency] public QuickTalk QuickTalk { get; set; }
    [Dependency] public Debouncer Debouncer { get; set; }

    private bool textVisible;
    public bool TextVisible
    {
      get => textVisible;
      set
      {
        if (textVisible == value) return;
        textVisible = value;

        BackgroundColor = value ? new Color(0, 0, 0, 100) : Color.Transparent;

        UpdateAnchor();
        this["layout"].Children.ForEach(c =>
        {
          if (c is QuickTalkButton button) button.TextVisible = value;
        });
      }
    }

    public void UpdateAnchor()
    {
      bool onTheLeft = Real.Left < CUI.GameScreenSize.X / 2.0f;
      if (onTheLeft)
      {
        if (Anchor == CUIAnchor.BottomRight)
        {
          Anchor = CUIAnchor.BottomLeft;
          Absolute = Absolute with { Left = Real.Left };
        }
      }
      else
      {
        if (Anchor == CUIAnchor.BottomLeft)
        {
          Anchor = CUIAnchor.BottomRight;
          Absolute = Absolute with { Left = (Real.Left + Real.Width) - CUI.GameScreenSize.X };
        }
      }
    }
    public void CreateUI()
    {
      OutlineColor = Color.Transparent;
      BackgroundColor = Color.Transparent;
      Absolute = new CUINullRect(y: -50);
      Anchor = CUIAnchor.BottomLeft;
      Relative = new CUINullRect(-0.5f, 0);
      Resizible = false;
      FitContent = new CUIBool2(true, true);
      //DragHandle.DragRelative = true;
      DragHandle.OutputRealPos = true;

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
        //UpdateAnchor();
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

      AddCommand("interact", (o) =>
      {
        if (o is Character character)
        {
          QuickTalk.InteractWith(character);
        }
      });
    }

    public void AfterInject()
    {
      Mod.Instance.OnPluginLoad += () => { Revealed = Utils.IsThisAnOutpost; Refresh(); };
      GameStageTracker.OnRoundStart += () => { Revealed = Utils.IsThisAnOutpost; ScheduleRefresh(500); }; // bruh
      GameStageTracker.OnRoundEnd += () => Revealed = false;

      QuickTalk.CharacterStatusUpdated += (c) => Refresh();

      CreateUI();
    }

    public void ScheduleRefresh(int delay = 100)
    {
      Debugger.Log("ScheduleRefresh", DebugLevel.UIRefresh);
      GameMain.LuaCs.Timer.Wait((object[] args) => Refresh(), delay);
    }

    public void Refresh()
    {
      if (!Revealed) return;

      Debouncer.Debounce("refresh", 100, () =>
      {
        if (GUI.DisableHUD)
        {
          ScheduleRefresh(500);
          return;
        }
        Debugger.Log("Refresh", DebugLevel.UIRefresh);

        bool onTheLeft = Real.Left < CUI.GameScreenSize.X / 2.0f;

        this["layout"].RemoveAllChildren();

        foreach (Character character in QuickTalk.WantToTalk)
        {
          this["layout"].Append(new QuickTalkButton(character, onTheLeft ? CUIDirection.Straight : CUIDirection.Reverse));
        }

        foreach (Character character in QuickTalk.Merchants)
        {
          this["layout"].Append(new QuickTalkButton(character, onTheLeft ? CUIDirection.Straight : CUIDirection.Reverse));
        }
      });
    }
  }
}