using Mirror;
using UnityEngine;
using static ServerMatchLogic;

public class PlayerRoleManager : NetworkBehaviour
{
    public PlayerRole PlayerRole => currentPlayerRole;
    public bool IsVIP => currentPlayerRole == PlayerRole.VIP;
    public bool IsBodyguard => currentPlayerRole == PlayerRole.Bodyguard;
    public bool IsAssassin => currentPlayerRole == PlayerRole.Assassin;

    private MeshRenderer meshRenderer;
    private PlayerUIHandler playerUIHandler;

    private PlayerRole currentPlayerRole = PlayerRole.None;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        playerUIHandler = GetComponent<PlayerUIHandler>();
    }

    [ClientRpc]
    public void SetPlayerRole(PlayerRole playerRole)
    {
        currentPlayerRole = playerRole;

        meshRenderer.material.color = GetPlayerRoleColor(playerRole);

        playerUIHandler.ShowObjectiveText(GetObjectiveTextByRole(playerRole), 3f);
    }

    private string GetObjectiveTextByRole(PlayerRole playerRole)
    {
        return playerRole switch
        {
            ServerMatchLogic.PlayerRole.Assassin => StringConstant.ASSASSIN_OBJECTIVE_TEXT,
            ServerMatchLogic.PlayerRole.Bodyguard => StringConstant.BODYGUARD_OBJECTIVE_TEXT,
            ServerMatchLogic.PlayerRole.VIP => StringConstant.VIP_OBJECTIVE_TEXT,
            _ => string.Empty,
        };
    }

    private Color GetPlayerRoleColor(PlayerRole playerRole)
    {
        return playerRole switch
        {
            ServerMatchLogic.PlayerRole.Assassin => Color.red,
            ServerMatchLogic.PlayerRole.Bodyguard => Color.blue,
            ServerMatchLogic.PlayerRole.VIP => Color.yellow,
            _ => Color.white,
        };
    }

    [ClientRpc]
    public void MatchRoundFinished(bool didWin)
    {
        string resultText = GetPlayerRoundFinishText(didWin);
        playerUIHandler.ShowObjectiveText(resultText, 5f);
    }

    private string GetPlayerRoundFinishText(bool didWin)
    {
        return PlayerRole switch
        {
            PlayerRole.Assassin => didWin ? StringConstant.ASSASSIN_WIN_ROUND_TEXT : StringConstant.ASSASSIN_LOSE_ROUND_TEXT,
            PlayerRole.Bodyguard => didWin ? StringConstant.BODYGUARD_WIN_ROUND_TEXT : StringConstant.BODYGUARD_LOSE_ROUND_TEXT,
            PlayerRole.VIP => didWin ? StringConstant.VIP_WIN_ROUND_TEXT : StringConstant.VIP_LOSE_ROUND_TEXT,
            _ => string.Empty,
        };
    }
}