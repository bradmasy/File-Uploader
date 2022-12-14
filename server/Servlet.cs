namespace server;

public interface Servlet
{

    void DoGet(Response response, Request request);
    void DoPost(Response response, Request request);
}