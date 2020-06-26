using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;

public class RequestNextTurnIfTeamMembersReadySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        int teamCurrentlyPlaying = CommonReads.GetCurrentTurnTeam(Accessor);
        bool everyoneIsReady = true;

        Entities.ForEach((ref Team team, ref ReadyForNextTurn readyForNextTurn) =>
        {
            // if a team member is NOT ready
            if(team.Value == teamCurrentlyPlaying && !readyForNextTurn.Value)
            {
                everyoneIsReady = false;
            }
        });

        if (everyoneIsReady)
        {
            CommonWrites.RequestNextTurn(Accessor);
        }
    }
}