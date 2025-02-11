using SonsSdk;

namespace EventTest;
extern alias BoltUser;

public class EventTest : SonsMod
{
    public static EventTest Instance;

    public EventTest()
    {
        Instance = this;
    }
    
    protected override void OnInitializeMod()
    {
        Config.Init();
        JoiningEvent.Register();
    }

    protected override void OnSdkInitialized()
    {
        // SettingsRegistry.CreateSettings(this, null, typeof(Config));
    }

    protected override void OnGameStart()
    {
        EventHandler.Create();
    }
}