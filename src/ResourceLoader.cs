namespace EnemyHPBar;
using CustomKnight;
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
	public static byte[] GetBackgroundImage() => GetImage(EnemyHPBar.HPBAR_BG);

	public static byte[] GetForegroundImage() => GetImage(EnemyHPBar.HPBAR_FG);

	public static byte[] GetMiddlegroundImage() => GetImage(EnemyHPBar.HPBAR_MG);

	public static byte[] GetOutlineImage() => GetImage(EnemyHPBar.HPBAR_OL);

	public static byte[] GetBossBackgroundImage() => GetImage(EnemyHPBar.HPBAR_BOSSBG);

	public static byte[] GetBossForegroundImage() => GetImage(EnemyHPBar.HPBAR_BOSSFG);

	public static byte[] GetBossOutlineImage() => GetImage(EnemyHPBar.HPBAR_BOSSOL);
}
