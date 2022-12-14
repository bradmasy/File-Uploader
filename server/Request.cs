using System.Runtime.InteropServices.JavaScript;

namespace server;

public class Request
{
    private Dictionary<String, String> _requestData;
    private String _boundary = "";
    public Request(String data)
    {
        _requestData = ParseRequest(data);
    }


    private void ProcessLines(String line, Dictionary<String, String> request)
    {
        Console.WriteLine("LINE: " + line);

        String[] content = line.Split(":");
        if (content.Length > 1)
        {
            if (content[0].Equals("User-Agent"))
            {
                content[1] = "Browser";
            }
            else if(content[0].Equals("Content-Type"))
            {
                Console.WriteLine("DATA: " + content[1]);
                String[] splitContent = content[1].Split(";");
                Console.WriteLine("Content 1: " + splitContent[0]);
                Console.WriteLine("Content 2: " + splitContent[1]);
                _boundary = "------"+splitContent[1].Substring(14);
                Console.WriteLine("-------------------BOUNDARY: [" + _boundary + "]");
            }
            request.Add(content[0].Trim(), content[1].Trim());

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
            else if (request["Request"].Equals("Post") && lines[i].Equals(_boundary))
            {
                Console.WriteLine("BREAKING ON THIS");
                
                
                break;
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


    public override string ToString()
    {
        String description = "";
        
        foreach (var each in _requestData)
        {
            description += "Key: " + each.Key + " | Val: " +each.Value +"\n";
            
        }

        return description;
    }
}