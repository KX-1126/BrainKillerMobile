using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class jumpToGames : MonoBehaviour
{
    public void jumpToGame(int typenum)
    {
        switch (typenum)
        {
            case 0:
                SceneManager.LoadScene("FlipPuzzle");
                break;
            case 1:
                SceneManager.LoadScene("CompareNext");
                break;
            case 2:
                SceneManager.LoadScene("MatchCards");
                break;
            case 3:
                SceneManager.LoadScene("ImageDetective");
                break;
        }
    }
}
