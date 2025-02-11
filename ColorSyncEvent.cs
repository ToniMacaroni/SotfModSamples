using System.Drawing;
using Bolt;
using RedLoader;
using RedLoader.Utils;
using Sons.Multiplayer;
using SonsSdk.Networking;
using Color = UnityEngine.Color;
using UdpKit;

namespace EventTest;

public class ColorSyncEvent : RelayEventBase<ColorSyncEvent, ColorSetter>
{
    /// <summary>
    /// Read message on the server
    /// </summary>
    protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
    {
    }
    
    /// <summary>
    /// For sending from the client
    /// </summary>
    public void SendClientResponse()
    {
    }

    private void SendColorInternal(BoltEntity entity, Color color)
    {
        RLog.Msg(ConsoleColor.Gray, "Sending packet");
        var packet = NewPacket(entity, 16, GlobalTargets.AllClients);
        packet.Packet.WriteColorRGB(color);
        Send(packet);
    }

    public static void SendColor(BoltEntity entity, Color color)
    {
        Instance.SendColorInternal(entity, color);
    }

    public override string Id => "EventTest_ColorSyncEvent";
}