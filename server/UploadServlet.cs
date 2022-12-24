using System.Net.Sockets;
using System.Text;

namespace server;

public class UploadServlet: Servlet
{
    private TcpClient _client;
    public UploadServlet()
    {
        _client = new TcpClient();
        Console.WriteLine("Creating upload servlet");
    }

    public void DoGet(Response response, Request request)
    {
        String html = "HTTP/1.1 200 OK\nContent-Type:text/html\nContent-Length: 600\n\n<!DOCTYPE html>\r\n<html>\r\n<head>\r\n<title> File Upload Form</title>\r\n</head>\r\n<body>\r\n<h1>Upload file</h1>\r\n<form id =\"form\" method=\"POST\" action=\"/\" enctype=\"multipart/form-data\">\r\n<input type=\"file\" name=\"fileName\"/><br/><br/>\r\nCaption: <input type =\"text\" name=\"caption\"<br/><br/>\r\n <br/>\nDate : <input type=\"date\" name=\"date\"<br/><br/>\r\n <br/>\n <input id='formBtn' type=\"submit\" name=\"submit\" value=\"Submit\"/>\r\n </form>\r\n</body>\r\n</html>\r\n";

        Byte[] responseStr = Encoding.UTF8.GetBytes(html);
        response.WriteToStream(responseStr);
    }

    public void SetClient(TcpClient client) 
    {
        _client = client;
    }

    private void ProcessPostRequest(Request request)
    {
        Console.WriteLine("WEEEE");

    }

    public void DoPost(Response response, Request request)
    {
       if(request.GetRequestMap() != null && request.GetRequestMap().ContainsKey("Boundary"))
        {
            ProcessPostRequest(request);
        }

        Console.WriteLine("PROCESSING POST REQUEST");        
    }
}