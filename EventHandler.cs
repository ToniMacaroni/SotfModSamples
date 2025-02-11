extern alias BoltUser;
using BoltUser::Bolt;
using RedLoader;
using UnityEngine;
using Color = System.Drawing.Color;

namespace EventTest;

/// <summary>
/// Global event handler to catch some events
/// Here we use it to catch if a player connects
/// </summary>
[RegisterTypeInIl2Cpp]
public class EventHandler : GlobalEventListener
{
    public static EventHandler Instance;
    
    public static void Create()
    {
        if (Instance)
            return;
        
        Instance = new GameObject("EventTestEventHandler").AddComponent<EventHandler>();
    }
    
    public override void Connected(BoltConnection connection)
    {
        RLog.Msg(System.Drawing.Color.Gray, "Player connected");
        JoiningEvent.Instance.RequestInfo(connection);
    }
}