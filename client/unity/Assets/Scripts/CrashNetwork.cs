using DotNetty.Transport.Channels;

public static class CrashNetwork
{
    public static string uuid { get; set; }
    public static int id { get; set; }
    public static long seed { get; set; }
    public static string token { get; set; }
    public static IChannel channel { get; set; }
}