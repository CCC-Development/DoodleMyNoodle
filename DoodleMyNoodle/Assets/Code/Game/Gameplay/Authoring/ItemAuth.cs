﻿using Unity.Entities;
using UnityEngine;
using System.Linq;
using System;
using CCC.InspectorDisplay;
using System.Collections.Generic;
using UnityEngineX.InspectorDisplay;
using UnityEngineX;
using System.Reflection;

[DisallowMultipleComponent]
public class ItemAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    // SIMULATION

    // Game Action
    public string Value;

    [SerializeReference]
    [AlwaysExpand]
    public List<GameActionSettingAuthBase> GameActionSettings = new List<GameActionSettingAuthBase>();

    public int ApCost = 1;

    public bool IsStackable = true;

    public enum CooldownMode
    {
        NoCooldown,
        Seconds,
        Turns
    }

    public CooldownMode CooldownType = CooldownMode.NoCooldown;
    public fix CooldownDuration = 1;

    public bool HideInInventory = false;

    public bool CanBeUsedAtAnytime = false;

    public bool HasCooldown => CooldownType != CooldownMode.NoCooldown;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var gameAction = GameActionBank.GetAction(Value);
        dstManager.AddComponentData(entity, GameActionBank.GetActionId(gameAction));

        // Update list of GameActionSettingAuth by adding any missing instances and removing any extra
        {
            var requiredSettingAuths = GameActionSettingAuthBase.GetRequiredSettingAuthTypes(gameAction.GetType());
            GameActionSettings.RemoveAll(authInstance => !requiredSettingAuths.Contains(authInstance.GetType()));
            foreach (var authType in requiredSettingAuths)
            {
                if (!GameActionSettings.Any(x => x.GetType() == authType))
                {
                    GameActionSettings.Add(Activator.CreateInstance(authType) as GameActionSettingAuthBase);
                }
            }
        }

        // Convert all GameActionSettingAuths
        foreach (GameActionSettingAuthBase setting in GameActionSettings)
        {
            setting.Context = gameObject;
            setting.Convert(entity, dstManager, conversionSystem);
        }

        if (Animation != null)
        {
            dstManager.AddComponentData(entity, new GameActionSettingAnimationType() { AnimationType = (int)Animation.AnimationType, Duration = (fix)Animation.Duration });
        }

        if (CooldownType == CooldownMode.Seconds)
        {
            dstManager.AddComponentData(entity, new ItemTimeCooldownData() { Value = CooldownDuration });
        }
        else if (CooldownType == CooldownMode.Turns)
        {
            dstManager.AddComponentData(entity, new ItemTurnCooldownData() { Value = fixMath.roundToInt(CooldownDuration) });
        }

        dstManager.AddComponentData(entity, new GameActionSettingAPCost() { Value = ApCost });
        dstManager.AddComponentData(entity, new StackableFlag() { Value = IsStackable });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (GameActionSettingAuthBase settings in GameActionSettings)
        {
            settings.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    // PRESENTATION

    // Description
    public Sprite Icon;
    public string Name;
    public string EffectDescription;
    public AudioPlayable SfxOnUse;

    public bool PlayAnimation = false;
    public AnimationDefinition Animation;

    // Surveys
    public List<SurveyBaseController> CustomSurveys;

    public SurveyBaseController FindCustomSurveyPrefabForParameters(params GameAction.ParameterDescription[] parameters)
    {
        if (parameters.Length == 0)
        {
            return null;
        }

        // example: 3 parameters
        // try find survey for all 3 params
        // then try find survey for 2 params
        // then try find survey for 1 params
        // return null

        SurveyBaseController result = null;
        for (int i = parameters.Length; i > 0; i--)
        {
            result = TryFindCustomSurveyPrefabForParametersSubset(parameters, i);
            if (result != null)
                break;
        }

        return result;
    }

    private SurveyBaseController TryFindCustomSurveyPrefabForParametersSubset(GameAction.ParameterDescription[] parameters, int paramCount)
    {
        foreach (SurveyBaseController survey in CustomSurveys)
        {
            bool hasAllTypes = false;

            for (int i = 0; i < paramCount; i++)
            {
                if (Array.IndexOf(survey.ExpectedQuery, parameters[i].GetParameterDescriptionType()) != -1)
                {
                    hasAllTypes = true;
                }
                else
                {
                    hasAllTypes = false;
                    break;
                }
            }

            if (hasAllTypes)
            {
                return survey;
            }
        }

        return null;
    }
}