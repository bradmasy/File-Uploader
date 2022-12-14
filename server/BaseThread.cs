
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;
abstract class BaseThread{

    protected Thread _thread;

    protected BaseThread()
    {
        Console.WriteLine("here");
        _thread = new Thread(new ThreadStart(Run));
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