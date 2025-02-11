using System.Drawing;
using Bolt;
using RedLoader;
using RedLoader.Utils;
using Sons.Multiplayer;
using SonsSdk.Networking;
using UdpKit;

namespace EventTest;

public class JoiningEvent : EventBase<JoiningEvent>
{
    public enum EMessageType : byte
    {
        InfoPayload = 1, // this message carries the info
        RequestInfo = 2 // this message requests some info from the client
    }

    /// <summary>
    /// Read message on the server
    /// </summary>
    protected override void ReadMessageServer(UdpPacket packet, BoltConnection fromConnection)
    {
        var messageType = (EMessageType)packet.ReadByte();
        switch (messageType)
        {
            case EMessageType.InfoPayload:
                // read the info the client has sent us
                var wildNumber = packet.ReadInt();
                var modVersion = packet.ReadString();
                
                RLog.Msg(Color.Orange, $"///////// The wild number is {wildNumber} /////////");
                RLog.Msg(Color.Orange, $"/////// Remote Mod version is {modVersion} ////////");
                
                // if we don't like the info kick the player
                if(wildNumber != 69420 || EventTest.Instance.Manifest.Version != modVersion)
                    KickPlayer(fromConnection);
                break;
            default:
                RLog.Error($"Message type not handled on server!");
                break;
        }
    }

    /// <summary>
    /// Read message on the client
    /// </summary>
    protected override void ReadMessageClient(UdpPacket packet, BoltConnection _)
    {
        var messageType = (EMessageType)packet.ReadByte();
        switch (messageType)
        {
            case EMessageType.RequestInfo:
                RLog.Msg(Color.Gray, "Got request message on client");
                SendClientResponse();
                break;
            default:
                RLog.Error("Message type not handled on client!");
                break;
        }
    }
    
    /// <summary>
    /// For sending from the server
    /// </summary>
    private void SendServerResponse(EMessageType message, BoltConnection connection)
    {
        RLog.Msg(Color.Gray, "Sending server message");
        var packet = NewPacket(1, connection);
        packet.Packet.WriteByte((byte)message);
        Send(packet);
    }
    
    public void RequestInfo(BoltConnection connection) => SendServerResponse(EMessageType.RequestInfo, connection);

    /// <summary>
    /// For sending from the client
    /// </summary>
    public void SendClientResponse()
    {
        RLog.Msg(Color.Gray, "Sending client message");
        var packet = NewPacket(5 + EventTest.Instance.Manifest.Version.Length + 2 /* sizeof(byte + int + string) */, GlobalTargets.OnlyServer);
        packet.Packet.WriteByte((byte)EMessageType.InfoPayload);
        
        // send the server some info about yourself
        packet.Packet.WriteInt(69420);
        packet.Packet.WriteString(EventTest.Instance.Manifest.Version);
        
        Send(packet);
    }

    private void KickPlayer(BoltConnection connection)
    {
        connection.Disconnect(new CoopKickToken{Banned = true, KickMessage = "HOST_BANNED_YOU"}.Cast<IProtocolToken>());
    }

    public override string Id => "EventTest_JoiningEvent";
}