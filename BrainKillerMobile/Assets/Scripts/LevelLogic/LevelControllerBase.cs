using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        throw new System.NotImplementedException();
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
