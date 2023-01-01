using System.Net.Sockets;

namespace server
{
    public interface Servlet
    {

        abstract void DoGet(Response response, Request request);
        abstract  void DoPost(Response response, Request request);

        void SetClient(Socket client);


    }
}

