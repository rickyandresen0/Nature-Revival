using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float speed;
    [SerializeField] private float patrolDistance;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float stopChaseRange = 12f;

    private float startingPosX;
    private bool movingRight = true;
    private Transform player;
    private EnemyOneAttack attackScript;

    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private void Awake()
    {
        startingPosX = transform.position.x;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogWarning("[EnemyMovement] Tag 'Player' tidak ditemukan!", this);

        attackScript = GetComponent<EnemyOneAttack>();
    }

    private void Update()
    {
        if (player == null) return;

        float distToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                if (distToPlayer <= chaseRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (distToPlayer > stopChaseRange)
                    currentState = State.Patrol;
                else if (attackScript != null && distToPlayer <= attackScript.Range)
                    currentState = State.Attack;
                break;

            case State.Attack:
                if (distToPlayer > attackScript.Range * 1.2f)
                    currentState = State.Chase;
                break;
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (attackScript != null) attackScript.SetAttackEnabled(false);
                break;

            case State.Chase:
                ChasePlayer();
                if (attackScript != null) attackScript.SetAttackEnabled(false);
                break;

            case State.Attack:
                StopAndFacePlayer();
                if (attackScript != null) attackScript.SetAttackEnabled(true);
                break;
        }
    }

    private void Patrol()
    {
        if (movingRight)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
            if (transform.position.x >= startingPosX + patrolDistance)
                Flip();
        }
        else
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
            if (transform.position.x <= startingPosX - patrolDistance)
                Flip();
        }
    }

    private void ChasePlayer()
    {
        float dir = player.position.x - transform.position.x;
        transform.position += Vector3.right * Mathf.Sign(dir) * chaseSpeed * Time.deltaTime;
        FaceDirection(dir);
    }

    private void StopAndFacePlayer()
    {
        float dir = player.position.x - transform.position.x;
        FaceDirection(dir);
    }

    private void Flip()
    {
        movingRight = !movingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void FaceDirection(float dirX)
    {
        if (dirX > 0f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (dirX < 0f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);
    }
}