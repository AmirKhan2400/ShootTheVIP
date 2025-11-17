using Mirror;

public class PlayerState : NetworkBehaviour
{
    public bool CanRespawn => respawnCountLeft != 0;

    [SyncVar] public int respawnCountLeft = -1;

    [Server]
    public void SetRespawnCount(int respawnCount)
    {
        respawnCountLeft = respawnCount;
    }

    [Server]
    public void ConsumeRespawnCount()
    {
        if (respawnCountLeft == -1)
            return;

        respawnCountLeft--;
    }

    //this method handle respawn point value for texts to show to user.
    //when respawn count is infinite it return false to prevent showing it to user
    public bool GetRespawnCountForText(out int value)
    {
        value = respawnCountLeft;

        return respawnCountLeft > -1;
    }
}