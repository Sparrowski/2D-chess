using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]private Chessboard chessboard;
    [SerializeField]private UIManager uIManager;

    // Update is called once per frame
    void Update()
    {
        if(chessboard.isWhiteWinner)
            gameOver(TeamPlayer.White);
        else if(chessboard.isBlackWinner)
            gameOver(TeamPlayer.Black);
    }

    public void gameOver(TeamPlayer winner){
        uIManager.ShowGameOverScreen(winner);
    }
}
