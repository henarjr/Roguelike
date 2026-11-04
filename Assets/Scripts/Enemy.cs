using UnityEngine;
using System.Collections;

public class Enemy : CellObject
{
    public int Health = 3;
    private int m_CurrentHealth;

    private Animator m_Animator;
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    public float MoveSpeed = 5f;

    public UnityEngine.UI.Slider HealthSlider;

    private AudioSource audioSource;

    private void Awake()
    {
        GameManager.Instance.TurnManager.OnTick += TurnHappened;
        m_Animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            StartCoroutine(PlayZombieSoundWithDelay());
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.TurnManager != null)
        {
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
        }
    }

    public override void Init(Vector2Int coord)
    {
        base.Init(coord);
        m_CurrentHealth = Health;

        if (HealthSlider != null)
        {
            HealthSlider.maxValue = Health;
            HealthSlider.value = m_CurrentHealth;
        }

        transform.position = GameManager.Instance.BoardManager.CellToWorld(coord);
    }

    public override bool PlayerWantsToEnter()
    {
        m_CurrentHealth -= 1;

        if (HealthSlider != null)
        {
            HealthSlider.value = m_CurrentHealth;
        }

        if (m_CurrentHealth <= 0)
        {
            GameManager.Instance.AddZombieKill();

            Destroy(gameObject);
        }

        return false;
    }

    bool MoveTo(Vector2Int coord)
    {
        var board = GameManager.Instance.BoardManager;
        var targetCell = board.GetCellData(coord);

        if (targetCell == null || !targetCell.Passable)
        {
            return false;
        }

        if (targetCell.ContainedObject != null)
        {
            return false;
        }
        var currentCell = board.GetCellData(m_Cell);
        currentCell.ContainedObject = null;

        targetCell.ContainedObject = this;
        m_Cell = coord;

        m_MoveTarget = board.CellToWorld(coord);
        m_IsMoving = true;

        return true;
    }

    private void Update()
    {
        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);

            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
            }
        }
    }

    void TurnHappened()
    {
        var player = GameManager.Instance.PlayerController;
        var playerCell = player.Cell;

        int xDist = playerCell.x - m_Cell.x;
        int yDist = playerCell.y - m_Cell.y;
        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && absYDist == 1) || (yDist == 0 && absXDist == 1))
        {
            GameManager.Instance.ChangeFood(-3);

            if (m_Animator != null) m_Animator.SetTrigger("Attack");
            player.TakeDamage();
        }
        else
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist)) TryMoveInY(yDist);
            }
            else
            {
                if (!TryMoveInY(yDist)) TryMoveInX(xDist);
            }
        }
    }

    bool TryMoveInX(int xDist)
    {
        if (xDist > 0) return MoveTo(m_Cell + Vector2Int.right);
        return MoveTo(m_Cell + Vector2Int.left);
    }

    bool TryMoveInY(int yDist)
    {
        if (yDist > 0) return MoveTo(m_Cell + Vector2Int.up);
        return MoveTo(m_Cell + Vector2Int.down);
    }

    IEnumerator PlayZombieSoundWithDelay()
    {
        while (true)
        {
            if (audioSource.clip != null)
            {
                audioSource.Play();
                yield return new WaitForSeconds(audioSource.clip.length + 3f);
            }
            else
            {
                yield return new WaitForSeconds(3f);
            }
        }
    }
}