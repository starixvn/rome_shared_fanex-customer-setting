namespace ONEbook.NG2.API.Client.Configurations;

public class NG2Configuration
{
    public const string NG2 = "NG2";

    public string Url { get; set; }

    public string Login { get; set; }

    public string Password { get; set; }

    public bool Debug { get; set; } = false;

    public bool Success { get; set; } = false;

    public RabbitConfiguration Rabbit { get; set; }
}

public class RabbitConfiguration
{
    public string Host { get; set; } = "localhost";

    public string Name { get; set; } = "hostname";

    public string Uid { get; set; } = "username";

    public string Pwd { get; set; } = "password";

    public int Timeout { get; set; } = 30;

    public string Topic { get; set; } = "topic";
}