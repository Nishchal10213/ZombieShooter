using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed = 1.5f;
    [SerializeField] private float _health = 100f;
    [SerializeField] private GameObject _gameOverText; // 👈 Game Over Text Reference

    private Animator _anim;

    void Start()
    {
        _agent.speed = _speed;
        _anim = GetComponent<Animator>();
        if (_gameOverText != null)
            _gameOverText.SetActive(false); // Hide on start
    }

    private void Update()
    {
        if (_agent.enabled)
            _agent.SetDestination(_target.position);
    }

    public void Damage(int amount)
    {
        _health -= amount;
        _agent.speed = 0;
        _anim.SetTrigger("Hit");

        if (_health < 1)
        {
            _agent.speed = 0;
            _anim.SetTrigger("Dead");
            _agent.enabled = false;

            if (_gameOverText != null)
                _gameOverText.SetActive(true); // 👈 Show Game Over
        }
        else
        {
            StartCoroutine(HitRoutine());
        }
    }

    IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        _agent.speed = _speed;
    }
}
