using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class resetAction : MonoBehaviour
{
    public MoveGenerator moveGenerator;
    public imageSelectorDropDown imageSelectorDropDown;

    public SWCRender swcRender;

    public void sendResetMove()
    {
        Move move = new Move(0, 255, -102, 1, "x:0,y:0,z:0,r:0");
        Debug.Log($"send reset move action: {move}");
        moveGenerator.addToSendingQueue(move);
    }

    public void sendSaveMove()
    {
        int imageNumber = imageSelectorDropDown.getCurrentImageName();
        Move move = new Move(0, 255, -101, imageNumber, "x:0,y:0,z:0,r:0");
        Debug.Log($"send reset move action: {move}");
        moveGenerator.addToSendingQueue(move);

        // 导出一份局部坐标的 swc 文件
        string exePath = System.IO.Path.GetDirectoryName(Application.dataPath);
        string outputDir = System.IO.Path.Combine(exePath, "swcOutput");
        if (!System.IO.Directory.Exists(outputDir))
        {
            System.IO.Directory.CreateDirectory(outputDir);
        }
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"tree_img{imageNumber}_{timestamp}.swc";
        string filePath = System.IO.Path.Combine(outputDir, fileName);

        SWC curSWC = swcRender.curSwc;
        Dictionary<int, Node> nodeMap = curSWC.indexNodeMap;
        List<Node> nodes = new List<Node>(nodeMap.Values);

        string swcTitle = "# n type x y z r parent";
        string swcContent = swcTitle + "\n";

        foreach (Node node in nodes)
        {
            // 这里可以根据需要修改节点的坐标和半径等属性
            Vector3 texturePos = Vector3.zero;
            if (node.id == 10000001) {
                texturePos = new Vector3(64, 64, 32);
            } else if (ConnectTwoDots.swcNodeTexturePos.ContainsKey(node.id)) {            
                texturePos = ConnectTwoDots.swcNodeTexturePos[node.id];
            } else {
                continue;
            }
            string line = $"{node.id} {node.type} {texturePos.x} {texturePos.y} {texturePos.z} {1.0} {node.pid}\n";
            swcContent += line;
        }

        // 这里可以写入SWC内容，示例写入空文件
        System.IO.File.WriteAllText(filePath, swcContent);
        Debug.Log($"SWC file saved to: {filePath}");


        // 清空坐标集合
        ConnectTwoDots.swcNodeTexturePos.Clear();
    }
}
