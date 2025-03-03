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
    public void CreateUI()
    {
      Absolute = new CUINullRect(0, 0, 300, 300);
      Anchor = CUIAnchor.CenterLeft;
      ResizibleLeft = false;

      this["layout"] = new CUIVerticalList() { Relative = new CUINullRect(0, 0, 1, 1) };

      Logger.Log($"Revealed: {Revealed}");
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
        this["layout"].Append(new QuickTalkButton()
        {
          Text = $"{character}",
        });
      }
    }
  }
}