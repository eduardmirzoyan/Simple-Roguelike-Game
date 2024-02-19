using System;
using System.Collections;
using UnityEngine;

public class EntityRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator aggroAnimator;

    [Header("Animation")]
    [SerializeField] private Sprite[] animationSprites;
    [SerializeField] private int frameIndex;
    [SerializeField] private float idleFrameDuration;
    [SerializeField] private float runFrameDuration;
    [SerializeField] private float hitFrameDuration;
    [SerializeField] private float deathFrameDuration;

    [Header("Settings")]
    [SerializeField] private float waitDuration = 0.5f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float attackDuration = 0.25f;
    [SerializeField] private float leanAngle;
    [SerializeField] private float attackPower;
    [SerializeField] private float hitStrength;

    [Header("Sound")]
    [SerializeField] private float moveSFXRate = 0.25f;

    [Header("Debug")]
    [SerializeField, ReadOnly] private EntityData entityData;
    [SerializeField, ReadOnly] private string currentAnimation;

    private Coroutine animationRoutine;

    public void Initialize(EntityData entityData)
    {
        this.entityData = entityData;
        animationSprites = entityData.idleSprites;
        entityData.entityRenderer = this;
        spriteRenderer.sprite = animationSprites[0];

        if (animationSprites.Length > 1)
            animationRoutine = StartCoroutine(IdleAnimation());

        gameObject.name = entityData.name + " Model";
    }

    private IEnumerator IdleAnimation()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        currentAnimation = "Idle";

        LeanTween.cancel(spriteRenderer.gameObject);
        spriteRenderer.gameObject.transform.localScale = Vector3.one;
        while (true)
        {
            spriteRenderer.sprite = animationSprites[frameIndex];
            yield return new WaitForSeconds(idleFrameDuration);
            frameIndex++;
            if (frameIndex >= animationSprites.Length)
                frameIndex = 0;
        }
    }

    private IEnumerator RunAnimation()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        bool alternate = false;
        currentAnimation = "Run";

        while (true)
        {
            spriteRenderer.sprite = animationSprites[frameIndex];

            // Squish n Stretch
            if (alternate)
                LeanTween.scale(spriteRenderer.gameObject, new Vector3(0.95f, 1.05f), runFrameDuration).setEaseLinear();
            else
                LeanTween.scale(spriteRenderer.gameObject, new Vector3(1.05f, 0.95f), runFrameDuration).setEaseLinear();
            alternate = !alternate;

            yield return new WaitForSeconds(runFrameDuration);

            frameIndex++;
            if (frameIndex >= animationSprites.Length)
                frameIndex = 0;
        }
    }

    private IEnumerator HitAnimation(Vector3Int sourcePosition)
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        currentAnimation = "Hit";

        Vector3 direction = entityData.tileData.position - sourcePosition;
        direction.Normalize();

        Vector3 knockbackOffset = transform.position + direction * hitStrength;
        Vector3 origin = transform.position;

        transform.position = knockbackOffset;
        LeanTween.move(gameObject, origin, hitFrameDuration).setEaseOutQuint();
        spriteRenderer.sprite = entityData.hitSprite;
        yield return new WaitForSeconds(hitFrameDuration);

        yield return IdleAnimation();
    }

    private IEnumerator DeathAnimation()
    {
        if (animationRoutine != null)
            StopCoroutine(animationRoutine);

        currentAnimation = "Die";

        frameIndex = 0;
        var deathSprites = entityData.deathSprites;
        while (frameIndex < deathSprites.Length)
        {
            spriteRenderer.sprite = deathSprites[frameIndex];

            yield return new WaitForSeconds(deathFrameDuration);

            frameIndex++;
        }

        Destroy(gameObject);
    }

    public IEnumerator WaitOverTime()
    {
        yield return new WaitForSeconds(waitDuration);
    }

    public IEnumerator MoveOverTime(Vector3Int direction)
    {
        Vector3 start = transform.position;
        Vector3 end = transform.position + direction;

        FaceDirection(direction);

        animationRoutine = StartCoroutine(RunAnimation());

        if (direction.x > 0)
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, leanAngle);
        else if (direction.x < 0)
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, -leanAngle);
        else
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, spriteRenderer.flipX ? -leanAngle : leanAngle);

        // Play sound
        InvokeRepeating(nameof(FootstepsSFX), 0f, moveSFXRate);

        // Move over time
        LeanTween.move(gameObject, end, moveDuration).setEaseInOutSine();
        yield return new WaitForSeconds(moveDuration);
        LeanTween.cancel(gameObject);

        // Stop sound
        CancelInvoke(nameof(FootstepsSFX));

        spriteRenderer.transform.rotation = Quaternion.identity;
        transform.position = end;

        animationRoutine = StartCoroutine(IdleAnimation());
    }

    public IEnumerator MeleeOverTime(TileData tileData)
    {
        Vector3 direction = tileData.position - entityData.tileData.position;
        direction.Normalize();

        FaceDirection(Vector3Int.RoundToInt(direction));

        LeanTween.move(gameObject, transform.position + direction * attackPower, attackDuration).setEasePunch();
        yield return new WaitForSeconds(attackDuration);
        LeanTween.cancel(gameObject);
    }

    public void TakeHit(Vector3Int sourcePosition)
    {
        AudioManager.instance.PlaySFX("Hit");
        animationRoutine = StartCoroutine(HitAnimation(sourcePosition));
    }

    public void Die()
    {
        AudioManager.instance.PlaySFX("Die");
        animationRoutine = StartCoroutine(DeathAnimation());
    }

    public void Aggro(bool state)
    {
        if (state)
            aggroAnimator.Play("Fade In");
        else
            aggroAnimator.Play("Fade Out");
    }

    private void FaceDirection(Vector3Int direction)
    {
        if (direction.x > 0)
            spriteRenderer.flipX = false;
        else if (direction.x < 0)
            spriteRenderer.flipX = true;
    }

    private void FootstepsSFX()
    {
        AudioManager.instance.PlaySFX("Move");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
