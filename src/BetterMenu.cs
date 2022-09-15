using MenuButton = Satchel.BetterMenus.MenuButton;

namespace EnemyHPBar;

// Thanks to CustomKnight menu: https://github.com/PrashantMohta/HollowKnight.CustomKnight/blob/moreskin/CustomKnight/Menu/BetterMenu.cs
internal static class BetterMenu {
	internal static int selectedSkin = 0;
	internal static Menu MenuRef = null;

	internal static MenuScreen GetMenu(MenuScreen lastMenu, ModToggleDelegates? toggleDelegates) {
		MenuRef ??= PrepareMenu((ModToggleDelegates)toggleDelegates);

		MenuRef.OnBuilt += (_, Element) => {
			if (EnemyHPBar.CurrentSkin != null) {
				BetterMenu.SelectedSkin(EnemyHPBar.CurrentSkin.GetId());
			}
		};

		return MenuRef.GetMenuScreen(lastMenu);
	}

	internal static void ApplySkin() {
		ISelectableSkin skinToApply = EnemyHPBar.SkinList[selectedSkin];
		BetterMenu.SetSkinById(skinToApply.GetId());
		EnemyHPBar.bossol = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossOutlineImage());
		EnemyHPBar.bossbg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossBackgroundImage());
		EnemyHPBar.bossfg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBossForegroundImage());
		EnemyHPBar.ol = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetOutlineImage());
		EnemyHPBar.fg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetForegroundImage());
		EnemyHPBar.mg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetMiddlegroundImage());
		EnemyHPBar.bg = EnemyHPBar.HPBarCreateSprite(ResourceLoader.GetBackgroundImage());
		AnimJson.animdic.Clear();
		AnimJson.Initdic();
		AnimJson.LoadAllConfig();
		AnimJson.SaveAllConfig();
	}

	internal static string[] GetSkinNameArray() => EnemyHPBar.SkinList.Select(s => HPBarList.MaxLength(s.GetName(), EnemyHPBar.globalSettings.NameLength)).ToArray();

	internal static Menu PrepareMenu(ModToggleDelegates toggleDelegates) => new Menu("EnemyHPBar", new Element[] {
		Blueprints.CreateToggle(toggleDelegates,"HPBar Toggle","","Enabled","Disabled"),
			new HorizontalOption(
				"Select Skin", "The skin will be used for current",
				GetSkinNameArray(),
				(setting) => { selectedSkin = setting; },
				() => selectedSkin,
				Id: "SelectSkinOption"),
			new HorizontalOption(
				"Intergration","Intergration with CK?(Make sure you install CK if you want to use it,and reset skin whenyou turn it to true),Create a folder named “HPBar” in your skin folder",
				new string[]{"True","False"},
                (choose)=>{EnemyHPBar.globalSettings.Intergration=(choose==0); },
                ()=>EnemyHPBar.globalSettings.Intergration?0:1,
				Id:"CKIntergration"
				)
			,
			new MenuRow(
				new List<Element>{
					Blueprints.NavigateToMenu( "Skin List","Opens a list of Skins", () => HPBarList.GetMenu(MenuRef.menuScreen)),
					 new MenuButton("Apply Skin","Apply The currently selected skin.", _ => ApplySkin()),
				},
				Id: "ApplyButtonGroup"
			){ XDelta = 400f},

		});

	internal static void SelectedSkin(string skinId) => selectedSkin = EnemyHPBar.SkinList.FindIndex(skin => skin.GetId() == skinId);

	public static ISelectableSkin GetSkinById(string id) => EnemyHPBar.SkinList.Find(skin => skin.GetId() == id) ?? GetDefaultSkin();

	public static ISelectableSkin GetDefaultSkin() {
		if (EnemyHPBar.DefaultSkin == null) {
			EnemyHPBar.DefaultSkin = GetSkinById("Default");
		}
		return EnemyHPBar.DefaultSkin;
	}

	public static void SetSkinById(string id) {
		ISelectableSkin Skin = GetSkinById(id);
		if (EnemyHPBar.CurrentSkin.GetId() == Skin.GetId()) { return; }
		EnemyHPBar.CurrentSkin = Skin;
	}
}
