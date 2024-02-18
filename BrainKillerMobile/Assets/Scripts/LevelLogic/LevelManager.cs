using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public virtual void InitLevel(int levelId)
    {
        throw  new System.NotImplementedException();
    }
    
    // init mode
    public virtual void InitMode(string modeName)
    {
        throw  new System.NotImplementedException();
    }

    public virtual void returnToLobby()
    {
        
    }

    public virtual void nextLevel()
    {
        
    }
}
