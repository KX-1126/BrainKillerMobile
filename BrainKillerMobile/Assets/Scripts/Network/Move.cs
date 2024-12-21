using System;
using System.Text;

public class Move
{
    public long Time { get; set; }
    public long Replica { get; set; }
    public long Parent { get; set; }
    public long Child { get; set; }
    public string Meta { get; set; } = "";

    public Move(long time, long replica, long parent, long child, string meta = "")
    {
        Time = time;
        Replica = replica;
        Parent = parent;
        Child = child;
        Meta = meta;
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
        long metaLength = metaBytes.Length; // Changed to long

        byte[] buffer = new byte[sizeof(long) * 5 + metaLength]; // Increased buffer size for 5 longs
        int offset = 0;

        WriteBigEndian(buffer, offset, move.Time);
        offset += sizeof(long);
        WriteBigEndian(buffer, offset, move.Replica);
        offset += sizeof(long);
        WriteBigEndian(buffer, offset, move.Parent);
        offset += sizeof(long);
        WriteBigEndian(buffer, offset, move.Child);
        offset += sizeof(long);
        WriteBigEndian(buffer, offset, metaLength); // Write metaLength as long
        offset += sizeof(long);

        Buffer.BlockCopy(metaBytes, 0, buffer, offset, (int)metaLength); // Cast metaLength to int for BlockCopy

        return buffer;
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
}