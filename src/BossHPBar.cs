namespace EnemyHPBar;

public class BossHPBar : MonoBehaviour {
	private GameObject bg_go;
	private GameObject fg_go;
	private GameObject ol_go;
	private CanvasRenderer bg_cr;
	private CanvasRenderer fg_cr;
	private CanvasRenderer ol_cr;

	private readonly float bossbgScale = EnemyHPBar.globalSettings.bossbgScale;
	private readonly float bossfgScale = EnemyHPBar.globalSettings.bossfgScale;
	private readonly float bossolScale = EnemyHPBar.globalSettings.bossolScale;

	private const float scaleFactor = 2f / 3f;

	public Image health_bar;

	public float maxHP;

	public int position;
	public Vector2 screenScale;
	public HealthManager hm;
	public Vector2 objectPos;

	public void Awake() {
		Logger.LogDebug($@"Creating Boss HP Bar for {name}");

		bg_go = CanvasUtil.CreateImagePanel(EnemyHPBar.bossCanvas, EnemyHPBar.bossbg, new CanvasUtil.RectData(
			EnemyHPBar.bossbg.rect.size * bossbgScale * scaleFactor, new Vector2(0f, 24f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
		fg_go = CanvasUtil.CreateImagePanel(EnemyHPBar.bossCanvas, EnemyHPBar.bossfg, new CanvasUtil.RectData(
			EnemyHPBar.bossfg.rect.size * bossfgScale * scaleFactor, new Vector2(0f, 24f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));
		ol_go = CanvasUtil.CreateImagePanel(EnemyHPBar.bossCanvas, EnemyHPBar.bossol, new CanvasUtil.RectData(
			EnemyHPBar.bossol.rect.size * bossolScale * scaleFactor, new Vector2(0f, 24f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f)));

		bg_cr = bg_go.GetComponent<CanvasRenderer>();
		fg_cr = fg_go.GetComponent<CanvasRenderer>();
		ol_cr = ol_go.GetComponent<CanvasRenderer>();

		objectPos = fg_go.transform.position;

		health_bar = fg_go.GetComponent<Image>();
		health_bar.type = Image.Type.Filled;
		health_bar.fillMethod = Image.FillMethod.Horizontal;
		health_bar.preserveAspect = false;

		bg_go.GetComponent<Image>().preserveAspect = false;
		ol_go.GetComponent<Image>().preserveAspect = false;

		hm = gameObject.GetComponent<HealthManager>();

		SetHPBarAlpha(0);

		maxHP = hm.hp;
		if (AnimJson.AnimExists("bossbg")) {
			bg_go.GetAddComponent<HPBarAnimationController>().Init(AnimJson.animDict["bossbg"]);
		}

		if (AnimJson.AnimExists("bossfg")) {
			fg_go.GetAddComponent<HPBarAnimationController>().Init(AnimJson.animDict["bossfg"]);
		}

		if (AnimJson.AnimExists("bossol")) {
			ol_go.GetAddComponent<HPBarAnimationController>().Init(AnimJson.animDict["bossol"]);
		}
	}

	private void SetHPBarAlpha(float alpha) {
		if (alpha <= 0f) {
			_ = EnemyHPBar.ActiveBosses.Remove(gameObject);
		} else if (!EnemyHPBar.ActiveBosses.Contains(gameObject)) {
			EnemyHPBar.ActiveBosses.Add(gameObject);
		}

		bg_cr.SetAlpha(alpha);
		fg_cr.SetAlpha(alpha);
		ol_cr.SetAlpha(alpha);
	}

	private void DestroyHPBar() {
		Destroy(fg_go);
		Destroy(bg_go);
		Destroy(ol_go);
		Destroy(health_bar);
	}

	private void MoveHPBar(Vector2 position) {
		fg_go.transform.position = position;
		bg_go.transform.position = position;
		ol_go.transform.position = position;
	}

#pragma warning disable IDE0051

	private void OnDestroy() {
		SetHPBarAlpha(0);
		DestroyHPBar();
		Logger.LogDebug($@"Destroyed enemy {gameObject.name}");
	}

	private void OnDisable() {
		SetHPBarAlpha(0);
		Logger.LogDebug($@"Disabled enemy {gameObject.name}");
	}

	private void FixedUpdate() {
		position = EnemyHPBar.ActiveBosses.IndexOf(gameObject) + 1;

		Logger.LogFine($@"Enemy {name}: currHP {hm.hp}, maxHP {maxHP}");
		health_bar.fillAmount = hm.hp / maxHP;
		if (health_bar.fillAmount < 1f && health_bar.fillAmount > 0f) {
			float alpha = GameManager.instance.gameState == GameState.PAUSED ? 0.5f : 1;
			SetHPBarAlpha(alpha);
		}

		if (gameObject.name == "New Game Object" && hm.hp <= 0) {
			Destroy(gameObject);
			Logger.LogDebug($@"Placeholder killed");
		}

		if (hm.hp <= 0f && !hm.hasSpecialDeath) {
			SetHPBarAlpha(0);
		}
	}

	private void LateUpdate() {
		position = EnemyHPBar.ActiveBosses.IndexOf(gameObject);
		MoveHPBar(new Vector2(objectPos.x, objectPos.y + (position * (health_bar.rectTransform.sizeDelta.y + (10f * Screen.height / 720f)))));
	}

#pragma warning restore IDE0051
}
