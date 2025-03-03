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
  [Singleton]
  public class QuickTalk
  {
    [Dependency] public Logger Logger { get; set; }
    [Dependency] public CustomInteractionsTracker CustomInteractionsTracker { get; set; }
    public event Action<Character> CharacterStatusUpdated;

    public IEnumerable<Character> Interactable => Character.CharacterList.Where(
      character => character.CampaignInteractionType != CampaignMode.InteractionType.None
    );

    public IEnumerable<Character> WantToTalk => Character.CharacterList.Where(character =>
      character.CampaignInteractionType == CampaignMode.InteractionType.Talk ||
      character.CampaignInteractionType == CampaignMode.InteractionType.Examine
    );

    public IEnumerable<Character> Merchants => Character.CharacterList.Where(character =>
      character.CampaignInteractionType == CampaignMode.InteractionType.Crew ||
      character.CampaignInteractionType == CampaignMode.InteractionType.Store ||
      character.CampaignInteractionType == CampaignMode.InteractionType.Upgrade ||
      character.CampaignInteractionType == CampaignMode.InteractionType.PurchaseSub ||
      character.CampaignInteractionType == CampaignMode.InteractionType.MedicalClinic
    );

    public void InteractWith(Character character)
    {
      if (character == null) return;

      if (character.IsDead)
      {
        CharacterStatusUpdated?.Invoke(character);
        return;
      }

      if (character.onCustomInteract != null)
      {
        character.onCustomInteract(character, Character.Controlled);
        ScheduleCharacterUpdate(character);
      }
      else
      {
        CharacterStatusUpdated?.Invoke(character);
      }
    }

    public void ScheduleCharacterUpdate(Character character, int delay = 200)
    {
      GameMain.LuaCs.Timer.Wait((object[] args) => CharacterStatusUpdated?.Invoke(character), delay);
    }

    public void AfterInject()
    {
      CustomInteractionsTracker.OnCharacterCreated += (c) => ScheduleCharacterUpdate(c);
      CustomInteractionsTracker.OnCharacterKilled += (c) => ScheduleCharacterUpdate(c);
      CustomInteractionsTracker.OnConversationEnded += (c) => ScheduleCharacterUpdate(c);
    }
  }
}