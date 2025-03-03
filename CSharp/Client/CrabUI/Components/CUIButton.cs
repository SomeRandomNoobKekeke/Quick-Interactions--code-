using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Barotrauma;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Barotrauma.Extensions;
namespace QICrabUI
{
  /// <summary>
  /// A button  
  /// It's derived from CUITextBlock and has all its props
  /// </summary>
  public class CUIButton : CUITextBlock
  {
    [CUISerializable]
    public GUISoundType ClickSound { get; set; } = GUISoundType.Select;
    [CUISerializable] public Color DisabledColor { get; set; }
    [CUISerializable] public Color InactiveColor { get; set; }
    [CUISerializable] public Color MouseOverColor { get; set; }
    [CUISerializable] public Color MousePressedColor { get; set; }

    /// <summary>
    /// Convenient prop to set all colors at once
    /// </summary>
    public Color MasterColor
    {
      set
      {
        InactiveColor = value.Multiply(0.7f);
        MouseOverColor = value.Multiply(0.9f);
        MousePressedColor = value;
      }
    }

    public Color MasterColorOpaque
    {
      set
      {
        InactiveColor = new Color((int)(value.R * 0.7f), (int)(value.G * 0.7f), (int)(value.B * 0.7f), value.A);
        MouseOverColor = new Color((int)(value.R * 0.9f), (int)(value.G * 0.9f), (int)(value.B * 0.9f), value.A);
        MousePressedColor = value;
      }
    }

    /// <summary>
    /// BackgroundColor is used in base.Draw, but here it's calculated from colors above
    /// So it's not a prop anymore, and i don't want to serialize it
    /// </summary>
    public new Color BackgroundColor
    {
      get => CUIProps.BackgroundColor.Value;
      set => CUIProps.BackgroundColor.SetValue(value);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
      if (Disabled)
      {
        BackgroundColor = DisabledColor;
      }
      else
      {
        BackgroundColor = InactiveColor;
        if (MouseOver) BackgroundColor = MouseOverColor;
        if (MousePressed) BackgroundColor = MousePressedColor;
      }
      base.Draw(spriteBatch);
    }
    public CUIButton() : base()
    {
      Text = "CUIButton";
      ConsumeMouseClicks = true;
      ConsumeDragAndDrop = true;
      ConsumeSwipe = true;

      OnMouseDown += (e) =>
      {
        if (!Disabled)
        {
          SoundPlayer.PlayUISound(ClickSound);
          if (Command != null && Command != "")
          {
            DispatchUp(new CUICommand(Command));
          }
        }
      };
    }

    public CUIButton(string text) : this()
    {
      Text = text;
    }


  }
}