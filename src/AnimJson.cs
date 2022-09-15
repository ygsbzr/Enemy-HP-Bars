using Newtonsoft.Json;
namespace EnemyHPBar;

public class AnimJson {
	public static string FilterExtension(string name) {
		return name.Replace(".png", "");
	}
	public static void SaveAnimConfig(HPBarAnimation anim,string name) {
		var fs = new FileStream(Path.Combine(EnemyHPBar.SkinPath, name + ".json"), FileMode.Create, FileAccess.Write);
		var fw = new StreamWriter(fs);
		fw.Write(JsonConvert.SerializeObject(anim));
		fw.Close();
		fs.Close();
	}
	public static Dictionary<string, HPBarAnimation> animdic = new();
	public static HPBarAnimation LoadAnimConfig(string path) {
		var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
		var fw = new StreamReader(fs);
		var animstr = fw.ReadToEnd();
		var anim = JsonConvert.DeserializeObject<HPBarAnimation>(animstr, new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace });
		fw.Close();
		fs.Close();
		return anim;

	}
	public static bool ConfigExist(HPBarAnimation anim,string name) {
		if (File.Exists(Path.Combine(EnemyHPBar.SkinPath, name + ".json"))) {
			return true;
		}
		return false;
	}
	public static void Initdic() {
		animdic["ol"]= CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_OL));
		animdic["fg"]= CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_FG));
		animdic["bg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BG));
		animdic["mg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_MG));
		animdic["bossfg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BOSSFG));
		animdic["bossbg"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BOSSBG));
		animdic["bossol"] = CustomHPBarAnimation.LoadAnimation(ResourceLoader.GetAllImages(EnemyHPBar.HPBAR_BOSSOL));
	}
	public static void LoadAllConfig() {
		foreach (var key in animdic.Keys) {
			if(ConfigExist(animdic[key],key)) {
				var configanim = LoadAnimConfig(Path.Combine(EnemyHPBar.SkinPath, key + ".json"));
				animdic[key].loop=configanim.loop;
				animdic[key].fps=configanim.fps;
			}
		}
	}
	public static bool Animexists(string name) {
		if (CustomHPBarAnimation.loadedSprites[animdic[name]].Length > 0) {
			return true;
		}
		return false;
	}
	public static void SaveAllConfig() {
		foreach (var key in animdic.Keys) {
			if (!ConfigExist(animdic[key], key)&&CustomHPBarAnimation.loadedSprites[animdic[key]].Length>0) {
				SaveAnimConfig(animdic[key], key);
			}
		}
	}
}
