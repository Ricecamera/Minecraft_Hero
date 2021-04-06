using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player State
    public int health = 3;
    public bool isDead;

    //Player model and animation
    public GameObject playerAvatar;
    public ParticleSystem DeathParticle;
    private Animator playerAnim;

    private GameManager gameManager;
    private InputHandler _input;
    private Rigidbody playerRb;
    public Transform respawnPos;

    // Audio
    private AudioSource p_audiosource;
    public AudioClip hurtSound;

    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private Camera sceneCamera;

    private float xBound = 26.6f;
    private float zBound = 15.0f;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        _input = GetComponent<InputHandler>();
        playerRb = GetComponent<Rigidbody>();
        playerAnim = playerAvatar.GetComponent<Animator>();
        p_audiosource = GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameManager.isGameOver)
        {
            ResetState();
            return;
        }
        // Change 2d input vector to 3d vector;
        if (!isDead)
        {
            var targetVector = new Vector3(_input.inputVector.x, 0, _input.inputVector.y);
            MoveTowardTragetVector(targetVector);
            RotateTowardMouse();
        }
        
    }

    private void MoveTowardTragetVector(Vector3 des) 
    {
        float speed = movementSpeed * Time.deltaTime;
        var tragetPosition = transform.position + des * speed;
        tragetPosition = CheckOutofBound(tragetPosition);
        transform.position = tragetPosition;
    }

    private void RotateTowardMouse()
    {
        Ray ray = sceneCamera.ScreenPointToRay(_input.mousePosition);

        // Find Mouse position and make player look at that position
        if (Physics.Raycast(ray, out RaycastHit hitinfo, maxDistance: 300f))
        {
            var target = hitinfo.point;
            target.y = transform.position.y; // Normalize y component
            transform.LookAt(target);
        }
    }

    private Vector3 CheckOutofBound(Vector3 targetPosition)
    {
        if (targetPosition.x > xBound)
        {
            targetPosition.x = xBound;
        } else if (targetPosition.x < -xBound)
        {
            targetPosition.x = -xBound;
        }

        if (targetPosition.z > zBound)
        {
            targetPosition.z = zBound;
        }
        else if (targetPosition.z < -zBound)
        {
            targetPosition.z = -zBound;
        }

        return targetPosition;
    }

    public void TakeDamge(int damage)
    {
        p_audiosource.PlayOneShot(hurtSound);
        health -= damage;
        if (health <= 0)
        {
            ResetState();
            OnDeathEntry();
        }
    }

    private void ResetState()
    {
        playerRb.velocity = Vector3.zero;
        playerRb.angularVelocity = Vector3.zero;
    }
    private void OnDeathEntry()
    {
        isDead = true;
        DeathParticle.Play();
        playerAnim.SetBool("Death_b", true);
        StartCoroutine(Respawn());
        
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(2.2f);
        health = 3;
        playerRb.rotation = Quaternion.identity;
        transform.position = respawnPos.position;
        playerAnim.SetBool("Death_b", false);
        isDead = false;
    }
}
