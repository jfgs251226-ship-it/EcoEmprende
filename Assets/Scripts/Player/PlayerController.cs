using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private bool isTopDown = false;
    private Rigidbody2D rb;
    private Animator animador;
    private Vector2 entrada;
    private bool mirandoDerecha = true;
    private EcoHUD pantalla;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = isTopDown ? 0f : 3f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        CapsuleCollider2D capsule = GetComponent<CapsuleCollider2D>();
        if (capsule == null) capsule = gameObject.AddComponent<CapsuleCollider2D>();
        capsule.size = new Vector2(0.57f, 0.89f);
        capsule.offset = new Vector2(0f, 0.45f);
        animador = GetComponent<Animator>();
    }

    private void Start() { pantalla = FindFirstObjectByType<EcoHUD>(); }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (pantalla != null && pantalla.EstaBloqueado) horizontal = 0f;
        float vertical = isTopDown ? Input.GetAxisRaw("Vertical") : 0f;
        if (pantalla != null && pantalla.EstaBloqueado) vertical = 0f;
        entrada = new Vector2(horizontal, vertical).normalized;
        if (horizontal > 0f && !mirandoDerecha) Voltear();
        if (horizontal < 0f && mirandoDerecha) Voltear();
        if (animador != null) animador.SetFloat("Speed", entrada.sqrMagnitude);
    }
    private void FixedUpdate()
    {
        if (rb == null) return;
        rb.linearVelocity = isTopDown ? entrada * moveSpeed
           : new Vector2(entrada.x * moveSpeed, rb.linearVelocity.y);
    }
    private void Voltear()
    {
        mirandoDerecha = !mirandoDerecha;
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }
}
