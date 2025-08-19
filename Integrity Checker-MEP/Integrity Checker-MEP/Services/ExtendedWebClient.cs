using System;
using System.Net;

public class ExtendedWebClient : WebClient
{
    public int Timeout { get; set; }
    public new bool AllowWriteStreamBuffering { get; set; }

    protected override WebRequest GetWebRequest(Uri address)
    {
        try
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = Timeout;
                var httpRequest = request as HttpWebRequest;
                if (httpRequest != null)
                {
                    httpRequest.KeepAlive = true;
                    httpRequest.AllowWriteStreamBuffering = AllowWriteStreamBuffering;
                }
            }

            return request;
        }
        catch (Exception e)
        {
            Console.WriteLine($"An exception occurred: {e.Message}");
            throw;
        }
    }

    public ExtendedWebClient()
    {
        Timeout = System.Threading.Timeout.Infinite;
    }
}
