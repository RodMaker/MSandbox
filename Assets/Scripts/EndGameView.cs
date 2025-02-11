using PurrNet;
using System.Collections;
using TMPro;
using UnityEngine;

public class EndGameView : View
{
    [SerializeField] private TMP_Text winnerText;

    private void Awake()
    {
        InstanceHandler.RegisterInstance(this);
    }

    private void OnDestroy()
    {
        InstanceHandler.UnregisterInstance<EndGameView>();
    }

    public void SetWinner(PlayerID winner)
    {
        winnerText.text = $"Player {winner.id} has won the game!";
    }

    public override void OnShow()
    {
        
    }

    public override void OnHide()
    {
        
    }
}
