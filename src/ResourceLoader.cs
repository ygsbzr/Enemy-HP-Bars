using System.Text.RegularExpressions;

using CustomKnight;

namespace EnemyHPBar;
internal static class ResourceLoader {
	private static byte[] GetImage(string name) => File.ReadAllBytes(Path.Combine(EnemyHPBar.DATA_DIR, EnemyHPBar.CurrentSkin.GetId(), name));

	public static byte[] GetCKImage(string name) => Directory.Exists(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar"))
			? File.ReadAllBytes(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar", name))
			: GetImage(name);

	public static Sprite[] GetAllImages(string name) {
		string[] imagenames = Directory.GetFiles(EnemyHPBar.SkinPath, $"{AnimJson.FilterExtension(name)}_*.png");
		var sprites = new Sprite[imagenames.Length];
		foreach (string imagename in imagenames) {
			_ = int.TryParse(Regex.Match(imagename, ".*_([0-9]*).png").Groups[1].Value, out int num);
			Logger.LogDebug($"num:{num}");
			sprites[num] = EnemyHPBar.HPBarCreateSprite(File.ReadAllBytes(imagename));
		}

		return sprites;

	}
	public static byte[] GetBackgroundImage() => GetImage(EnemyHPBar.HPBAR_BG);

	public static byte[] GetForegroundImage() => GetImage(EnemyHPBar.HPBAR_FG);

	public static byte[] GetMiddlegroundImage() => GetImage(EnemyHPBar.HPBAR_MG);

	public static byte[] GetOutlineImage() => GetImage(EnemyHPBar.HPBAR_OL);

	public static byte[] GetBossBackgroundImage() => GetImage(EnemyHPBar.HPBAR_BOSSBG);

	public static byte[] GetBossForegroundImage() => GetImage(EnemyHPBar.HPBAR_BOSSFG);

	public static byte[] GetBossOutlineImage() => GetImage(EnemyHPBar.HPBAR_BOSSOL);
}
