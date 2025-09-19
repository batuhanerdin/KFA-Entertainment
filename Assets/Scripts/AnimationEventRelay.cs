using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private AttackSystem attackSystem;

    private void Awake()
    {
        // Root objede AttackSystem’i bul
        attackSystem = GetComponentInParent<AttackSystem>();
    }

    // === Animation Event Fonksiyonlarý ===
    public void EnableHitbox()
    {
        attackSystem?.EnableHitbox();
    }

    public void DisableHitbox()
    {
        attackSystem?.DisableHitbox();
    }

    public void AttackEnd()
    {
        attackSystem?.AttackEnd();
    }
}
