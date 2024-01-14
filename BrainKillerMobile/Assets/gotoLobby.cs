using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gotoLobby : MonoBehaviour
{
    public void goToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }
}
