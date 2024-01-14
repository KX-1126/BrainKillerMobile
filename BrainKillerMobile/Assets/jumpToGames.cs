using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum gameType{
    flip = 0,
    rotate = 1,
    memorize = 2,
    shoot = 3,
}

public class jumpToGames : MonoBehaviour
{
    public void jumpToGame(int typenum)
    {
        gameType type = (gameType)typenum;
        switch (type)
        {
            case gameType.flip:
                SceneManager.LoadScene("FlipPuzzle");
                break;
            case gameType.rotate:
                SceneManager.LoadScene("CirclePuzzle");
                break;
            case gameType.memorize:
                SceneManager.LoadScene("MemPuzzle");
                break;
            case gameType.shoot:
                SceneManager.LoadScene("ShootPuzzle");
                break;
        }
    }
}
