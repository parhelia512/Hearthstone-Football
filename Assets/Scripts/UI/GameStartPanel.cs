using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartPanel : MonoBehaviour
{
    public void StartGameClick()
    {
       // SceneManager.LoadScene(1);
        GameManager.Inst.StartGame();
        Active(false);
    }

    public void ShopClick()
    {
        SceneManager.LoadScene(1);
        Active(false);
    }

    public void GameOutClick()
    {
        SceneManager.UnloadScene(0);
        Active(false);
    }

    public void Active(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
