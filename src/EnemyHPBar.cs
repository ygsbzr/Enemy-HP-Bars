using Satchel.Futils;
using CustomKnight;
namespace EnemyHPBar;

public class EnemyHPBar : Mod, IGlobalSettings<Settings>, ICustomMenuMod,ITogglableMod {
	private static readonly Lazy<string> Version = new(() => Assembly
		.GetExecutingAssembly()
		.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
		.InformationalVersion
#if DEBUG
		+ "-dev"
#endif
	);


	public static GameObject canvas;
	public static GameObject bossCanvas;
	private static GameObject spriteLoader;
	public static bool InstallCK = false;

	public const string HPBAR_BG = "bg.png";
	public const string HPBAR_FG = "fg.png";
	public const string HPBAR_MG = "mg.png";
	public const string HPBAR_OL = "ol.png";
	public const string HPBAR_BOSSOL = "bossol.png";
	public const string HPBAR_BOSSFG = "bossfg.png";
	public const string HPBAR_BOSSBG = "bossbg.png";
	public const string SPRITE_FOLDER = "CustomHPBar";

	public static readonly string DATA_DIR = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SPRITE_FOLDER);
	public static string SkinPath { get { 
			if (InstallCK && globalSettings.Intergration) {
				return GetCKPath();
			}
			return Path.Combine(EnemyHPBar.DATA_DIR, EnemyHPBar.CurrentSkin.GetId());
		} }

	public static Sprite bg;
	public static Sprite mg;
	public static Sprite fg;
	public static Sprite ol;
	public static Sprite bossbg;
	public static Sprite bossfg;
	public static Sprite bossol;
	public bool ToggleButtonInsideMenu { get; } = true;
	public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggle) => BetterMenu.GetMenu(modListMenu, toggle);

	public override string GetVersion() => Version.Value;

	public EnemyHPBar() =>
		typeof(EnemyHPBarExport).ModInterop();

	public override void Initialize() {

		if (!Directory.Exists(DATA_DIR)) {
			Directory.CreateDirectory(DATA_DIR);
		}

		if (!Directory.Exists(Path.Combine(DATA_DIR, "Default"))) {
			Directory.CreateDirectory(Path.Combine(DATA_DIR, "Default"));
		}
		CompleteImage(Path.Combine(DATA_DIR,"Default"));
		
		GetSkinList();
		LoadLoader();

		ModHooks.OnEnableEnemyHook += Instance_OnEnableEnemyHook;
		ModHooks.OnReceiveDeathEventHook += Instance_OnReceiveDeathEventHook;
		UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
		On.PlayMakerFSM.Start += ModifyFSM;


		canvas = CanvasUtil.CreateCanvas(RenderMode.WorldSpace, new Vector2(1920f, 1080f));
		bossCanvas = CanvasUtil.CreateCanvas(RenderMode.ScreenSpaceOverlay, new Vector2(1920f, 1080f));
		canvas.GetComponent<Canvas>().sortingOrder = 1;
		bossCanvas.GetComponent<Canvas>().sortingOrder = 1;


		bossol = HPBarCreateSprite(ResourceLoader.GetBossOutlineImage());
		bossbg = HPBarCreateSprite(ResourceLoader.GetBossBackgroundImage());
		bossfg = HPBarCreateSprite(ResourceLoader.GetBossForegroundImage());
		ol = HPBarCreateSprite(ResourceLoader.GetOutlineImage());
		fg = HPBarCreateSprite(ResourceLoader.GetForegroundImage());
		mg = HPBarCreateSprite(ResourceLoader.GetMiddlegroundImage());
		bg = HPBarCreateSprite(ResourceLoader.GetBackgroundImage());
		UObject.DontDestroyOnLoad(canvas);
		UObject.DontDestroyOnLoad(bossCanvas);
		InstallCK = ModHooks.GetMod("CustomKnight") is Mod;
		if (InstallCK)
        {
			AddCKHandle();
        }
		AnimJson.animdic.Clear();
		AnimJson.Initdic();
		AnimJson.LoadAllConfig();
		AnimJson.SaveAllConfig();

	}
	private void AddCKHandle()
    {
        SkinManager.OnSetSkin += ChangeSkin;
    }

    private void ChangeSkin(object sender, EventArgs e)
    {
        if(Directory.Exists(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar")))
        {
			if(globalSettings.Intergration)
            {
				bossol = HPBarCreateSprite(ResourceLoader.GetCKImage(HPBAR_BOSSOL));
				bossbg = HPBarCreateSprite(ResourceLoader.GetCKImage(HPBAR_BOSSBG));
				bossfg = HPBarCreateSprite(ResourceLoader.GetCKImage(HPBAR_BOSSFG));
				ol = HPBarCreateSprite(ResourceLoader.GetCKImage(HPBAR_OL));
				fg = HPBarCreateSprite(ResourceLoader.GetCKImage(HPBAR_FG));
				mg = HPBarCreateSprite(ResourceLoader.GetCKImage(HPBAR_MG));
				bg = HPBarCreateSprite(ResourceLoader.GetCKImage(HPBAR_BG));
				AnimJson.animdic.Clear();
				AnimJson.Initdic();
				AnimJson.LoadAllConfig();
				AnimJson.SaveAllConfig();
			}
		}
    }
	public static string GetCKPath() {
		if(Directory.Exists(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar"))) {
			return Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar");
		}
		return Path.Combine(EnemyHPBar.DATA_DIR, EnemyHPBar.CurrentSkin.GetId());
	}
    public static Settings globalSettings = new();

	public void OnLoadGlobal(Settings s) => globalSettings = s;

	public Settings OnSaveGlobal() {
		globalSettings.CurrentSkin = EnemyHPBar.CurrentSkin.GetId();
		return globalSettings;
	}

	public void LoadLoader() {
		if (spriteLoader == null) {
			spriteLoader = new GameObject();
			spriteLoader.AddComponent<ResourceLoader>();
		}
	}

	public static Sprite HPBarCreateSprite(byte[] data) {
		var texture2D = new Texture2D(1, 1);
		texture2D.LoadImage(data);
		texture2D.anisoLevel = 0;
		return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.zero);
	}

	private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1) => ActiveBosses = new();

	private void Instance_OnReceiveDeathEventHook(EnemyDeathEffects enemyDeathEffects, bool eventAlreadyRecieved,
	ref float? attackDirection, ref bool resetDeathEvent, ref bool spellBurn, ref bool isWatery) {
		Logger.LogDebug($@"Enemy {enemyDeathEffects.gameObject.name} dead");
		if (enemyDeathEffects.gameObject.GetComponent<HPBar>() != null) {
			if (enemyDeathEffects.gameObject.GetComponent<HPBar>().oldHP == 0) {
				var placeHolder = new GameObject();
				placeHolder.transform.position = enemyDeathEffects.gameObject.transform.position;
				HealthManager phhm = placeHolder.AddComponent<HealthManager>();
				HPBar phhp = placeHolder.AddComponent<HPBar>();
				phhp.currHP = 18;
				phhp.maxHP = 18;
				phhm.hp = 0;
			} else {
				var placeHolder = new GameObject();
				placeHolder.transform.position = enemyDeathEffects.gameObject.transform.position;
				HealthManager phhm = placeHolder.AddComponent<HealthManager>();
				HPBar phhp = placeHolder.AddComponent<HPBar>();
				placeHolder.AddComponent<EnemyDeathEffects>();
				phhp.currHP = enemyDeathEffects.gameObject.GetComponent<HPBar>().oldHP;
				phhp.maxHP = enemyDeathEffects.gameObject.GetComponent<HPBar>().maxHP;
				phhm.hp = 0;
			}
		}

		return;
	}

	private bool Instance_OnEnableEnemyHook(GameObject enemy, bool isAlreadyDead) {
		if (enemy.GetComponent<DisableHPBar>() != null) {
			return isAlreadyDead;
		}

		if (enemy.name.Contains("White Palace Fly")) {
			return isAlreadyDead;
		}

		HealthManager hm = enemy.GetComponent<HealthManager>();

		if (hm == null) {
			return isAlreadyDead;
		}

		EnemyDeathEffects ede = enemy.GetComponent<EnemyDeathEffects>();
		EnemyDeathTypes? deathType = ede == null
			? null
			: ReflectionHelper.GetField<EnemyDeathEffects, EnemyDeathTypes>(ede, "enemyDeathType");


		bool isBoss = hm.hp >= 200 || deathType == EnemyDeathTypes.LargeInfected;
		if (enemy.GetComponent<BossMarker>() is BossMarker marker) {
			isBoss = marker.isBoss;
		}

		if (!isBoss) {
			HPBar hpbar = hm.gameObject.GetComponent<HPBar>();
			if (hpbar != null || hm.hp >= 7000 || isAlreadyDead) {
				return isAlreadyDead;
			}

			hm.gameObject.AddComponent<HPBar>();
			LogDebug($@"Added HP bar to {enemy.name}");
		} else {
			BossHPBar bossHpBar = hm.gameObject.GetComponent<BossHPBar>();
			if (bossHpBar != null || hm.hp >= 7000 || isAlreadyDead) {
				return isAlreadyDead;
			}

			hm.gameObject.AddComponent<BossHPBar>();
			LogDebug($@"Added Boss HP bar to {enemy.name}");
		}

		return isAlreadyDead;
	}

	private static void ModifyFSM(On.PlayMakerFSM.orig_Start orig, PlayMakerFSM self) {
		orig(self);

		try {
			GameObject go = self.gameObject;

			if (self is {
				name: "False Knight New",
				FsmName: "FalseyControl"
			}) {
				EnemyHPBarExport.MarkAsBoss(go);
			} else if (self.name.StartsWith("Zote Balloon") && self.FsmName == "Control") {
				self.AddCustomAction("Reset", () => EnemyHPBarExport.RefreshHPBar(go));
			} else if (self.name.StartsWith("Zote Crew ")) {
				self.AddCustomAction("Death", () => EnemyHPBarExport.DisableHPBar(go));
				self.AddCustomAction("Activate", () => EnemyHPBarExport.EnableHPBar(go));
			} else if (self is {
				name: "Zote Fluke",
				FsmName: "Control"
			}) {
				self.AddCustomAction("Sleeping", () => EnemyHPBarExport.RefreshHPBar(go));
			} else if (self is {
				name: "Zote Salubra",
				FsmName: "Control"
			}) {
				self.AddCustomAction("Death", () => EnemyHPBarExport.RefreshHPBar(go));
			} else if (self is {
				name: "Zote Thwomp",
				FsmName: "Control"
			}) {
				EnemyHPBarExport.MarkAsNonBoss(self.gameObject);
				self.AddCustomAction("Wait", () => EnemyHPBarExport.DisableHPBar(go));
				self.AddCustomAction("Set Pos", () => EnemyHPBarExport.EnableHPBar(go));
			} else if (self is {
				name: "Zote Turrep",
				FsmName: "Control"
			}) {
				EnemyHPBarExport.MarkAsNonBoss(self.gameObject);
				self.AddCustomAction("Death Pause", () => EnemyHPBarExport.DisableHPBar(go));
				self.AddCustomAction("Appear", () => EnemyHPBarExport.EnableHPBar(go));
			}
		} catch (Exception e) {
			Logger.LogError(e.ToString());
		}
	}

	internal static void GetSkinList() {
		string[] dicts = Directory.GetDirectories(DATA_DIR);
		SkinList = new();
		for (int i = 0; i < dicts.Length; i++) {
			string directoryname = new DirectoryInfo(dicts[i]).Name;
			SkinList.Add(new HPBarList(directoryname));
		}
		EnemyHPBar.CurrentSkin = BetterMenu.GetSkinById(EnemyHPBar.globalSettings.CurrentSkin);
		Logger.Log("Loaded Skins list");
	}
	public static void CompleteImage(string skinpath)
    {
		foreach (string res in Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(t => t.EndsWith("png")))
		{
			string properRes = res.Replace("EnemyHPBar.Resources.", "");
			string resPath = Path.Combine(skinpath, properRes);
			if (File.Exists(resPath))
			{
				continue;
			}

			using FileStream file = File.Create(resPath);
			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(res);
			stream.CopyTo(file);
		}
	}
	public void Unload()
    {
		ModHooks.OnEnableEnemyHook -= Instance_OnEnableEnemyHook;
		ModHooks.OnReceiveDeathEventHook -= Instance_OnReceiveDeathEventHook;
		UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
		On.PlayMakerFSM.Start -= ModifyFSM;
		globalSettings.CurrentSkin = EnemyHPBar.CurrentSkin.GetId();
		foreach (var go in UObject.FindObjectsOfType<GameObject>())
        {
			if (go.GetComponent<HPBar>() is HPBar hpBar)
			{
				UObject.Destroy(hpBar);
			}

			if (go.GetComponent<BossHPBar>() is BossHPBar bossHPBar)
			{
				UObject.Destroy(bossHPBar);
			}
		}
	}

	public static List<GameObject> ActiveBosses;

	public static List<ISelectableSkin> SkinList;
	public static ISelectableSkin CurrentSkin;
	public static ISelectableSkin DefaultSkin;
}
