using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class resetAction : MonoBehaviour
{
    public MoveGenerator moveGenerator;
    public void sendResetMove()
    {
        Move move = new Move(0, 255, -101, 10000001, "x:0,y:0,z:0,r:0");
        Debug.Log($"send reset move action: {move}");
        moveGenerator.addToSendingQueue(move);
    }
}
