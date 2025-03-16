using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace QuickInteractions
{
  public class AdditionalHooks
  {
    [Hook]
    public static void GUI_Draw_Postfix(SpriteBatch spriteBatch)
    {
      GameMain.LuaCs.Hook.Call("GUI_Draw_Postfix", spriteBatch);
    }
  }
}