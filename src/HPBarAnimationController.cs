namespace EnemyHPBar;

public class HPBarAnimation {
	public float fps;
	public bool loop;
}
public class HPBarAnimationController : MonoBehaviour {
	public HPBarAnimation anim;
	public Sprite[] sprites;
	public int currentFrame = 0;
	public bool animating = false;
	private Image img;
	private DateTime lastFrameChange;

	public void Start() => img = gameObject.GetAddComponent<Image>();
	public void Init(HPBarAnimation animation) {
		anim = animation;
		// init values
		lastFrameChange = DateTime.MinValue;
		currentFrame = -1;
		animating = true;
		sprites = CustomHPBarAnimation.loadedSprites[anim];
	}
	public void Update() {
		if (!animating) { return; }

		if (lastFrameChange == null || (DateTime.Now - lastFrameChange).TotalMilliseconds > 1000 / anim.fps) {
			currentFrame++;
			if (currentFrame >= sprites.Length) {
				currentFrame = anim.loop ? 0 : sprites.Length - 1;
			}

			lastFrameChange = DateTime.Now;
		}

		if (img == null) {
			img = gameObject.GetAddComponent<Image>();
		}

		img.sprite = sprites[currentFrame];
	}
}
public static class CustomHPBarAnimation {
	/// <summary>
	/// All currently loaded Sprites
	/// </summary>
	public static Dictionary<HPBarAnimation, Sprite[]> loadedSprites = new();
	public static HPBarAnimation LoadAnimation(Sprite[] sprites) {
		var anim = new HPBarAnimation {
			fps = 1,
			loop = true
		};
		loadedSprites[anim] = sprites;
		return anim;
	}
}
