using System;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
public class Move
{
    public long Time { get; set; }
    public int Replica { get; set; }
    public int Parent { get; set; }
    public int Child { get; set; }
    public string Meta { get; set; } = "";

    public Move(long time, int replica, int parent, int child, string meta = "")
    {
        Time = time; // 同步需要，显示不需要
        Replica = replica; // 同步需要，显示不需要
        Parent = parent; // 都需要
        Child = child; // 都需要
        Meta = meta; // 同步不需要，显示需要
    }

    public override string ToString()
    {
        return $"{Time},{Replica},{Parent},{Child},{Meta}";
    }
}

public static class MoveEncoder
{
    public static byte[] EncodeMove(Move move)
    {
        byte[] metaBytes = Encoding.UTF8.GetBytes(move.Meta);
        Debug.Log("content of meta string: " + move.Meta);
        Debug.Log("length of metaBytes: " + metaBytes.Length);
        int metaLength = metaBytes.Length; 

        byte[] dataPartBuffer = new byte[sizeof(long)  + sizeof(int)* 4 + metaLength]; 
        int offset = 0;

        WriteBigEndian(dataPartBuffer, offset, move.Time);
        offset += sizeof(long);
        WriteBigEndian(dataPartBuffer, offset, move.Replica);
        offset += sizeof(int);
        WriteBigEndian(dataPartBuffer, offset, move.Parent);
        offset += sizeof(int);
        WriteBigEndian(dataPartBuffer, offset, move.Child);
        offset += sizeof(int);
        WriteBigEndian(dataPartBuffer, offset, metaLength); 
        offset += sizeof(int);

        Buffer.BlockCopy(metaBytes, 0, dataPartBuffer, offset, metaLength); 

        // 定义魔数：0xDE 0xAD 0xBE 0xEF（4字节）
        byte[] magicNumber = { 0xDE, 0xAD, 0xBE, 0xEF };

        // 创建最终缓冲区（数据 + 魔数）
        byte[] finalBuffer = new byte[dataPartBuffer.Length + magicNumber.Length];
        
        // 复制数据部分
        Buffer.BlockCopy(dataPartBuffer, 0, finalBuffer, 0, dataPartBuffer.Length);
        
        // 追加魔数到末尾
        Buffer.BlockCopy(magicNumber, 0, finalBuffer, dataPartBuffer.Length, magicNumber.Length);
        Debug.Log("length of finalBuffer: " + finalBuffer.Length);
        Debug.Log("content of finalBuffer: " + finalBuffer);
        return finalBuffer;
    }

    private static void WriteBigEndian(byte[] buffer, int offset, long value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        Buffer.BlockCopy(bytes, 0, buffer, offset, sizeof(long));
    }

    private static void WriteBigEndian(byte[] buffer, int offset, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }
        Buffer.BlockCopy(bytes, 0, buffer, offset, sizeof(int));
    }
}