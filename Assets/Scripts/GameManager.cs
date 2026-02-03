using UnityEngine;



public class GameManager : MonoBehaviour
{
    private PlayerStatus currentPlayerStatus;
    public void SetPlayerStatus(PlayerStatus playerStatus)
    {
        currentPlayerStatus = playerStatus;
    }
    public PlayerStatus GetPlayerStatus()
    {
        return currentPlayerStatus;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentPlayerStatus = PlayerStatus.Pitcher;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
