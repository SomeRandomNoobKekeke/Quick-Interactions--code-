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
  public class QuickTalkButton : CUIButton
  {
    public QuickTalkButton() : base()
    {
      BreakSerialization = true;
      this.Append(new CUIComponent()
      {
        Absolute = new CUINullRect(0, 0, 20, 20),
        BackgroundColor = Color.White * 0.5f,
      });

      Padding = new Vector2(20, 0);
      TextAlign = CUIAnchor.CenterLeft;
      AbsoluteMin = new CUINullRect(w: 20, h: 20);
    }

  }
}