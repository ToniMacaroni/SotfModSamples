using System.Drawing;
using RedLoader;
using SonsSdk.Networking;
using UdpKit;

namespace EventTest;

public class EventBase<T> : Packets.NetEvent where T : Packets.NetEvent, new()
{
    public static T Instance;
    
    public static void Register()
    {
        Instance = new T();
        Packets.Register(Instance);
        RLog.Msg(Color.GreenYellow, $"Registered {typeof(T).Name}");
    }
    
    public override void Read(UdpPacket packet, BoltConnection fromConnection)
    {
        if (BoltNetwork.isServer)
            ReadMessageServer(packet, fromConnection);
        else
            ReadMessageClient(packet);
    }
    
    /// <summary>
    /// Read message on the server
    /// </summary>
    protected virtual void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
    { }

    /// <summary>
    /// Read message on the client
    /// </summary>
    protected virtual void ReadMessageClient(UdpPacket packet)
    { }

    public override string Id => typeof(T).FullName;
}