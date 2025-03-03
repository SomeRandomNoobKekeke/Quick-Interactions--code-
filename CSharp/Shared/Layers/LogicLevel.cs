using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.IO;

using Barotrauma;
using HarmonyLib;
using Microsoft.Xna.Framework;
using QIDependencyInjection;

namespace QuickInteractions
{
  public class LogicLevel
  {
    [Singleton] public QuickTalk QuickTalk { get; set; }
  }
}