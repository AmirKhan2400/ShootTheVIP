using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class ServerMatchLogic : NetworkSingleton<ServerMatchLogic>
{
    private const int bodyGuardRespawnCount = 3;

    [SerializeField] private List<Transform> assassinTeamSpawnPoints = new List<Transform>();
    [SerializeField] private List<Transform> bodyGuardTeamSpawnPoints = new List<Transform>();
    [SerializeField] private VIPEscapeSpot VIPEscapeSpot;

    private List<NetworkIdentity> assassinTeam;
    private List<NetworkIdentity> bodyguardTeam;
    private NetworkIdentity vipPlayer;

    private int currentAssassinTeamSpawnPointIndex = -1;
    private int currentBodyGuardTeamSpawnPointIndex = -1;

    [SyncVar] private int assassinRoundWon;
    [SyncVar] private int bodyguardRoundWon;

    [Server]
    public void StartMatch()
    {
        SetupTeamMembers();

        PreparePlayers();

        VIPEscapeSpot.Activate();

        VIPEscapeSpot.OnVIPPlayerEscaped += OnVIPPlayerEscaped;

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.Server_OnPlayerDied += GameStateManager_OnPlayerDied; //TODO: need to un sub somewhhere
    }

    private void GameStateManager_OnPlayerDied(NetworkIdentity deadPlayerID)
    {
        if (deadPlayerID.TryGetComponent(out PlayerRoleManager roleManager) && roleManager.IsVIP)
            MatchRoundFinished(false);
    }

    [Server]
    public void RestartMatch()
    {
        SelectVIP();

        PreparePlayers();

        VIPEscapeSpot.Activate();
    }

    [Server]
    private void TeleportPlayerToSpawnPoint(NetworkIdentity player, PlayerRole playerRole)
    {
        Transform spawnPoint;

        if (playerRole == PlayerRole.Assassin)
            spawnPoint = GetNextAssassinSpawnPoint();
        else
            spawnPoint = GetNextBodyguardSpawnPoint();

        player.GetComponent<NetworkTransformReliable>().ServerTeleport(spawnPoint.position, spawnPoint.rotation);
    }

    [Server]
    private void SetPlayerRole(NetworkIdentity player, PlayerRole playerRole)
    {
        if (player.TryGetComponent(out PlayerRoleManager playerRoleManager))
        {
            playerRoleManager.SetPlayerRole(playerRole);
            SetPlayerRespawnCount(player, playerRole);
        }
        else
            Debug.LogError("Failed to find PlayerRoleManager component on player:" + player.name);
    }

    [Server]
    private void SetPlayerRespawnCount(NetworkIdentity player, PlayerRole playerRole)
    {
        if (player.TryGetComponent(out PlayerState playerState))
            playerState.SetRespawnCount(playerRole == PlayerRole.Bodyguard ? bodyGuardRespawnCount : -1);
        else
            Debug.LogError("Failed to find PlayerState component on player:" + player.name);
    }

    [Server]
    public void SendMessageToAllPlayers(string message)
    {
        var players = GameStateManager.Instance.GetPlayerList();

        foreach (var player in players)
        {
            if (player.TryGetComponent(out PlayerUIHandler uIHandler))
            {
                string text = string.Format("<color=#ff0000>Host Said: {0}", message);
                uIHandler.ShowObjectiveTextRPC(text, 2f);
            }
        }
    }

    [Server]
    private void OnVIPPlayerEscaped()
    {
        Debug.Log("VIP Escaped!");

        MatchRoundFinished(true);
    }

    [Server]
    private void MatchRoundFinished(bool didVIPEscaped)
    {
        SendRoundResultToTeam(PlayerRole.Bodyguard, didVIPEscaped);

        SendRoundResultToTeam(PlayerRole.Assassin, !didVIPEscaped);

        UpdateMatchScore(didVIPEscaped);

        Invoke(nameof(RestartMatch), 5f);
    }

    [Server]
    private void UpdateMatchScore(bool didBodyguardWin)
    {
        if (didBodyguardWin)
            bodyguardRoundWon++;
        else
            assassinRoundWon++;

        //notify all players to update the scoreboard
        var playerList = GameStateManager.Instance.GetPlayerList();

        foreach (var player in playerList)
            if (player.TryGetComponent(out PlayerUIHandler uIHandler))
                uIHandler.UpdateScoreboard(assassinRoundWon, bodyguardRoundWon);
    }

    [Server]
    private void SendRoundResultToTeam(PlayerRole team, bool didWin)
    {
        List<NetworkIdentity> targetTeamMembers;

        if (team == PlayerRole.Assassin)
            targetTeamMembers = new List<NetworkIdentity>(assassinTeam);
        else
        {
            targetTeamMembers = new List<NetworkIdentity>(bodyguardTeam)
            {
                vipPlayer
            };
        }

        foreach (var teamMember in targetTeamMembers)
            if (teamMember.TryGetComponent(out PlayerRoleManager playerRoleManager))
                playerRoleManager.MatchRoundFinished(didWin);
    }

    private void SetupTeamMembers()
    {
        List<NetworkIdentity> playerList = GameStateManager.Instance.GetPlayerList();
        playerList.Shuffle();

        int playerHalf = playerList.Count / 2;

        assassinTeam = playerList.GetRange(0, playerHalf);

        bodyguardTeam = playerList.GetRange(playerHalf, playerList.Count - playerHalf);

        SelectVIP();
    }

    private void SelectVIP()
    {
        if (vipPlayer != null)
            bodyguardTeam.Add(vipPlayer);

        vipPlayer = bodyguardTeam[UnityEngine.Random.Range(0, bodyguardTeam.Count)];
        bodyguardTeam.Remove(vipPlayer);
    }

    //teleport players to their spawn point and assign their roles
    private void PreparePlayers()
    {
        foreach (var assassinMember in assassinTeam)
        {
            SetPlayerRole(assassinMember, PlayerRole.Assassin);
            TeleportPlayerToSpawnPoint(assassinMember, PlayerRole.Assassin);
        }

        foreach (var bodyguardMember in bodyguardTeam)
        {
            SetPlayerRole(bodyguardMember, PlayerRole.Bodyguard);
            TeleportPlayerToSpawnPoint(bodyguardMember, PlayerRole.Bodyguard);
        }

        SetPlayerRole(vipPlayer, PlayerRole.VIP);
        TeleportPlayerToSpawnPoint(vipPlayer, PlayerRole.VIP);
    }

    private Transform GetNextAssassinSpawnPoint()
    {
        currentAssassinTeamSpawnPointIndex = (currentAssassinTeamSpawnPointIndex + 1) % assassinTeamSpawnPoints.Count;

        return assassinTeamSpawnPoints[currentAssassinTeamSpawnPointIndex];
    }

    private Transform GetNextBodyguardSpawnPoint()
    {
        currentBodyGuardTeamSpawnPointIndex = (currentBodyGuardTeamSpawnPointIndex + 1) % bodyGuardTeamSpawnPoints.Count;

        return bodyGuardTeamSpawnPoints[currentBodyGuardTeamSpawnPointIndex];
    }

    public enum PlayerRole
    {
        None, Assassin, Bodyguard, VIP
    }
}