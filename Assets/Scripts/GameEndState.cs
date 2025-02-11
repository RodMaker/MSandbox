using UnityEngine;
using PurrNet.StateMachine;
using System.Collections.Generic;
using PurrNet;
using System.Linq;

public class GameEndState : StateNode
{
    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!InstanceHandler.TryGetInstance(out ScoreManager scoreManager))
        {
            Debug.LogError($"GameEndState failed to get scoremanager!", this);
            return;
        }

        var winner = scoreManager.GetWinner();

        if (winner == default)
        {
            Debug.LogError($"GameEndState failed to get winner!", this);
            return;
        }

        if (!InstanceHandler.TryGetInstance(out EndGameView endGameView))
        {
            Debug.LogError($"GameEndState failed to get end game view!", this);
            return;
        }

        if (!InstanceHandler.TryGetInstance(out GameViewManager gameViewManager))
        {
            Debug.LogError($"GameEndState failed to get game view manager!", this);
            return;
        }

        endGameView.SetWinner(winner);
        gameViewManager.ShowView<EndGameView>();
        Debug.Log($"Game has now ended with {winner} as our champion!");
    }
}
