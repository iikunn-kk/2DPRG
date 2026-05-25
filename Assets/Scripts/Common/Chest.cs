using UnityEngine;

/// <summary>
/// 宝箱交互组件
/// 实现 IInteractable 接口，支持开箱与掉落物生成
/// </summary>
public class Chest : MonoBehaviour, IInteractable
{
    [Header("宝箱外貌")]
    public Sprite openSprite;
    public Sprite closeSprite;

    [Header("掉落物")]
    public GameObject bloodBottle;

    [Header("状态")]
    public bool isDone;

    [Header("掉落偏移")]
    [SerializeField] private Vector3 _dropOffset = new Vector3(0, 3, 0);

    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.sprite = isDone ? openSprite : closeSprite;
    }

    public void TriggerAction()
    {
        if (!isDone)
            OpenChest();
    }

    private void OpenChest()
    {
        if (_spriteRenderer != null)
            _spriteRenderer.sprite = openSprite;

        isDone = true;
        gameObject.tag = "Untagged";

        if (bloodBottle != null)
            Instantiate(bloodBottle, transform.position + _dropOffset, Quaternion.identity);
    }
}
