using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;

namespace server;

public class Request
{
    // public statics.
    public static int OFFSET  = 14;
    public static int MIN_LEN = 1;
    public const  int CONTENT = 1;
    public const int CAPTION  = 2;
    public const int DATE     = 3;
    public const int TYPE     = 4;
    public const String START = "\n\r";

  
    
    // private instance data.
    private Dictionary<String, String> _requestData;
    private String _boundary = "";
    private String _data;
   
    public Request(String data)
    {
        _data = data;
        _requestData = ParseRequest(data);
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

        Console.WriteLine("CONTENT 0: " + content[0]);
        if(content.Length > 1)
        {
            Console.WriteLine("\tCONTENT 1: " + content[1]);

        }
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
                    request.Add("Boundary", _boundary);
            }

            request.Add(content[0].Trim(), content[1].Trim());
        }
    }



    private void ContentData(String line, Dictionary<String,String> d)
    {
        // adding the name to the dictionary.

        String patternName       = "name=";
        String patternSeparater  = ";";
        Match m                  = Regex.Match(line,patternName);
        MatchCollection sepMatch = Regex.Matches(line,patternSeparater);
        String key               =  line.Substring( (m.Index + patternName.Length + 1), (sepMatch[1].Index - m.Index - patternName.Length - 2));
        String patternFile       = "filename=";
        Match m1                 = Regex.Match(line, patternFile);
        String quoteSeparator    = "\"";
        MatchCollection quotes   = Regex.Matches(line, quoteSeparator);
        String value             = line.Substring( (m1.Index + patternFile.Length +1), (quotes[3].Index - m1.Index - patternFile.Length-1)  );

        d.Add(key, value);

        // adding the content to the dictionary.

        //String end = "\n\r";
        MatchCollection contentBorders = Regex.Matches(line, START);
        Match startOfContent = Regex.Match(line, START); // will find where the content starts
        String content = line.Substring(startOfContent.Index + START.Length + 1 );
        d.Add("Content", content.Substring(0,content.Length - 2));

    }

    private void CaptionData(String line, Dictionary<String, String> d)
    {
        Match captionStart = Regex.Match(line, START);
        String caption = line.Substring(captionStart.Index + START.Length + 1);
        d.Add("Caption",caption.Substring(0, caption.Length - 2));
    }

    private void DateData(String line, Dictionary<String, String> d)
    {
        Match dateStart = Regex.Match(line, START);
        String date = line.Substring(dateStart.Index + START.Length + 1);
        d.Add("Date", date.Substring(0,date.Length - 2));
    }

    private void ProcessMultipart(String MultipartData)
    {
        Dictionary<String, String> MultipartDictionary = new Dictionary<String, String>();

        String[] multiSplit = MultipartData.Split(_boundary);

        Console.WriteLine("------------------------------------------------------------------");
        for (int i = 0; i < multiSplit.Length; i++)
        {
            String line = multiSplit[i];

            /*
             * this splits each chunk of multipart data, the first chucnk will contain nothing, the second will contain the content
             * of the text file as well as the name, the third will contain the caption and the fourth will contain the date
             */

            switch (i)
            {
                case CONTENT:
                    ContentData(line, MultipartDictionary);
                    break;
                case CAPTION:
                    CaptionData(line, MultipartDictionary);
                    break;
                case DATE:
                    DateData(line, MultipartDictionary);
                    break;
                case TYPE:
                    MultipartDictionary.Add("Type", "Submit"); // we know the type.
                    break;

            }

            Console.WriteLine("------------------------------------------------------------------");
        }

        ReconstructFile(MultipartDictionary);
        

    }

    private void ReconstructFile(Dictionary<String, String> filedata)
    {
        List<String> keys = new List<String>(filedata.Keys);
        Console.WriteLine("ALL KEYS IN MULTIPART");
        foreach(String key in keys)
        {
            Console.WriteLine("Key: [" + key + "] Value: [" + filedata[key] +"]");
        }

        try
        {
            String filename = filedata["fileName"]; 
            StreamWriter fileWriter = new StreamWriter($"C:\\Users\\bradl\\Desktop\\C#\\Server-Project-1\\server\\upload\\{filename}");
            fileWriter.Write(filedata["Content"]);
            fileWriter.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }


    }
         

    private Dictionary<String, String> ParseRequest(String data)
    {
        Dictionary<String, String> request = new Dictionary<string, string>();
        String[] lines = data.Split("\n");

        int index = 0;

        for (var i = 0; i < lines.Length; i++)
        {

            if (i == 0)
            {
                String result = lines[i].Contains("GET") ? "Get" : "Post"; // this is the http GET or POST
                request.Add("Request", result);
            } 
            else if (request["Request"].Equals("Post") && lines[i].Equals(_boundary))
            {
                Console.WriteLine("BOUNDARY FOUND");
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