using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveGenerator : MonoBehaviour
{
    UnityAppClientThread clientThread;
    public SWCRender render;
    int childNumber = 1;
    private Queue<Move> sendingQueue = new Queue<Move>();
    int replicaId = 0;

    void Start()
    {
        DataManager dataManager = DataManager.Instance;
        replicaId = dataManager.userId;
        clientThread = new UnityAppClientThread(replicaId, "127.0.0.1", 8080);
        clientThread.Start();
        
    }

    Move generateMoveAction() {
        int rootID = 10000001;
        int t = 0;
        int replica = replicaId;
        int x = Random.Range(-20, 20);
        int y = Random.Range(-20, 20);
        int z = Random.Range(-20, 20);
        int r = Random.Range(-20, 20);
        Move move;
        SWC swc = render.curSwc;
        int randomNodeID = swc.GetRandomIndex();
        if (Random.Range(0, 10) > 5 || randomNodeID == 10000001) {
            if (t < 5) {
                move = new Move(t, replica, rootID, childNumber + replica * 100000, $"x:{x},y:{y},z:{z},r:{r}");
            } else {
                move = new Move(t, replica, swc.GetRandomIndex(), childNumber + replica * 100000, $"x:{x},y:{y},z:{z},r:{r}");
            }
        } else {
            move = new Move(t, replica, swc.GetRandomIndex(), randomNodeID, $"x:{x},y:{y},z:{z},r:{r}");
        }
        childNumber++;
        Debug.Log($"Generated move action: {move}");
        return move;
    }

    public Move generateMoveAction2()
    {
        int rootID = 10000001;
        int t = 0;
        int replica = 255;
        Move move;
        SWC swc = render.curSwc;
        int randomNodeID = swc.GetRandomIndex();
        int sourceNodeID;

        // Determine the source node for the new connection
        if (Random.Range(0, 10) > 5 || randomNodeID == rootID)
        {
            sourceNodeID = (t < 5) ? rootID : swc.GetRandomIndex();
        }
        else
        {
            sourceNodeID = swc.GetRandomIndex();
        }

        Node sourceNode = swc.indexNodeMap.ContainsKey(sourceNodeID) ? swc.indexNodeMap[sourceNodeID] : null;
        Vector3 newPosition;
        float newRadius = Random.Range(0.5f, 1.5f); // Example radius

        if (sourceNode != null)
        {
            Vector3 parentPosition = new Vector3(sourceNode.x, sourceNode.y, sourceNode.z);
            Vector3 direction = Vector3.forward; // Default direction

            if (sourceNode.parent != null)
            {
                Vector3 grandParentPosition = new Vector3(sourceNode.parent.x, sourceNode.parent.y, sourceNode.parent.z);
                direction = (parentPosition - grandParentPosition).normalized;
            }
            else if (sourceNodeID == rootID)
            {
                // If the source is the root, define a default outward direction (can be randomized)
                direction = Random.insideUnitSphere.normalized;
            }

            float baseExtensionLength = Random.Range(20f, 50f); // Base length of the new segment
            float randomOffsetX = Random.Range(-1f, 1f);
            float randomOffsetY = Random.Range(-1f, 1f);
            float randomOffsetZ = Random.Range(-1f, 1f);

            newPosition = parentPosition + direction * baseExtensionLength + new Vector3(randomOffsetX, randomOffsetY, randomOffsetZ);
        }
        else
        {
            // If source node is null (shouldn't happen often if GetRandomIndex is correct), generate random position
            newPosition = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        }

        int targetId = childNumber + replica * 100000;
        move = new Move(t, replica, sourceNodeID, targetId, $"x:{newPosition.x:F2},y:{newPosition.y:F2},z:{newPosition.z:F2},r:{newRadius:F2}");

        childNumber++;
        Debug.Log($"Generated move action: {move}");
        return move;
    }

    public Move generateMove(Vector3 pos, long pid, float scale) {
        // int rootID = 10000001;
        int t = 0;
        int replica = replicaId;
        int childId = childNumber + replica * 100000;
        string meta = $"x:{pos.x},y:{pos.y},z:{pos.z},r:{scale}";
        Move m = new Move(t, replica, pid, childId, meta);
        childNumber++;
        return m;
    }

    public void addToSendingQueue(Move m) {
        sendingQueue.Enqueue(m);
    }

    private void sendMove(Move m) {
        if (clientThread == null) {
            clientThread = new UnityAppClientThread(replicaId, "127.0.0.1", 8080);
            clientThread.Start();
            System.Threading.Thread.Sleep(100);
        }
        clientThread.Send(m);
    }

    private IEnumerator SendQueueContents() {
        while (true) {
            if (sendingQueue.Count > 0) {
                Move move = sendingQueue.Dequeue();
                sendMove(move);
                yield return new WaitForSeconds(0.2f);
            } else {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    void OnEnable() {
        StartCoroutine(SendQueueContents());
    }

    void OnDisable() {
        StopCoroutine(SendQueueContents());
    }

    // public void sendRandomAction() {
    //     if (clientThread == null) {
    //         clientThread = new UnityAppClientThread(255, "127.0.0.1", 8080);
    //         clientThread.Start();
    //     }
    //     Move move = generateMoveAction2();
    //     clientThread.Send(move);
    // }
}
