using System.Runtime.InteropServices.JavaScript;

namespace server;

public class Request
{
    private Dictionary<String, String> _requestData;

    public Request(String data)
    {
        _requestData = ParseRequest(data);
    }


    private void ProcessLines(String line, Dictionary<String, String> request)
    {
        String[] content = line.Split(":");
        if (content.Length > 1)
        {
            if (content[0].Equals("User-Agent"))
            {
                content[1] = "Browser";
                request.Add(content[0].Trim(), content[1].Trim());
            }
        }
    }

    private Dictionary<String, String> ParseRequest(String data)
    {
        Dictionary<String, String> request = new Dictionary<string, string>();
        String[] lines = data.Split("\n");

        for (var i = 0; i < lines.Length; i++)
        {
            if (i == 0)
            {
                String result = lines[i].Contains("GET") ? "Get" : "Post"; // this is the http GET or POST
                request.Add("Request", result);
            }
            else
            {
                ProcessLines(lines[i], request);
            }
        }

        return request;
    }

    public Dictionary<String, String> GetRequestMap()
    {
        return _requestData;
    }
}