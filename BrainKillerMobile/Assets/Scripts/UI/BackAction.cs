using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackAction : MonoBehaviour
{
    public void backToMainScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Login");
    }
}
