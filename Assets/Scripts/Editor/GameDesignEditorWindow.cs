using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameDesignEditorWindow : EditorWindow
{
    private enum Tab { Cards, Collectibles, Enemies, Encounters, Events, Shop, Map }
    private Tab currentTab = Tab.Cards;
    private static readonly string[] TabLabels =
        { "Cards", "Collectibles", "Enemies", "Encounters", "Events", "Shop", "Map" };

    private Vector2 leftScroll;
    private Vector2 rightScroll;
    private Object selectedAsset;

    private List<CardData>        cards        = new();
    private List<CollectibleData> collectibles = new();
    private List<EnemyData>       enemies      = new();
    private List<EncounterData>   encounters   = new();
    private List<EventData>       events       = new();
    private List<ShopData>        shops        = new();
    private MapConfig             mapConfig;

    private const string CardsPath        = "Assets/Game Data/Cards";
    private const string CollectiblesPath = "Assets/Game Data/Collectibles";
    private const string EnemiesPath      = "Assets/Game Data/Enemies";
    private const string EncountersPath   = "Assets/Game Data/Encounters";
    private const string EventsPath       = "Assets/Game Data/Events";
    private const string ShopsPath        = "Assets/Game Data/Shops";
    private const string MapPath          = "Assets/Game Data/Map";

    [MenuItem("Tools/Game Design Editor")]
    public static void Open()
    {
        GameDesignEditorWindow window = GetWindow<GameDesignEditorWindow>("Game Design Editor");
        window.minSize = new Vector2(750, 500);
        window.Refresh();
    }

    private void OnEnable() => Refresh();

    private void Refresh()
    {
        cards        = LoadAll<CardData>(CardsPath);
        collectibles = LoadAll<CollectibleData>(CollectiblesPath);
        enemies      = LoadAll<EnemyData>(EnemiesPath);
        encounters   = LoadAll<EncounterData>(EncountersPath);
        events       = LoadAll<EventData>(EventsPath);
        shops        = LoadAll<ShopData>(ShopsPath);
        mapConfig    = LoadFirst<MapConfig>(MapPath);
    }

    private void OnGUI()
    {
        DrawToolbar();
        EditorGUILayout.BeginHorizontal();
        DrawLeftPanel();
        DrawDivider();
        DrawRightPanel();
        EditorGUILayout.EndHorizontal();
    }


    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        currentTab = (Tab)GUILayout.Toolbar((int)currentTab, TabLabels, EditorStyles.toolbarButton);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            selectedAsset = null;
            Refresh();
        }
        EditorGUILayout.EndHorizontal();
    }


    private void DrawLeftPanel()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(220));
        leftScroll = EditorGUILayout.BeginScrollView(leftScroll);

        switch (currentTab)
        {
            case Tab.Cards:        DrawAssetList(cards,        "New Card",        CreateCard,        c  => string.IsNullOrWhiteSpace(c.cardName)        ? c.name : c.cardName);        break;
            case Tab.Collectibles: DrawAssetList(collectibles, "New Collectible", CreateCollectible, c  => string.IsNullOrWhiteSpace(c.collectibleName) ? c.name : c.collectibleName); break;
            case Tab.Enemies:      DrawAssetList(enemies,      "New Enemy",       CreateEnemy,       e  => string.IsNullOrWhiteSpace(e.enemyName)       ? e.name : e.enemyName);       break;
            case Tab.Encounters:   DrawAssetList(encounters,   "New Encounter",   CreateEncounter,   ec => string.IsNullOrWhiteSpace(ec.encounterName)   ? ec.name : ec.encounterName); break;
            case Tab.Events:       DrawAssetList(events,       "New Event",       CreateEvent,       ev => string.IsNullOrWhiteSpace(ev.eventName)       ? ev.name : ev.eventName);     break;
            case Tab.Shop:         DrawAssetList(shops,        "New Shop",        CreateShop,        s  => string.IsNullOrWhiteSpace(s.shopName)         ? s.name : s.shopName);        break;
            case Tab.Map:          DrawMapLeftPanel();                                                                                                                                  break;
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private void DrawAssetList<T>(List<T> list, string createLabel, System.Action onCreate, System.Func<T, string> displayName)
        where T : ScriptableObject
    {
        if (GUILayout.Button(createLabel)) onCreate();

        EditorGUILayout.Space(4);

        foreach (T asset in list)
        {
            bool isSelected = selectedAsset == asset;
            GUI.backgroundColor = isSelected ? new Color(0.5f, 0.8f, 1f) : Color.white;
            if (GUILayout.Button(displayName(asset), EditorStyles.miniButton))
                selectedAsset = asset;
            GUI.backgroundColor = Color.white;
        }
    }

    private void DrawMapLeftPanel()
    {
        if (GUILayout.Button("Create Map Config")) CreateMapConfig();
        if (mapConfig != null)
        {
            string label = string.IsNullOrWhiteSpace(mapConfig.mapName) ? mapConfig.name : mapConfig.mapName;
            GUI.backgroundColor = selectedAsset == mapConfig ? new Color(0.5f, 0.8f, 1f) : Color.white;
            if (GUILayout.Button(label, EditorStyles.miniButton))
                selectedAsset = mapConfig;
            GUI.backgroundColor = Color.white;
        }
    }


    private void DrawDivider()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(2));
        GUILayout.Box("", GUILayout.ExpandHeight(true), GUILayout.Width(2));
        EditorGUILayout.EndVertical();
    }


    private void DrawRightPanel()
    {
        EditorGUILayout.BeginVertical();
        rightScroll = EditorGUILayout.BeginScrollView(rightScroll);

        if (selectedAsset == null)
        {
            EditorGUILayout.HelpBox("Select or create an asset from the left panel.", MessageType.Info);
        }
        else
        {
            string displayName = GetDisplayName(selectedAsset);
            EditorGUILayout.LabelField(displayName, EditorStyles.boldLabel);
            EditorGUILayout.Space(4);

            switch (currentTab)
            {
                case Tab.Cards:        DrawCardEditor(selectedAsset as CardData);               break;
                case Tab.Collectibles: DrawCollectibleEditor(selectedAsset as CollectibleData); break;
                case Tab.Enemies:      DrawEnemyEditor(selectedAsset as EnemyData);             break;
                case Tab.Encounters:   DrawEncounterEditor(selectedAsset as EncounterData);     break;
                case Tab.Events:       DrawEventEditor(selectedAsset as EventData);             break;
                case Tab.Shop:         DrawShopEditor(selectedAsset as ShopData);               break;
                case Tab.Map:          DrawMapEditor(selectedAsset as MapConfig);               break;
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();
    }

    private SerializedObject cardSO;
    private void DrawCardEditor(CardData card)
    {
        if (card == null) return;
        if (cardSO == null || cardSO.targetObject != card)
            cardSO = new SerializedObject(card);

        cardSO.Update();

        EditorGUILayout.PropertyField(cardSO.FindProperty("cardName"));
        EditorGUILayout.PropertyField(cardSO.FindProperty("description"));
        EditorGUILayout.PropertyField(cardSO.FindProperty("artwork"));
        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(cardSO.FindProperty("manaCost"));
        EditorGUILayout.PropertyField(cardSO.FindProperty("cardType"));
        EditorGUILayout.PropertyField(cardSO.FindProperty("targetType"));
        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(cardSO.FindProperty("effects"), true);
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Upgrade", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(cardSO.FindProperty("upgradedVersion"));

        cardSO.ApplyModifiedProperties();
        DrawRenameAndPingButtons(card);
    }

    private SerializedObject collectibleSO;
    private void DrawCollectibleEditor(CollectibleData col)
    {
        if (col == null) return;
        if (collectibleSO == null || collectibleSO.targetObject != col)
            collectibleSO = new SerializedObject(col);

        collectibleSO.Update();

        EditorGUILayout.PropertyField(collectibleSO.FindProperty("collectibleName"));
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("description"));
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("artwork"));
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("rarity"));
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Passive Bonuses", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("bonusMaxHP"));
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("bonusMana"));
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("bonusCardDraw"));
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("bonusStrength"));
        EditorGUILayout.PropertyField(collectibleSO.FindProperty("bonusStartingGold"));

        collectibleSO.ApplyModifiedProperties();
        DrawRenameAndPingButtons(col);
    }

    private SerializedObject enemySO;
    private void DrawEnemyEditor(EnemyData enemy)
    {
        if (enemy == null) return;
        if (enemySO == null || enemySO.targetObject != enemy)
            enemySO = new SerializedObject(enemy);

        enemySO.Update();

        EditorGUILayout.PropertyField(enemySO.FindProperty("enemyName"));
        EditorGUILayout.PropertyField(enemySO.FindProperty("sprite"));
        EditorGUILayout.PropertyField(enemySO.FindProperty("maxHP"));
        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(enemySO.FindProperty("actions"), true);

        enemySO.ApplyModifiedProperties();
        DrawRenameAndPingButtons(enemy);
    }

    private SerializedObject encounterSO;
    private void DrawEncounterEditor(EncounterData encounter)
    {
        if (encounter == null) return;
        if (encounterSO == null || encounterSO.targetObject != encounter)
            encounterSO = new SerializedObject(encounter);

        encounterSO.Update();

        EditorGUILayout.PropertyField(encounterSO.FindProperty("encounterName"));
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Enemy Slots", EditorStyles.boldLabel);
        SerializedProperty slots = encounterSO.FindProperty("slots");
        EditorGUILayout.PropertyField(slots, true);

        encounterSO.ApplyModifiedProperties();

        if (encounter.slots != null && encounter.slots.Count > 0)
        {
            EditorGUILayout.Space(4);
            EditorGUILayout.HelpBox(
                "Enemies in this encounter:\n" +
                string.Join("\n", encounter.slots.ConvertAll(s =>
                    s.enemyData != null ? $"  • {s.enemyData.enemyName} (HP {s.enemyData.maxHP})" : "  • (none)")),
                MessageType.None);
        }

        DrawRenameAndPingButtons(encounter);
    }

    private SerializedObject eventSO;
    private void DrawEventEditor(EventData ev)
    {
        if (ev == null) return;
        if (eventSO == null || eventSO.targetObject != ev)
            eventSO = new SerializedObject(ev);

        eventSO.Update();

        EditorGUILayout.PropertyField(eventSO.FindProperty("eventName"));
        EditorGUILayout.PropertyField(eventSO.FindProperty("description"));
        EditorGUILayout.PropertyField(eventSO.FindProperty("artwork"));
        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(eventSO.FindProperty("choices"), true);

        eventSO.ApplyModifiedProperties();
        DrawRenameAndPingButtons(ev);
    }

    private SerializedObject shopSO;
    private void DrawShopEditor(ShopData shop)
    {
        if (shop == null) return;
        if (shopSO == null || shopSO.targetObject != shop)
            shopSO = new SerializedObject(shop);

        shopSO.Update();

        EditorGUILayout.PropertyField(shopSO.FindProperty("shopName"));
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Cards", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(shopSO.FindProperty("cardPool"), true);
        EditorGUILayout.PropertyField(shopSO.FindProperty("cardSlotCount"));
        EditorGUILayout.PropertyField(shopSO.FindProperty("cardPriceMin"));
        EditorGUILayout.PropertyField(shopSO.FindProperty("cardPriceMax"));

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Collectibles", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(shopSO.FindProperty("collectiblePool"), true);
        EditorGUILayout.PropertyField(shopSO.FindProperty("collectibleSlotCount"));
        EditorGUILayout.PropertyField(shopSO.FindProperty("collectiblePriceMin"));
        EditorGUILayout.PropertyField(shopSO.FindProperty("collectiblePriceMax"));

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Services", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(shopSO.FindProperty("removalCost"));
        EditorGUILayout.PropertyField(shopSO.FindProperty("upgradeCost"));

        shopSO.ApplyModifiedProperties();
        DrawRenameAndPingButtons(shop);
    }

    private SerializedObject mapSO;
    private void DrawMapEditor(MapConfig config)
    {
        if (config == null) return;
        if (mapSO == null || mapSO.targetObject != config)
            mapSO = new SerializedObject(config);

        mapSO.Update();

        EditorGUILayout.PropertyField(mapSO.FindProperty("mapName"));
        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Layout", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mapSO.FindProperty("floorCount"));
        EditorGUILayout.PropertyField(mapSO.FindProperty("columnsPerFloor"));

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Node Type Weights", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mapSO.FindProperty("weightNormal"));
        EditorGUILayout.PropertyField(mapSO.FindProperty("weightElite"));
        EditorGUILayout.PropertyField(mapSO.FindProperty("weightShop"));
        EditorGUILayout.PropertyField(mapSO.FindProperty("weightEvent"));
        EditorGUILayout.PropertyField(mapSO.FindProperty("weightRest"));

        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(mapSO.FindProperty("eliteGuaranteedFloors"), true);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Boss", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mapSO.FindProperty("bossEncounter"));

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Encounter Pools", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mapSO.FindProperty("normalEncounters"), true);
        EditorGUILayout.PropertyField(mapSO.FindProperty("eliteEncounters"), true);

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Event Pool", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(mapSO.FindProperty("events"), true);

        mapSO.ApplyModifiedProperties();
        DrawRenameAndPingButtons(config);
    }

    private string GetDisplayName(Object asset)
    {
        if (asset is CardData        card) return string.IsNullOrWhiteSpace(card.cardName)           ? card.name        : card.cardName;
        if (asset is CollectibleData col)  return string.IsNullOrWhiteSpace(col.collectibleName)    ? col.name         : col.collectibleName;
        if (asset is EnemyData       enemy) return string.IsNullOrWhiteSpace(enemy.enemyName)       ? enemy.name       : enemy.enemyName;
        if (asset is EncounterData   enc)  return string.IsNullOrWhiteSpace(enc.encounterName)      ? enc.name         : enc.encounterName;
        if (asset is EventData       ev)   return string.IsNullOrWhiteSpace(ev.eventName)           ? ev.name          : ev.eventName;
        if (asset is ShopData        shop) return string.IsNullOrWhiteSpace(shop.shopName)          ? shop.name        : shop.shopName;
        if (asset is MapConfig       map)  return string.IsNullOrWhiteSpace(map.mapName)            ? map.name         : map.mapName;
        return asset.name;
    }

    private void DrawRenameAndPingButtons(Object asset)
    {
        EditorGUILayout.Space(8);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Ping in Project", GUILayout.Width(130)))
            EditorGUIUtility.PingObject(asset);
        if (GUILayout.Button("Select in Inspector", GUILayout.Width(150)))
            Selection.activeObject = asset;
        EditorGUILayout.EndHorizontal();
    }


    private void CreateCard()
    {
        CardData asset = CreateAssetAt<CardData>(CardsPath, "NewCard");
        cards.Add(asset);
        selectedAsset = asset;
    }

    private void CreateCollectible()
    {
        CollectibleData asset = CreateAssetAt<CollectibleData>(CollectiblesPath, "NewCollectible");
        collectibles.Add(asset);
        selectedAsset = asset;
    }

    private void CreateEnemy()
    {
        EnemyData asset = CreateAssetAt<EnemyData>(EnemiesPath, "NewEnemy");
        enemies.Add(asset);
        selectedAsset = asset;
    }

    private void CreateEncounter()
    {
        EncounterData asset = CreateAssetAt<EncounterData>(EncountersPath, "NewEncounter");
        encounters.Add(asset);
        selectedAsset = asset;
    }

    private void CreateEvent()
    {
        EventData asset = CreateAssetAt<EventData>(EventsPath, "NewEvent");
        events.Add(asset);
        selectedAsset = asset;
    }

    private void CreateShop()
    {
        ShopData asset = CreateAssetAt<ShopData>(ShopsPath, "NewShop");
        shops.Add(asset);
        selectedAsset = asset;
    }

    private void CreateMapConfig()
    {
        mapConfig = CreateAssetAt<MapConfig>(MapPath, "MapConfig");
        selectedAsset = mapConfig;
    }

    private static T CreateAssetAt<T>(string folder, string defaultName) where T : ScriptableObject
    {
        EnsureDirectory(folder);
        T asset = ScriptableObject.CreateInstance<T>();
        string path = AssetDatabase.GenerateUniqueAssetPath($"{folder}/{defaultName}.asset");
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        return asset;
    }

    private static List<T> LoadAll<T>(string folder) where T : ScriptableObject
    {
        EnsureDirectory(folder);
        List<T> result = new List<T>();
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder });
        foreach (string guid in guids)
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
            if (asset != null) result.Add(asset);
        }
        return result;
    }

    private static T LoadFirst<T>(string folder) where T : ScriptableObject
    {
        EnsureDirectory(folder);
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { folder });
        if (guids.Length == 0) return null;
        return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guids[0]));
    }

    private static void EnsureDirectory(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                string next = current + "/" + parts[i];
                if (!AssetDatabase.IsValidFolder(next))
                    AssetDatabase.CreateFolder(current, parts[i]);
                current = next;
            }
        }
    }
}
