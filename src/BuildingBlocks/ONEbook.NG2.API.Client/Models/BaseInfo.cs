using System.Net;
using System.Net.Sockets;

namespace ONEbook.NG2.API.Client.Models;

public class BaseInfo
{
    public BaseInfo()
    {
        TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff tt");
        Language = "en-US";
        Ip = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork).ToString().Trim();
    }

    public string TimeStamp { get; set; }

    public int UserId { get; set; }

    public string Language { get; set; }

    public string Ip { get; set; }
}