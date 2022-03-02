namespace EnemyHPBar;

[ModExportName(nameof(EnemyHPBar))]
public static class EnemyHPBarExport {
	public static void DisableHPBar(GameObject go) {
		go.RemoveComponent<HPBar>();
		go.RemoveComponent<BossHPBar>();
		go.GetAddComponent<DisableHPBar>();
	}

	public static void EnableHPBar(GameObject go) {
		go.RemoveComponent<DisableHPBar>();
		HealthManager hm = go.GetComponent<HealthManager>();
		hm.enabled = false;
		hm.enabled = true;
	}

	public static void RefreshHPBar(GameObject go) {
		DisableHPBar(go);
		EnableHPBar(go);
	}

	public static void MarkAsBoss(GameObject go) {
		go.GetAddComponent<BossMarker>().isBoss = true;
		RefreshHPBar(go);
	}

	public static void MarkAsNonBoss(GameObject go) {
		go.GetAddComponent<BossMarker>().isBoss = false;
		RefreshHPBar(go);
	}
}
