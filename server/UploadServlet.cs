using System.Net.Sockets;
using System.Text;

namespace server
{
    public class UploadServlet : Servlet
    {

        public const String HOME = "HTTP/1.1 200 OK\nContent-Type:text/html\nContent-Length: 600\n\n<!DOCTYPE html>\r\n<html>\r\n<head>\r\n<title> File Upload Form</title>\r\n</head>\r\n<body>\r\n<h1>Upload file</h1>\r\n<form id =\"form\" method=\"POST\" action=\"/\" enctype=\"multipart/form-data\">\r\n<input type=\"file\" name=\"fileName\"/><br/><br/>\r\nCaption: <input type =\"text\" name=\"caption\"<br/><br/>\r\n <br/>\nDate : <input type=\"date\" name=\"date\"<br/><br/>\r\n <br/>\n <input id='formBtn' type=\"submit\" name=\"submit\" value=\"Submit\"/>\r\n </form>\r\n</body>\r\n</html>\r\n";

        private Socket _client;

        public UploadServlet(Socket client)
        {
            Console.WriteLine("Creating upload servlet");

            _client = client;
        }

        public void DoGet(Response response, Request request)
        {
            Byte[] responseStr = Encoding.UTF8.GetBytes(HOME);
            response.WriteToStream(responseStr);
        }

        public void SetClient(Socket client)
        {
            _client = client;
        }

        private void ProcessPostRequest(Response response, Request request)
        {
            int responseCode = request.ReconstructFile(request.GetMultiData());
            Console.WriteLine($"response code: {responseCode}");

            if (responseCode == 200)
            {
                Byte[] responseStr = Encoding.UTF8.GetBytes(HOME);
                response.WriteToStream(responseStr); // directory listing???
            }
        }

        public void DoPost(Response response, Request request)
        {
            Console.WriteLine($"Post request: {request}");
            if (request.GetRequestMap() != null && request.GetRequestMap().ContainsKey("Boundary"))
            {
                ProcessPostRequest(response, request);
            }
        }
    }
}

