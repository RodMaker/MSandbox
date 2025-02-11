using UnityEngine;
using PurrNet.StateMachine;
using System.Collections;
using System.Collections.Generic;
using PurrNet;

public class RoundEndState : StateNode<PlayerID>
{
    [SerializeField] private int amountOfRounds = 3;
    [SerializeField] private StateNode spawningState;

    private int roundCount = 0;
    private WaitForSeconds delay = new(3f);

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
        {
            return;
        }

        CheckForGameEnd();
    }

    private void CheckForGameEnd()
    {
        roundCount++;

        if (roundCount >= amountOfRounds)
        {
            machine.Next();
            return;
        }

        StartCoroutine(DelayNextState());
    }

    private IEnumerator DelayNextState()
    {
        yield return delay;
        machine.SetState(spawningState);
    }
}
