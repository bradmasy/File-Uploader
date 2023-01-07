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
    public static int OFFSET = 14;
    public static int MIN_LEN = 1;
    public const int CONTENT = 1;
    public const int CAPTION = 2;
    public const int DATE = 3;
    public const int TYPE = 4;
    public const int OK = 200;
    public const int ERROR = 404;
    public const int REDIRECT = 301;
    public const String START = "\r\n";
    public const String DELIMETER = "\r\n";
    public const int BOUNDARY_LENGTH = 38;
    private const int CONTENT_DISPOSITION = 0;
    private const int CONTENT_TYPE = 1;
    private const int FILENAME_INDEX = 2;
    private const int FILENAME_VALUE = 1;
    private const int CONTENT_TYPE_VALUE = 1;
    private const int CONTENT_HEADER_INFORMATION = 0;
    private const int CONTENT_BODY = 1;
    private const int CAPTION_DATA = 1;



    // private instance data.
    public Dictionary<String, String> _requestData;
    private Dictionary<String, String> _multipartData;
    private String _boundary = "";
    private String _data;
    public int _status = ERROR;

    public Request(String data)
    {
        _data = data;
        _multipartData = new Dictionary<string, string>();
        _requestData = Parse_Request(data);
    }

    public String GetData()
    {
        return _data;
    }

    public String GetBoundary()
    {
        return _boundary;
    }




    private void Process_Content(string request_line, Dictionary<String, String> request_data)
    {
        String[] split_request_line = request_line.Split(":");

        if (split_request_line.Length > 1)
        {
            string key = split_request_line[0].Trim();
            string value = split_request_line[1].Trim();

            if (Regex.IsMatch(split_request_line[0].Trim(), "User-Agent"))
            {
                if (value != "CLI")
                {
                    value = "Browser";

                }
            }

            if (Regex.IsMatch(split_request_line[0], "Content-Type"))
            {

                string boundary = @"\w*----WebKitFormBoundary\w*[a-zA-Z0-9]{16}"; // regex for the boundary, specifically the last boundary of the incoming data.
                int boundary_location = Regex.Match(split_request_line[1], boundary).Index;
                _boundary = "--" + split_request_line[1].Substring(boundary_location, BOUNDARY_LENGTH);
                request_data.Add("Boundary", _boundary);
            }

            request_data.Add(key, value);
        }
    }

    private Dictionary<string, string> Parse_Request(string data)
    {
        Dictionary<String, String> request_data = new Dictionary<String, String>();
        String[] request_lines = data.Split(START);
        string get_pattern = "GET";
        string http_string = "HTTP/1.1";

        foreach (string line in request_lines)
        {

            //Console.WriteLine($"each line: {line}");

            if (Regex.IsMatch(line, http_string))
            {
                string value = Regex.IsMatch(line, get_pattern) ? "GET" : "POST";
                request_data.Add("Request", value);
            }
            else
            {
                Process_Content(line, request_data);
            }

        }

        return request_data;
    }


    private void Parse_Content_Type_Data(string data, Dictionary<String, String> multipart_dictionary)
    {
        String[] split_content_type = data.Split(":");
        multipart_dictionary.Add("Content-Type", split_content_type[CONTENT_TYPE_VALUE].Trim());
    }
    private void Parse_Content_Disposition_Data(string data, Dictionary<String, String> multipart_dictionary)
    {
        String[] split_content_disposition = data.Split(";");
        String[] filename_info = split_content_disposition[FILENAME_INDEX].Split("=");
        string filename_value = filename_info[FILENAME_VALUE].Substring(1, filename_info[FILENAME_VALUE].Length - 2);  // name of the file
        multipart_dictionary.Add("Filename", filename_value);
    }

    private void Parse_Content_Data(string data, Dictionary<String, String> multipart_dictionary)
    {
        String[] split_data = data.Split(START);

        for (int i = 0; i < split_data.Length; i++)
        {
            switch (i)
            {
                case CONTENT_DISPOSITION:
                    // Content disposition
                    Parse_Content_Disposition_Data(split_data[i], multipart_dictionary);
                    break;
                case CONTENT_TYPE:
                    // Content type
                    Parse_Content_Type_Data(split_data[i], multipart_dictionary);
                    break;
            }

        }
    }
    private void Content_Data(string multipart_chunk, Dictionary<String, String> multipart_dictionary)
    {
        String[] split_chunk = multipart_chunk.Split("\r\n\r\n");

        for (int i = 0; i < split_chunk.Length; i++)
        {
            // Console.WriteLine($"each line: [{split_chunk[i]}]");

            switch (i)
            {
                case CONTENT_HEADER_INFORMATION:
                    Parse_Content_Data(split_chunk[i], multipart_dictionary);
                    // all the information about the content
                    break;
                case CONTENT_BODY:
                    // the actual content either text or the image bytecode 
                    multipart_dictionary.Add("Content", split_chunk[i]);
                    // Create_File();
                    break;
            }
        }

    }

    private void Caption_Data(string multipart_chunk, Dictionary<String, String> multipart_dictionary)
    {
        String[] split_chunk = multipart_chunk.Split("\r\n\r\n");

        multipart_dictionary.Add("Caption", split_chunk[CAPTION_DATA]);
    }


    private void Date_Data(string multipart_chunk, Dictionary<String, String> multipart_dictionary)
    {
        String[] split_chunk = multipart_chunk.Split("\r\n\r\n");

        multipart_dictionary.Add("Date", split_chunk[CAPTION_DATA]);
    }

    private int Create_File()
    {
        String filename = _multipartData["Filename"];
        String timestamp = GetTimestamp(DateTime.Now);
        String path = Directory.GetCurrentDirectory() + $"\\upload\\{timestamp}-{filename}";

        if (_multipartData["Content-Type"] == "image/png")
        {
            //Console.WriteLine($"is little endian: {BitConverter.IsLittleEndian}");

            byte[] imageBytes = Encoding.UTF8.GetBytes(_multipartData["Content"]); // gets actual content
            string imageBase64 = Convert.ToBase64String(imageBytes);
            // File.WriteAllBytes(path, imageBytes); // write to file
            File.WriteAllText(path, imageBase64);
            SetStatus(OK);
        }
        else if (_multipartData["Content-Type"] == "text/plain")
        {
            File.WriteAllText(path, _multipartData["Content"]);
            SetStatus(OK); // set the status to 200 after a proper upload.
        }
        else
        {
            SetStatus(ERROR);
            return ERROR;
        }

        Console.WriteLine("WROTE TO FILE");
        return OK;
    }





    public void Parse_Multipart_Data(string multipart_data)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("\r\n").Append(multipart_data); // so that we can split the data by the boundary, first one is missing the /r/n so it is appended for ease.
        string multipart = sb.ToString();

        String[] multipart_chunks = multipart.Split("\r\n" + _boundary + "\r\n");


        for (int i = 0; i < multipart_chunks.Length; i++)
        {
            switch (i)
            {
                case CONTENT:
                    Content_Data(multipart_chunks[i], _multipartData); // finished
                    break;
                case CAPTION:
                    Caption_Data(multipart_chunks[i], _multipartData);
                    break;
                case DATE:
                    Date_Data(multipart_chunks[i], _multipartData);
                    break;
                case TYPE:
                    _multipartData.Add("Type", "Submit"); // we know the type.
                    break;
                default:
                    break;
            }

        }

        Create_File();


        List<String> keys = new List<String>(_multipartData.Keys);

        foreach (String key in keys)
        {
            Console.WriteLine($"multi Key: [{key}] multi Value: [{_multipartData[key]}]");
        }


    }

    private void SetStatus(int status)
    {
        _status = status;
    }

    public static String GetTimestamp(DateTime value)
    {
        return value.ToString("yyyyMMddHHmmssffff");
    }

    public Dictionary<String, String> GetMultiData()
    {
        return _multipartData;
    }

    public int GetStatus()
    {
        return _status;
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
            description += "Key: " + each.Key + " | Val: " + each.Value + "\n";
        }

        return description;
    }
}