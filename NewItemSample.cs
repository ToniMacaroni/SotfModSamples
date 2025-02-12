using Endnight.Extensions;
using Endnight.Utilities;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using RedLoader;
using RedLoader.Utils;
using Sons.Crafting;
using Sons.Gameplay;
using Sons.Inventory;
using Sons.Items.Core;
using SonsSdk;
using SonsSdk.Attributes;
using TheForest;
using TheForest.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityGLTF;
using Color = System.Drawing.Color;
using Types = UnityEngine.Types;

namespace NewItemSample;

public class NewItemSample : SonsMod, IOnAfterSpawnReceiver
{
    private ItemData _item;
    private GameObject _itemPrefab;
    private GameObject _pickupPrefab;
    private Texture2D _itemTex;
    
    public NewItemSample()
    {
        HarmonyPatchAll = true;
    }
    
    protected override void OnInitializeMod()
    {
        Config.Init();
    }

    protected override void OnSdkInitialized()
    {
        // SettingsRegistry.CreateSettings(this, null, typeof(Config));
        SdkEvents.OnGameActivated.Subscribe(OnFirstGameActivation, unsubscribeOnFirstInvocation:true);
        _ = LoadObject(); // load the gltf model
        _itemTex = AssetLoaders.LoadTexture(DataPath / "itemtex.png"); // load the item image
        _itemTex.hideFlags = HideFlags.HideAndDontSave;
    }

    /// <summary>
    /// Load the item model from a gltf
    /// </summary>
    private async Task LoadObject()
    {
        var importer = new GLTFSceneImporter(DataPath / "fancy_perfume_bottle.glb", new());
        await importer.LoadSceneAsync();
        _itemPrefab = importer.CreatedObject.DontDestroyOnLoad().HideAndDontSave();
        foreach (var renderer in _itemPrefab.GetComponentsInChildren<MeshRenderer>())
        {
            // the renderers need colliders
            if (!renderer.gameObject.TryGetComponent(out Collider _))
            {
                renderer.gameObject.GetOrAddComponent<BoxCollider>();
            }
            
            // it doesn't snow in our inventory :)
            renderer.sharedMaterial.SetFloat("_EnableSnow", 0);
        }
        
        // model is a tad to big so I scale it down
        _itemPrefab.transform.localScale = Vector3.one * 0.2f;
    }
    
    /// <summary>
    /// Create a test model for the item
    /// </summary>
    /// <returns></returns>
    public void CreateDebugModel()
    {
        _itemPrefab = DebugTools.CreatePrimitive(PrimitiveType.Sphere).DontDestroyOnLoad();
        _itemPrefab.transform.localScale = Vector3.one * 0.1f;
    }

    private void OnFirstGameActivation()
    {
        _item = ItemTools.CreateAndRegisterItem(667655, "Super Strong Item", description:"Some super strong item.");
        _item._uiData._icon = _item._uiData._outlineIcon = _itemTex;
        
        // ============== Some parameters for holdable stuff =============
        _item._equipmentSlot = EquipmentSlot.RightHand;
        _item._equippedAnimVars.AddItem(AnimatorVariables.axeHeld);
        _item._type = Sons.Items.Core.Types.Equippable | Sons.Items.Core.Types.Craftable | Sons.Items.Core.Types.CraftingMaterial;
        _item._guiType = ItemData.GuiType.Weapon;
        _item._uiData._leftClick = ItemUiData.LeftClickCommands.equip;
        // ===============================================================

        // use the same model for the pickup
        _pickupPrefab = _itemPrefab;
        
        // also use the same model for the held item
        _item._heldPrefab = _itemPrefab.transform;
    }

    public void OnAfterSpawn()
    {
        // drop the ingredients in front of the player so we can test the recipe
        DebugConsole.Instance.SendCommand("spawnitem stick 5");
        
        new ItemTools.ItemBuilder(_itemPrefab, _item)
            .AddInventoryItem() // add a location in the inventory
            .AddIngredientItem() // add a location on the mat as an ingredient
            .AddCraftingResultItem() // add a location on the mat as a crafting result
            .SetupHeld() // add a location in the hand. This will enable you to hold the item
            .SetupPickup(_pickupPrefab) // setup a pickup for the item given a model
            .Recipe // recipe setup
            .AddIngredient(ItemTools.Identifiers.Stick, 2) // ingredients of the recipe
            .AddResult(_item._id) // resulting item of the recipe
            .Animation(ItemTools.CraftAnimations.CraftedArrows) // crafting animation
            .BuildAndAdd(); // register the recipe

        new ItemTools.RecipeBuilder() // Workaround: in order to be able to add the custom item to the mat it needs to be an ingredient of a recipe
            .AddIngredient(_item._id, 2)
            .AddResult(ItemTools.Identifiers.Stick)
            .BuildAndAdd();

        RLog.Msg(Color.SeaGreen, "[ ADDED ITEM ]");
    }
}