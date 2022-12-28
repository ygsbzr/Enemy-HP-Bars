// ReSharper disable InconsistentNaming

namespace EnemyHPBar;

public class HPBar : MonoBehaviour {
	private GameObject bg_go;
	private GameObject mg_go;
	private GameObject fg_go;
	private GameObject ol_go;
	private CanvasRenderer bg_cr;
	private CanvasRenderer fg_cr;
	private CanvasRenderer mg_cr;
	private CanvasRenderer ol_cr;

	private readonly float bgScale = EnemyHPBar.globalSettings.bgScale;
	private readonly float fgScale = EnemyHPBar.globalSettings.fgScale;
	private readonly float olScale = EnemyHPBar.globalSettings.olScale;
	private readonly float mgScale = EnemyHPBar.globalSettings.mgScale;

	public Image health_bar;
	public Image hpbg;

	public float currHP;
	public float maxHP;
	public int oldHP;

	public HealthManager hm;

	public Vector2 objectPos;
	public Vector2 screenScale;

	public void Awake() {
		Logger.LogDebug($@"Creating HP Bar for {name}");

		// On.CameraController.FadeOut += CameraController_FadeOut;

		bg_go = CanvasUtil.CreateImagePanel(EnemyHPBar.canvas, EnemyHPBar.bg,
			new CanvasUtil.RectData(EnemyHPBar.bg.textureRect.size * bgScale * 0.025f, new Vector2(0, 32)));
		mg_go = CanvasUtil.CreateImagePanel(EnemyHPBar.canvas, EnemyHPBar.mg,
			new CanvasUtil.RectData(EnemyHPBar.mg.textureRect.size * mgScale * 0.025f, new Vector2(0, 32)));
		fg_go = CanvasUtil.CreateImagePanel(EnemyHPBar.canvas, EnemyHPBar.fg,
			new CanvasUtil.RectData(EnemyHPBar.fg.textureRect.size * fgScale * 0.025f, new Vector2(0, 32)));
		ol_go = CanvasUtil.CreateImagePanel(EnemyHPBar.canvas, EnemyHPBar.ol,
			new CanvasUtil.RectData(EnemyHPBar.ol.textureRect.size * olScale * 0.025f, new Vector2(0, 32)));

		bg_cr = bg_go.GetComponent<CanvasRenderer>();
		fg_cr = fg_go.GetComponent<CanvasRenderer>();
		mg_cr = mg_go.GetComponent<CanvasRenderer>();
		ol_cr = ol_go.GetComponent<CanvasRenderer>();
		hpbg = mg_go.GetComponent<Image>();
		hpbg.type = Image.Type.Filled;
		hpbg.fillMethod = Image.FillMethod.Horizontal;
		hpbg.preserveAspect = false;

		health_bar = fg_go.GetComponent<Image>();
		health_bar.type = Image.Type.Filled;
		health_bar.fillMethod = Image.FillMethod.Horizontal;
		health_bar.preserveAspect = false;
		bg_go.GetComponent<Image>().preserveAspect = false;
		ol_go.GetComponent<Image>().preserveAspect = false;

		hm = gameObject.GetComponent<HealthManager>();

		SetHPBarAlpha(0);
		maxHP = hm.hp;
		currHP = hm.hp;
		if (AnimJson.AnimExists("bg")) {
			bg_go.GetAddComponent<HPBarAnimationController>().Init(AnimJson.animDict["bg"]);
		}

		if (AnimJson.AnimExists("fg")) {
			fg_go.GetAddComponent<HPBarAnimationController>().Init(AnimJson.animDict["fg"]);
		}

		if (AnimJson.AnimExists("mg")) {
			mg_go.GetAddComponent<HPBarAnimationController>().Init(AnimJson.animDict["mg"]);
		}

		if (AnimJson.AnimExists("ol")) {
			ol_go.GetAddComponent<HPBarAnimationController>().Init(AnimJson.animDict["ol"]);
		}
	}

	private void SetHPBarAlpha(float alpha) {
		bg_cr.SetAlpha(alpha);
		fg_cr.SetAlpha(alpha);
		mg_cr.SetAlpha(alpha);
		ol_cr.SetAlpha(alpha);
	}

	private void DestroyHPBar() {
		Destroy(fg_go);
		Destroy(bg_go);
		Destroy(mg_go);
		Destroy(ol_go);
		Destroy(hpbg);
		Destroy(health_bar);
	}

	private void MoveHPBar(Vector2 position) {
		fg_go.transform.position = position;
		mg_go.transform.position = position;
		bg_go.transform.position = position;
		ol_go.transform.position = position;
	}

#pragma warning disable IDE0051

	private void OnDestroy() {
		SetHPBarAlpha(0);
		DestroyHPBar();
		Logger.LogDebug($@"Destroyed enemy {name}");
	}

	private void OnDisable() {
		SetHPBarAlpha(0);
		Logger.LogDebug($@"Disabled enemy {name}");
	}

	private void FixedUpdate() {
		if (currHP > hm.hp) {
			currHP -= 1f;
		} else {
			currHP = hm.hp;
		}

		Logger.LogFine($@"Enemy {name}: currHP {hm.hp}, maxHP {maxHP}");
		health_bar.fillAmount = hm.hp / maxHP;

		hpbg.fillAmount = currHP / maxHP;

		if (health_bar.fillAmount < 1f) {
			SetHPBarAlpha(1);
		}

		if (gameObject.name == "New Game Object" && currHP <= 0) {
			Logger.LogDebug($@"Placeholder killed");
			Destroy(gameObject);
		}

		if (currHP <= 0f) {
			SetHPBarAlpha(0);
		}

		oldHP = hm.hp;
	}

	private void LateUpdate() {
		objectPos = transform.position + (Vector3.up * 1.5f);
		MoveHPBar(objectPos);
	}

#pragma warning restore IDE0051
}
