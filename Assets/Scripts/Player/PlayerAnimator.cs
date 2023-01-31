#region
using UnityEngine;
#endregion

public class PlayerAnimator : MonoBehaviour
{
    [Header("Movement Tilt")] [SerializeField] float maxTilt;
    [SerializeField] [Range(0, 1)] float tiltSpeed;

    [Header("Particle FX")] [SerializeField] GameObject jumpFX;
    [SerializeField] GameObject landFX;

    public float currentVelY;
    ParticleSystem _jumpParticle;
    ParticleSystem _landParticle;
    Animator anim;

    GameManager gameManager;
    PlayerMovement mov;
    SpriteRenderer spriteRend;

    public bool startedJumping { private get; set; }
    public bool justLanded { private get; set; }

    void Start()
    {
        mov        = GetComponent<PlayerMovement>();
        spriteRend = GetComponentInChildren<SpriteRenderer>();
        anim       = spriteRend.GetComponent<Animator>();

        gameManager = FindObjectOfType<GameManager>();

        _jumpParticle = jumpFX.GetComponent<ParticleSystem>();
        _landParticle = landFX.GetComponent<ParticleSystem>();
    }

    void LateUpdate()
    {
        #region Tilt
        float tiltProgress;

        int mult = -1;

        if (mov.IsSliding) { tiltProgress = 0.25f; }
        else
        {
            tiltProgress = Mathf.InverseLerp(-mov.Data.runMaxSpeed, mov.Data.runMaxSpeed, mov.RB.velocity.x);
            mult         = mov.IsFacingRight ? 1 : -1;
        }

        float newRot = tiltProgress * maxTilt * 2 - maxTilt;
        float rot    = Mathf.LerpAngle(spriteRend.transform.localRotation.eulerAngles.z * mult, newRot, tiltSpeed);
        spriteRend.transform.localRotation = Quaternion.Euler(0, 0, rot * mult);
        #endregion

        CheckAnimationState();

        ParticleSystem.MainModule jumpPSettings = _jumpParticle.main;
        jumpPSettings.startColor = new ParticleSystem.MinMaxGradient(gameManager.SceneData.foregroundColor);
        ParticleSystem.MainModule landPSettings = _landParticle.main;
        landPSettings.startColor = new ParticleSystem.MinMaxGradient(gameManager.SceneData.foregroundColor);
    }

    void CheckAnimationState()
    {
        if (startedJumping)
        {
            anim.SetTrigger("Jump");

            GameObject obj = Instantiate(jumpFX, transform.position - Vector3.up * transform.localScale.y / 2,
                                         Quaternion.Euler(-90, 0, 0));

            Destroy(obj, 1);
            startedJumping = false;
            return;
        }

        if (justLanded)
        {
            anim.SetTrigger("Land");

            GameObject obj = Instantiate(landFX, transform.position - Vector3.up * transform.localScale.y / 1.5f,
                                         Quaternion.Euler(-90, 0, 0));

            Destroy(obj, 1);
            justLanded = false;
            return;
        }

        anim.SetFloat("Vel Y", mov.RB.velocity.y);
    }
}
