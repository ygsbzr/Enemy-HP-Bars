namespace EnemyHPBar;
using CustomKnight;

using System.Text.RegularExpressions;
internal sealed class ResourceLoader : MonoBehaviour {
	private static byte[] GetImage(string name)
    {
		EnemyHPBar.CompleteImage(Path.Combine(EnemyHPBar.DATA_DIR,EnemyHPBar.CurrentSkin.GetId()));
		return File.ReadAllBytes(Path.Combine(EnemyHPBar.DATA_DIR, EnemyHPBar.CurrentSkin.GetId(), name));

	}
	public static byte[] GetCKImage(string name)
    {
		if(Directory.Exists(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(),"HPBar")))
        {
			EnemyHPBar.CompleteImage(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar"));
			return File.ReadAllBytes(Path.Combine(SkinManager.GetCurrentSkin().getSwapperPath(), "HPBar", name));
        }
		return GetImage(name);
    }
	public static Sprite[] GetAllImages(string name)
	{
		var imagenames = Directory.GetFiles(EnemyHPBar.SkinPath, $"{AnimJson.FilterExtension(name)}_*.png");
		Sprite[] sprites = new Sprite[imagenames.Length];
		foreach (var imagename in imagenames)
		{
			_ = int.TryParse(Regex.Match(imagename,".*_([0-9]*).png").Groups[1].Value, out int num);
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
