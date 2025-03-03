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
  public class UILayer
  {
    [Singleton] public QuickTalkUI QuickTalkUI { get; set; }

    public void AfterInject()
    {
      CUI.Main.Append(QuickTalkUI);
    }
  }

}