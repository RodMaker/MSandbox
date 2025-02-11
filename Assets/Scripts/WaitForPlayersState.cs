using UnityEngine;
using PurrNet.StateMachine;
using System.Collections;

public class WaitForPlayersState : StateNode
{
    [SerializeField] private int minPlayers = 2;

    public override void Enter(bool asServer)
    {
        base.Enter(asServer);

        if (!asServer)
        {
            return;
        }

        StartCoroutine(WaitForPlayers());
    }

    private IEnumerator WaitForPlayers()
    {
        while (networkManager.players.Count < minPlayers)
        {
            yield return null;
        }

        machine.Next();
    }
}
