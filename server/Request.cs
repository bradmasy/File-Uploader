using System.Runtime.InteropServices.JavaScript;

namespace server;

public class Request
{
    // public statics.
    public static int OFFSET = 14;
    public static int MIN_LEN = 1;
    
    // private instance data.
    private Dictionary<String, String> _requestData;
    private String _boundary = "";
    private String _data;
   // private String _type;
   
    public Request(String data)
    {
        _data = data;
        _requestData = ParseRequest(data);
//        _type = _requestData["Request"] + "/ HTTP / 1.1";
    }

    public String GetData()
    {
        return _data;
    }

    public String GetBoundary()
    {
        return _boundary;
    }

    private void ProcessLines(String line, Dictionary<String, String> request)
    {
        Console.WriteLine("LINE: " + line);
        String[] content = line.Split(":");
        Console.WriteLine("CONTENT LEN: " + content.Length);
        if (content.Length > MIN_LEN)
        {
            if (content[0].Equals("User-Agent"))
            {
                content[1] = "Browser";
            }
            else if(content[0].Equals("Content-Type"))
            {
                String[] splitContent = content[1].Split(";");
                _boundary = "------" + splitContent[1].Substring(OFFSET);
                request.Add("Boundary",_boundary);
            }
            request.Add(content[0].Trim(), content[1].Trim());
        }
    }

    private void ProcessMultipart(String MultipartData)
    {
        Console.WriteLine("MULTIPART:" + MultipartData);
        Dictionary<String,String> MultipartDictionary = new Dictionary<String,String>();

        String[] multiSplit = MultipartData.Split(_boundary);

        Console.WriteLine("------------------------------------------------------------------");
        for (int i = 0; i < multiSplit.Length; i++)
        {
            String line = multiSplit[i];

            if(i == 1)
            {
                String[] splitLines = line.Split("\n");

                for(int y = 0; y < splitLines.Length; y++)
                {
                    Console.WriteLine("MULTI LINE: " + splitLines[y]);
                }
            }


            Console.WriteLine("DATA: " + multiSplit[i]);
        }
        Console.WriteLine("------------------------------------------------------------------");


    }

    private Dictionary<String, String> ParseRequest(String data)
    {
        Console.WriteLine("INCOMING DATA");
        Console.WriteLine(data);
        Dictionary<String, String> request = new Dictionary<string, string>();
        String[] lines = data.Split("\n");
        int index = 0;

        for (var i = 0; i < lines.Length; i++)
        {

            if (i == 0)
            {
                String result = lines[i].Contains("GET") ? "Get" : "Post"; // this is the http GET or POST
                Console.WriteLine("Result: " + result);
                request.Add("Request", result);
            } 
            else if (request["Request"].Equals("Post") && lines[i].Equals(_boundary))
            {
                Console.WriteLine("Last Line: " + lines[i]);
                break;
            }
            else
            {

                ProcessLines(lines[i], request);
                
            }
            index += lines[i].Length;

        }


   
        string type = request["Request"] + " / HTTP / 1.1";
        String MultipartData = data.Substring(index + type.Length);        


        if (request.ContainsKey("Boundary"))
        {
            ProcessMultipart(MultipartData);
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