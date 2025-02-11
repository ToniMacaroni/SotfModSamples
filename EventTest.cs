using Il2CppInterop.Runtime.Injection;
using RedLoader;
using SonsSdk;
using SonsSdk.Attributes;
using SonsSdk.Networking;
using UnityEngine;

namespace EventTest;
extern alias BoltUser;

public class EventTest : SonsMod, IOnAfterSpawnReceiver
{
    public static EventTest Instance;

    private GameObject _prefab;
    private GameObject _cube;

    public EventTest()
    {
        Instance = this;
    }
    
    protected override void OnInitializeMod()
    {
        Config.Init();
        JoiningEvent.Register();
        ColorSyncEvent.Register();
        ClassInjector.RegisterTypeInIl2Cpp<ColorSetter>();
        
        _prefab = DebugTools.CreatePrimitive(PrimitiveType.Cube).DontDestroyOnLoad().HideAndDontSave();
        _prefab.name = "ColorSetterSamplePrefab";
        _prefab.AddComponent<ColorSetter>();
        EntityManager.RegisterPrefab(_prefab.AddComponent<BoltEntity>().Init(112233, BoltFactories.RigidbodyState));
    }

    protected override void OnSdkInitialized()
    {
        // SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        EventHandler.Create();
    }

    public void OnAfterSpawn()
    {
        if (!BoltNetwork.isClient)
            return;
        
        var entity = BoltNetwork.Instantiate(_prefab, SonsTools.GetPositionInFrontOfPlayer(2, 1), Quaternion.identity);
        GlobalInput.RegisterKey(KeyCode.Y, () =>
        {
            ColorSyncEvent.SendColor(entity, Color.red);
        });

        RLog.Msg(System.ConsoleColor.Gray, "Spawned entity");
    }
}