using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;

public class addMoveAction : MonoBehaviour
{
    UnityAppClientThread clientThread;
    // Start is called before the first frame update
    void Start()
    {
        clientThread = new UnityAppClientThread(255, "127.0.0.1", 8080);
        clientThread.Start();
    }

    Move generateMoveAction() {
        int rootID = 10000001;
        int t = 0;
        int replica = 0;
        int x = Random.Range(1, 100);
        int y = Random.Range(1, 100);
        int z = Random.Range(1, 100);
        int r = Random.Range(1, 100);
        Move move = new Move(t, replica, rootID, t, $"x:{x},y:{y},z:{z},r:{r}");
        Debug.Log($"Generated move action: {move}");
        return move;
    }

    public void sendRandomAction() {
        Move move = generateMoveAction();
        clientThread.Send(move);
    }
}
