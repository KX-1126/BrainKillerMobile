using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Debug = UnityEngine.Debug;

public class UnityAppClientThread
{
    private int id;
    private string ip;
    private int port;
    private Socket socket;
    private Thread thread;
    private volatile bool isRunning = false;

    public UnityAppClientThread(int id, string ip, int port)
    {
        this.id = id;
        this.ip = ip;
        this.port = port;
    }

    public bool IsConnected()
    {
        return socket != null && socket.Connected;
    }

    public void Start()
    {
        if (thread == null)
        {
            isRunning = true;
            Debug.Log($"Client {id}: Try to Connecte to {ip}:{port}");
            thread = new Thread(ConnectAndRun);
            thread.IsBackground = true; // Allow the application to exit even if this thread is running
            thread.Start();
        }
    }

    private void ConnectAndRun()
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
            Debug.Log($"Client {id}: Connected to {ip}:{port}");

            // You can add a receive loop here if needed for two-way communication
            // while (isRunning)
            // {
            //     // Receive data
            // }
        }
        catch (SocketException e)
        {
            Debug.LogError($"Client {id}: Socket connection error: {e.Message}");
            // Optionally, attempt reconnection after a delay
        }
    }

    public void Send(Move move)
    {
        if (!IsConnected())
        {
            Debug.LogWarning($"Client {id}: Socket not connected, cannot send move.");
            return;
        }
        try
        {
            byte[] encodedMove = MoveEncoder.EncodeMove(move);
            Debug.Log($"Client {id}: Sending Move: {move}, encoded size: {encodedMove.Length}");
            socket.Send(encodedMove);
        }
        catch (SocketException e)
        {
            Debug.LogError($"Client {id}: Failed to send: {e.Message}");
            // Handle potential disconnection scenarios here
        }
    }

    public void Stop()
    {
        isRunning = false;
        if (socket != null && socket.Connected)
        {
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Debug.Log($"Client {id}: Socket closed.");
            }
            catch (SocketException e)
            {
                Debug.LogError($"Client {id}: Error closing socket: {e.Message}");
            }
        }
        if (thread != null && thread.IsAlive)
        {
            // Consider a more graceful shutdown mechanism if needed
            thread.Join();
        }
    }
}