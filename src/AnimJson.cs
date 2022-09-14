using Newtonsoft.Json;
namespace EnemyHPBar;

public class AnimJson {
	public static string FilterExtension(string name) {
		return name.Replace(".png", "");
	}
	public static void SaveAnimConfig(HPBarAnimation anim) {
		var fs = new FileStream(Path.Combine(EnemyHPBar.SkinPath, anim.name + ".json"), FileMode.Create, FileAccess.Write);
		var fw = new StreamWriter(fs);
		fw.Write(JsonConvert.SerializeObject(anim));
		fw.Close();
		fs.Close();
	}
	public static HPBarAnimation LoadAnimConfig(string path) {
		var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
		var fw = new StreamReader(fs);
		var animstr = fw.ReadToEnd();
		var anim = JsonConvert.DeserializeObject<HPBarAnimation>(animstr, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
		fw.Close();
		fs.Close();
		return anim;

	}
	public static bool ConfigExist(HPBarAnimation anim) {
		if (File.Exists(Path.Combine(EnemyHPBar.SkinPath, anim.name + ".json"))) {
			return true;
		}
		return false;
	}
	public static void LoadAllConfig() {
		EnemyHPBar.bganim = CustomHPBarAnimation.LoadAnimation(FilterExtension(EnemyHPBar.HPBAR_BG), ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BG));
		EnemyHPBar.olanim = CustomHPBarAnimation.LoadAnimation(FilterExtension(EnemyHPBar.HPBAR_OL), ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_OL));
		EnemyHPBar.mganim = CustomHPBarAnimation.LoadAnimation(FilterExtension(EnemyHPBar.HPBAR_MG), ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_MG));
		EnemyHPBar.fganim = CustomHPBarAnimation.LoadAnimation(FilterExtension(EnemyHPBar.HPBAR_FG), ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_FG));
		EnemyHPBar.bossfganim = CustomHPBarAnimation.LoadAnimation(FilterExtension(EnemyHPBar.HPBAR_BOSSFG), ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BOSSFG));
		EnemyHPBar.bossbganim = CustomHPBarAnimation.LoadAnimation(FilterExtension(EnemyHPBar.HPBAR_BOSSBG), ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BOSSBG));
		EnemyHPBar.bossolanim = CustomHPBarAnimation.LoadAnimation(FilterExtension(EnemyHPBar.HPBAR_BOSSOL), ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BOSSOL));

		if (ConfigExist(EnemyHPBar.bganim)) {
			var tmpvalue=CustomHPBarAnimation.loadedSprites[EnemyHPBar.bganim];
			_=CustomHPBarAnimation.loadedSprites.Remove(EnemyHPBar.bganim);
			EnemyHPBar.bganim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, EnemyHPBar.bganim.name + ".json"));
			CustomHPBarAnimation.loadedSprites[EnemyHPBar.bganim] = tmpvalue;
		}
		if (ConfigExist(EnemyHPBar.fganim)) {
			var tmpvalue = CustomHPBarAnimation.loadedSprites[EnemyHPBar.fganim];
			_ = CustomHPBarAnimation.loadedSprites.Remove(EnemyHPBar.fganim);
			EnemyHPBar.fganim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, EnemyHPBar.fganim.name + ".json"));
			CustomHPBarAnimation.loadedSprites[EnemyHPBar.fganim] = tmpvalue;
		}
		if (ConfigExist(EnemyHPBar.olanim)) {
			var tmpvalue = CustomHPBarAnimation.loadedSprites[EnemyHPBar.olanim];
			_ = CustomHPBarAnimation.loadedSprites.Remove(EnemyHPBar.olanim);
			EnemyHPBar.olanim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, EnemyHPBar.olanim.name + ".json"));
			CustomHPBarAnimation.loadedSprites[EnemyHPBar.olanim] = tmpvalue;
		}
		if (ConfigExist(EnemyHPBar.mganim)) {
			var tmpvalue = CustomHPBarAnimation.loadedSprites[EnemyHPBar.mganim];
			_ = CustomHPBarAnimation.loadedSprites.Remove(EnemyHPBar.mganim);
			EnemyHPBar.mganim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, EnemyHPBar.mganim.name + ".json"));
			CustomHPBarAnimation.loadedSprites[EnemyHPBar.mganim] = tmpvalue;
		}
		if (ConfigExist(EnemyHPBar.bossbganim)) {
			var tmpvalue = CustomHPBarAnimation.loadedSprites[EnemyHPBar.bossbganim];
			_ = CustomHPBarAnimation.loadedSprites.Remove(EnemyHPBar.bossbganim);
			EnemyHPBar.bossbganim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, EnemyHPBar.bossbganim.name + ".json"));
			CustomHPBarAnimation.loadedSprites[EnemyHPBar.bossbganim] = tmpvalue;
		} 
		if (ConfigExist(EnemyHPBar.bossolanim)) {
			var tmpvalue = CustomHPBarAnimation.loadedSprites[EnemyHPBar.bossolanim];
			_ = CustomHPBarAnimation.loadedSprites.Remove(EnemyHPBar.bossolanim);
			EnemyHPBar.bossolanim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, EnemyHPBar.bossolanim.name + ".json"));
			CustomHPBarAnimation.loadedSprites[EnemyHPBar.bossolanim] = tmpvalue;
		}
		if (ConfigExist(EnemyHPBar.bossfganim)) {
			var tmpvalue = CustomHPBarAnimation.loadedSprites[EnemyHPBar.bossfganim];
			_ = CustomHPBarAnimation.loadedSprites.Remove(EnemyHPBar.bossfganim);
			EnemyHPBar.bossfganim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, EnemyHPBar.bossfganim.name + ".json"));
			CustomHPBarAnimation.loadedSprites[EnemyHPBar.bossfganim] = tmpvalue;
		}

	}
	public static void SaveAllConfig() {

		if (!ConfigExist(EnemyHPBar.bganim)&& EnemyHPBar.bganim.frame>0) {
			SaveAnimConfig(EnemyHPBar.bganim);
		}
		if (!ConfigExist(EnemyHPBar.olanim) && EnemyHPBar.olanim.frame > 0) {
			SaveAnimConfig(EnemyHPBar.olanim);
		}
		if (!ConfigExist(EnemyHPBar.mganim) && EnemyHPBar.mganim.frame > 0) {
			SaveAnimConfig(EnemyHPBar.mganim);
		}
		if (!ConfigExist(EnemyHPBar.fganim) && EnemyHPBar.fganim.frame > 0) {
			SaveAnimConfig(EnemyHPBar.fganim);
		}
		if (!ConfigExist(EnemyHPBar.bossbganim) && EnemyHPBar.bossbganim.frame > 0) {
			SaveAnimConfig(EnemyHPBar.bossbganim);
		}
		if (!ConfigExist(EnemyHPBar.bossfganim) && EnemyHPBar.bossfganim.frame > 0) {
			SaveAnimConfig(EnemyHPBar.bossfganim);
		}
		if (!ConfigExist(EnemyHPBar.bossolanim) && EnemyHPBar.bossfganim.frame > 0) {
			SaveAnimConfig(EnemyHPBar.bossolanim);
		}


	}
}
