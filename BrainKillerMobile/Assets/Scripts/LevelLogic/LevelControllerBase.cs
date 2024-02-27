using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControllerBase : MonoBehaviour
{
    public virtual void InitLevel()
    {
        throw new System.NotImplementedException();
    }
    
    public virtual void JudgeLevel()
    {
        throw new System.NotImplementedException();
    }
    
    public virtual void returnToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    public virtual void retry()
    {
        throw new System.NotImplementedException();
    }

    public virtual void nextLevel()
    {
        throw new System.NotImplementedException();
    }
    
    
}
