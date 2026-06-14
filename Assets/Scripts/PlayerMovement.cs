using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    private Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isRunning = false;
    private float moveInput;
    public bool bisaGerak = true;

    private float moveInputAndroid = 0f;

    // ==========================================
    // SFX MOVEMENT
    // ==========================================
    [Header("SFX Movement")]
    public AudioClip sfxLangkah; // Suara langkah kaki
    private bool sedangMainSuaraLangkah = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!bisaGerak)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("isRunning", false);
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            StopSuaraLangkah();
            return;
        }

        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput == 0f) moveInput = moveInputAndroid;

        if (Input.GetKeyDown(KeyCode.R))
            isRunning = !isRunning;

        if (moveInput > 0f) sr.flipX = false;
        else if (moveInput < 0f) sr.flipX = true;

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isRunning", isRunning && Mathf.Abs(moveInput) > 0f);

        // ✅ Main SFX langkah kalau lagi gerak, stop kalau diam
        if (Mathf.Abs(moveInput) > 0f)
            PlaySuaraLangkah();
        else
            StopSuaraLangkah();
    }

    void FixedUpdate()
    {
        if (!bisaGerak)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
    }

    private void PlaySuaraLangkah()
    {
        if (sedangMainSuaraLangkah || sfxLangkah == null) return;
        if (AudioManager.Instance == null) return;
        sedangMainSuaraLangkah = true;
        // Pakai coroutine biar suara langkah loop dengan jeda natural
        StartCoroutine(LoopLangkah());
    }

    private void StopSuaraLangkah()
    {
        sedangMainSuaraLangkah = false;
        StopCoroutine(LoopLangkah());
    }

    private System.Collections.IEnumerator LoopLangkah()
    {
        while (sedangMainSuaraLangkah)
        {
            AudioManager.Instance?.PlaySFX(sfxLangkah);
            // Jeda antar langkah sesuai panjang clip, bisa disesuaiin
            yield return new WaitForSeconds(sfxLangkah.length);
        }
    }

    // =========================================================================
    // FUNGSI TOMBOL ANDROID
    // =========================================================================
    public void TekanJalanAndroid(float arah)
    {
        if (!bisaGerak) return;
        moveInputAndroid = arah;
    }

    public void LepasJalanAndroid()
    {
        moveInputAndroid = 0f;
    }

    public void MulaiJawab()
    {
        bisaGerak = false;
        moveInputAndroid = 0f;
        StopSuaraLangkah();
    }

    public void SelesaiJawab()
    {
        bisaGerak = true;
    }
}