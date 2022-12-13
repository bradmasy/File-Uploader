
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
abstract class BaseThread{

    private Thread _thread;
    protected Socket _socket;

    protected BaseThread()
    {
    }

    protected BaseThread(Socket sock){
        _thread = new Thread(new ThreadStart(Run));
        _socket = sock;
    }
    
    public abstract void Run();
    public void Start() {
        
        _thread.Start();
    }

    public void Join()
    {
        _thread.Join();
    }

    public bool IsAlive()
    {
        return _thread.IsAlive;
    }
}