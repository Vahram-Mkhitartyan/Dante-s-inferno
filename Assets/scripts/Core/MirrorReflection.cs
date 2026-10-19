using Spine;
using Spine.Unity;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MirrorReflection : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController player;
    [SerializeField] private SkeletonAnimation playerVisual;

    [Header("Mirror")]
    [SerializeField] private Vector2 mirrorSize = new Vector2(3.2f, 5.4f);
    [SerializeField] private Vector2 reflectionOffset = new Vector2(0f, 2.5f);
    [SerializeField] private float activationDistance = 8f;
    [SerializeField] private float reflectionScale = 1f;

    [Header("Placeholder Surface")]
    [SerializeField] private bool buildPlaceholderSurface = true;
    [SerializeField] private Color glassColor = new Color(0.08f, 0.11f, 0.16f, 0.92f);
    [SerializeField] private Color frameColor = new Color(0.24f, 0.16f, 0.08f, 1f);
    [SerializeField] private float frameThickness = 0.18f;

    private const string ReflectionName = "Reflection Visual";
    private const string RuntimeSurfaceName = "Runtime Mirror Surface";
    private const string ReflectionSortingLayer = "player";
    private const int GlassSortingOrder = 150;
    private const int ReflectionSortingOrder = 175;
    private const int FrameSortingOrder = 250;

    private static Sprite runtimeSprite;

    private SkeletonAnimation reflectionVisual;
    private GameObject reflectionObject;
    private Spine.Animation lastAnimation;

    void Awake()
    {
        ResolvePlayer();
        BuildRuntimeSurface();
        BuildReflection();
    }

    void LateUpdate()
    {
        if (player == null || playerVisual == null)
        {
            ResolvePlayer();
            BuildReflection();
        }

        if (player == null || playerVisual == null || reflectionObject == null)
            return;

        bool shouldBeVisible = Mathf.Abs(player.transform.position.x - transform.position.x) <= activationDistance;
        if (reflectionObject.activeSelf != shouldBeVisible)
            reflectionObject.SetActive(shouldBeVisible);

        if (!shouldBeVisible)
            return;

        SynchronizeTransform();
        SynchronizeAnimation();
    }

    private void ResolvePlayer()
    {
        if (player == null)
            player = FindFirstObjectByType<PlayerController>();

        if (playerVisual == null && player != null)
            playerVisual = player.GetComponentInChildren<SkeletonAnimation>();
    }

    private void BuildReflection()
    {
        if (reflectionObject != null || playerVisual == null)
            return;

        reflectionObject = Instantiate(playerVisual.gameObject);
        reflectionObject.name = ReflectionName;
        reflectionObject.transform.SetParent(transform, true);

        CharacterAnimator characterAnimator = reflectionObject.GetComponent<CharacterAnimator>();
        if (characterAnimator != null)
            characterAnimator.enabled = false;

        reflectionVisual = reflectionObject.GetComponent<SkeletonAnimation>();
        reflectionVisual.Initialize(false);
        reflectionVisual.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        MeshRenderer renderer = reflectionObject.GetComponent<MeshRenderer>();
        renderer.sortingLayerName = ReflectionSortingLayer;
        renderer.sortingOrder = ReflectionSortingOrder;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        renderer.receiveShadows = false;

        CopyCurrentGear();
        if (reflectionObject.GetComponent<SinReflection>() == null)
            reflectionObject.AddComponent<SinReflection>();

        SynchronizeTransform();
        SynchronizeAnimation();
        reflectionObject.SetActive(false);
    }

    private void CopyCurrentGear()
    {
        GearEquipper source = playerVisual.GetComponent<GearEquipper>();
        GearEquipper target = reflectionObject.GetComponent<GearEquipper>();
        if (source == null || target == null)
            return;

        target.Job = source.Job;
        target.Melee = source.Melee;
        target.Shield = source.Shield;
        target.Bow = source.Bow;
        target.Quiver = source.Quiver;
        target.Staff = source.Staff;
        target.DuelistOffhand = source.DuelistOffhand;
        target.Armor = source.Armor;
        target.Helmet = source.Helmet;
        target.Shoulder = source.Shoulder;
        target.Arm = source.Arm;
        target.Feet = source.Feet;
        target.Hair = source.Hair;
        target.Face = source.Face;
        target.ApplySkinChanges();
    }

    private void SynchronizeTransform()
    {
        Vector3 sourcePosition = playerVisual.transform.position;
        reflectionObject.transform.position = new Vector3(
            sourcePosition.x + reflectionOffset.x,
            sourcePosition.y + reflectionOffset.y,
            transform.position.z);

        Vector3 sourceScale = playerVisual.transform.lossyScale;
        reflectionObject.transform.localScale = new Vector3(
            -sourceScale.x * reflectionScale,
            sourceScale.y * reflectionScale,
            sourceScale.z * reflectionScale);
        reflectionObject.transform.rotation = playerVisual.transform.rotation;
    }

    private void SynchronizeAnimation()
    {
        TrackEntry sourceTrack = playerVisual.AnimationState?.GetCurrent(0);
        if (sourceTrack?.Animation == null)
            return;

        TrackEntry reflectionTrack = reflectionVisual.AnimationState.GetCurrent(0);
        if (lastAnimation != sourceTrack.Animation || reflectionTrack == null)
        {
            reflectionTrack = reflectionVisual.AnimationState.SetAnimation(0, sourceTrack.Animation, sourceTrack.Loop);
            lastAnimation = sourceTrack.Animation;
        }

        reflectionTrack.TrackTime = sourceTrack.TrackTime;
        reflectionTrack.TimeScale = sourceTrack.TimeScale;
    }

    private void BuildRuntimeSurface()
    {
        if (!buildPlaceholderSurface || transform.Find(RuntimeSurfaceName) != null)
            return;

        Transform surface = new GameObject(RuntimeSurfaceName).transform;
        surface.SetParent(transform, false);

        CreateGlass(surface);
        CreateMask(surface);
        CreateFrame(surface);
    }

    private void CreateGlass(Transform parent)
    {
        SpriteRenderer glass = CreateSpriteRenderer("Glass", parent, glassColor, GlassSortingOrder);
        glass.transform.localScale = mirrorSize;
    }

    private void CreateMask(Transform parent)
    {
        GameObject maskObject = new GameObject("Reflection Mask");
        maskObject.transform.SetParent(parent, false);
        maskObject.transform.localScale = mirrorSize;

        SpriteMask mask = maskObject.AddComponent<SpriteMask>();
        mask.sprite = GetRuntimeSprite();
    }

    private void CreateFrame(Transform parent)
    {
        float halfWidth = mirrorSize.x * 0.5f;
        float halfHeight = mirrorSize.y * 0.5f;

        CreateFramePiece("Frame Left", parent, new Vector2(frameThickness, mirrorSize.y + frameThickness * 2f), new Vector2(-halfWidth - frameThickness * 0.5f, 0f));
        CreateFramePiece("Frame Right", parent, new Vector2(frameThickness, mirrorSize.y + frameThickness * 2f), new Vector2(halfWidth + frameThickness * 0.5f, 0f));
        CreateFramePiece("Frame Top", parent, new Vector2(mirrorSize.x, frameThickness), new Vector2(0f, halfHeight + frameThickness * 0.5f));
        CreateFramePiece("Frame Bottom", parent, new Vector2(mirrorSize.x, frameThickness), new Vector2(0f, -halfHeight - frameThickness * 0.5f));
    }

    private void CreateFramePiece(string objectName, Transform parent, Vector2 size, Vector2 position)
    {
        SpriteRenderer piece = CreateSpriteRenderer(objectName, parent, frameColor, FrameSortingOrder);
        piece.transform.localPosition = position;
        piece.transform.localScale = size;
    }

    private static SpriteRenderer CreateSpriteRenderer(string objectName, Transform parent, Color color, int sortingOrder)
    {
        GameObject child = new GameObject(objectName);
        child.transform.SetParent(parent, false);

        SpriteRenderer renderer = child.AddComponent<SpriteRenderer>();
        renderer.sprite = GetRuntimeSprite();
        renderer.color = color;
        renderer.sortingLayerName = ReflectionSortingLayer;
        renderer.sortingOrder = sortingOrder;
        return renderer;
    }

    private static Sprite GetRuntimeSprite()
    {
        if (runtimeSprite != null)
            return runtimeSprite;

        Texture2D texture = new Texture2D(1, 1, TextureFormat.RGBA32, false)
        {
            name = "Mirror Runtime Pixel",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp,
            hideFlags = HideFlags.HideAndDontSave
        };
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        runtimeSprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        runtimeSprite.name = "Mirror Runtime Sprite";
        runtimeSprite.hideFlags = HideFlags.HideAndDontSave;
        return runtimeSprite;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.9f, 1f, 0.9f);
        Gizmos.DrawWireCube(transform.position, new Vector3(mirrorSize.x, mirrorSize.y, 0f));
    }
}
