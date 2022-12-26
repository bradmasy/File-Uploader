using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using System.Drawing;

namespace server;

public class Request
{
    // public statics.
    public static int OFFSET  = 14;
    public static int MIN_LEN = 1;
    public const int CONTENT  = 1;
    public const int CAPTION  = 2;
    public const int DATE     = 3;
    public const int TYPE     = 4;
    public const int OK       = 200;
    public const int ERROR    = 404;
    public const int REDIRECT = 301;
    public const String START = "\n\r";

  
    
    // private instance data.
    private Dictionary<String, String> _requestData;
    private Dictionary<String, String> _multipartData ;
    private String _boundary = "";
    private String _data;
    private int _status = ERROR;
   
    public Request(String data)
    {
        _data          = data;
        _multipartData = new Dictionary<string, string>();
        _requestData   = ParseRequest(data);
    }

    public String GetData()
    {
        return _data;
    }

    public String GetBoundary()
    {
        return _boundary;
    }

    /**
     *  Processes each incoming line that represents a line in the request made.
     */
    private void ProcessLines(String line, Dictionary<String, String> request)
    {
        String[] content = line.Split(":");

        if (content.Length > MIN_LEN) // if there is something to split.
        {
            if (content[0].Equals("User-Agent"))
            {
                if (content[1].Trim() != "command line interface")
                {
                    content[1] = "Browser"; // identify the user agent as browser, we will tweak later.
                }
              
            }
            else if(content[0].Equals("Content-Type")) // to get the boundary.
            {   
                    String[] splitContent = content[1].Split(";");
                    _boundary = "------" + splitContent[1].Substring(OFFSET);
                    request.Add("Boundary", _boundary);
            }

            Console.WriteLine($"adding key: {content[0]} and value: {content[1]}");
            request.Add(content[0].Trim(), content[1].Trim());
        }
    }


    /**
     * Parses the first part of the multipart data to get the content information.
     */
    private void ContentData(String line, Dictionary<String,String> d)
    {

       String[] CONTENT_VALUES = new String[2] { "text/plain", "image/jpeg" };

    // adding the name to the dictionary.

        String patternName       = "name=";
        String patternSeparater  = ";";
        Match m                  = Regex.Match(line,patternName);
        MatchCollection sepMatch = Regex.Matches(line,patternSeparater);
        String key               =  line.Substring( (m.Index + patternName.Length + 1), (sepMatch[1].Index - m.Index - patternName.Length - 2));
        String patternFile       = "filename=";
        Match m1                 = Regex.Match(line, patternFile);
        String quoteSeparator    = "\""; // try to find a better thing here.
        MatchCollection quotes   = Regex.Matches(line, quoteSeparator);
        String value             = line.Substring( (m1.Index + patternFile.Length +1), (quotes[3].Index - m1.Index - patternFile.Length-1)  );
    
        d.Add(key, value);

        // adding the content type.

        String contentType = "";
      
        for(int i = 0; i < CONTENT_VALUES.Length; i++)
        {
            String each = CONTENT_VALUES[i];

            if (Regex.IsMatch(line,each))
            {
                contentType = CONTENT_VALUES[i];
                break;
            }
        }

        d.Add("Content-Type", contentType);

        // adding the content to the dictionary.
        MatchCollection contentBorders = Regex.Matches(line, START);
        Match startOfContent = Regex.Match(line, START); // will find where the content starts
        String content = line.Substring(startOfContent.Index + START.Length + 1 );
        d.Add("Content", content.Substring(0,content.Length));
    }

    /**
     * processes the second part of the multipart data in order to get the caption information.
     */
    private void CaptionData(String line, Dictionary<String, String> d)
    {
        Match captionStart = Regex.Match(line, START);
        String caption = line.Substring(captionStart.Index + START.Length + 1);
        d.Add("Caption",caption.Substring(0, caption.Length - 2));
    }

    /**
     * processes the third part of the multipart data in order to get the date information.
     */
    private void DateData(String line, Dictionary<String, String> d)
    {
        Match dateStart = Regex.Match(line, START);
        String date = line.Substring(dateStart.Index + START.Length + 1);
        d.Add("Date", date.Substring(0,date.Length - 2));
    }

    /**
     * processes the the multipart data into a dictionary for further processing.
     */
    private void ProcessMultipart(String MultipartData)
    {
        Dictionary<String, String> MultipartDictionary = new Dictionary<String, String>();

        String[] multiSplit = MultipartData.Split(_boundary);

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
                    ContentData(line, _multipartData);
                    break;
                case CAPTION:
                    CaptionData(line, _multipartData);
                    break;
                case DATE:
                    DateData(line, _multipartData);
                    break;
                case TYPE:
                    _multipartData.Add("Type", "Submit"); // we know the type.
                    break;

            }

        }
    }

    /**
     * Sets the status of the request for the response.
     */
    private void SetStatus(int status)
    {
        _status = status;
    }

    /**
     * Reconstructs the file from the map.
     */
    public int ReconstructFile(Dictionary<String, String> filedata)
    {
        List<String> keys = new List<String>(filedata.Keys);
        Console.WriteLine("ALL KEYS IN MULTIPART");
        foreach(String key in keys)
        {
            Console.WriteLine("Key: [" + key + "] Value: [" + filedata[key] +"]");
        }

        try
        {
            String filename         = filedata["fileName"];
            String path             = $".\\upload\\{filename}";
            // $"C:\\Users\\bradl\\Desktop\\C#\\Server-Project-1\\server\\upload\\{filename}"
            StreamWriter fileWriter = new StreamWriter(path);

            if (filedata["Content-Type"] == "image/jpeg")
            {
                byte[] imageBytes = Encoding.ASCII.GetBytes(filedata["Content"]);
                Console.WriteLine($"image bytes: {imageBytes.ToString}");
                String converted = Convert.ToBase64String(imageBytes);
                Console.WriteLine($"converted code: {converted}");

                
                
                fileWriter.Write(converted);
                fileWriter.Close();
                SetStatus(OK);

            } else if (filedata["Content-Type"] == "text/plain")
            {
                fileWriter.Write(filedata["Content"]);
                fileWriter.Close();
                SetStatus(OK); // set the status to 200 after a proper upload.
            } else {
                SetStatus(ERROR);
            }
            
           
        }
        catch(Exception e)
        {
            SetStatus(ERROR);
            Console.WriteLine(e.Message);
        }

        return _status;

    }

    /**
     * Gets the multipart data dictionary.
     */
    public Dictionary<String,String> GetMultiData()
    {
        return _multipartData;
    }

    /**
     * Gets the status of the request.
     */
    public int GetStatus()
    {
        return _status;
    }
         
    /**
     * Parses the incoming request.
     */
    private Dictionary<String, String> ParseRequest(String data)
    {
        Dictionary<String, String> request = new Dictionary<string, string>();
        String[] lines = data.Split("\n");
        
        int index = 0;

        for (var i = 0; i < lines.Length; i++)
        {
            Console.WriteLine($"LINE: {lines[i]}");
            if (i == 0)
            {
                String result = lines[i].Contains("GET") ? "Get" : "Post"; // this is the http GET or POST
                request.Add("Request", result);
            } 
            else if (request["Request"].Equals("Post") && lines[i].Equals(_boundary))
            {
                break;
            }
            else
            {
                ProcessLines(lines[i], request);
            }
            index += lines[i].Length;

        }


        if (request.ContainsKey("Request"))
        {
            string type = request["Request"] + " / HTTP / 1.1";


            if (request.ContainsKey("Boundary"))
            {
                String MultipartData = data.Substring(index + type.Length);

                ProcessMultipart(MultipartData);
            }
        }
      

        return request;
    }

    /**
     * Gets the request dictionary containing all of the parses information.
     */
    public Dictionary<String, String> GetRequestMap()
    {
        return _requestData;
    }

    /**
     * Overidden ToString Method.
     */
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