using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField]private TextMeshProUGUI victory_text;
    [SerializeField]private GameObject GameOverScreen;

    private void Awake() {
    }


    private void UpdateText(TeamPlayer winner){
        if(winner == TeamPlayer.White)
            victory_text.text = "White won!";
        else 
            victory_text.text = "Black won!";
    }

    public void ShowGameOverScreen(TeamPlayer winner){
        UpdateText(winner);
        GameOverScreen.SetActive(true);
    }


    public void onRestart(){
        SceneManager.LoadScene(0);
    }


}
