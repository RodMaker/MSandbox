using UnityEngine;
using PurrNet.StateMachine;
using System.Collections.Generic;
using System;
using PurrNet;

public class RoundRunningState : StateNode<List<PlayerHealth>>
{
    private List<PlayerID> players = new();

    public override void Enter(List<PlayerHealth> data, bool asServer)
    {
        base.Enter(data, asServer);

        if (!asServer)
        {
            return;
        }

        players.Clear();

        foreach (var player in data)
        {
            if (player.owner.HasValue)
            {
                players.Add(player.owner.Value);
            }

            player.OnDeath_Server += OnPlayerDeath;
        }
    }

    private void OnPlayerDeath(PlayerID deadPlayer)
    {
        players.Remove(deadPlayer);

        if (players.Count <= 1)
        {
            machine.Next();
        }
    }
}
