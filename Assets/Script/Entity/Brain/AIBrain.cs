using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ENEMY_STATES
{
    ES_IDLE,
    ES_PATROL,
    ES_PURCHASE,
    ES_ATTACK
}

public class AIBrain : MonoBehaviour
{
    [SerializeField, BoxGroup("Special Dependency")] PlayerReference _playerEntity;

    [SerializeField, BoxGroup("Dependencies")] Entity _root;
    [SerializeField, BoxGroup("Dependencies")] EntityMovement _movement;
    [SerializeField, BoxGroup("Dependencies")] EntityAttack _attack;

    [SerializeField, BoxGroup("Conf")] float _distanceDetection;
    [SerializeField, BoxGroup("Conf")] float _distanceAttack;
    [SerializeField, BoxGroup("Conf")] float _stopDistance;
    [SerializeField, BoxGroup("Conf")] private float _fireSpeed = 10f;

    ENEMY_STATES _currentState;

    bool IsPlayerNear => Vector3.Distance(_root.transform.position, _playerEntity.Instance.transform.position) < _distanceDetection;
    bool IsPlayerInAttackRange => Vector3.Distance(_root.transform.position, _playerEntity.Instance.transform.position) < _distanceAttack;
    bool IsPlayerTooNear => Vector3.Distance(_root.transform.position, _playerEntity.Instance.transform.position) < _stopDistance;

    bool _playerInSight = false;

    #region EDITOR
#if UNITY_EDITOR
    void Reset()
    {
        _playerEntity = null;
        _root = GetComponentInParent<Entity>();
        _movement = _root.GetComponentInChildren<EntityMovement>();
        _distanceDetection = 3f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_root.transform.position, _distanceDetection);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_root.transform.position, _distanceAttack);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_root.transform.position, _stopDistance);
    }
#endif
    #endregion

    private void Start()
    {
        _currentState = ENEMY_STATES.ES_IDLE;
    }

    private void Update()
    {
        RaycastHit2D rh2D = Physics2D.Raycast(this.transform.position, this.transform.position - this.transform.position);
        if (rh2D.collider.gameObject.name == "Player")
        {
            _playerInSight = true;
        }
        // Move To Player
        else if (IsPlayerNear)
        {
            _movement.MoveToward(_playerEntity.Instance.transform);
        }
        // Stay idle
        else
        {
            _playerInSight = false;
        }


        // AI State Machine
        switch(_currentState)
        {
            case ENEMY_STATES.ES_IDLE:
                if(IsPlayerNear && _playerInSight)
                {
                    _currentState = ENEMY_STATES.ES_PURCHASE;
                    break;
                }
                else if(IsPlayerInAttackRange && _playerInSight)
                {
                    _currentState = ENEMY_STATES.ES_ATTACK;
                    break;
                }
                _movement.Move(Vector2.zero);
                break;

            case ENEMY_STATES.ES_PATROL:
                break;

            case ENEMY_STATES.ES_PURCHASE:
                if (!IsPlayerNear)
                {
                    _currentState = ENEMY_STATES.ES_IDLE;
                    break;
                }
                else if(IsPlayerInAttackRange)
                {
                    _currentState = ENEMY_STATES.ES_ATTACK;
                    break;
                }
                _movement.MoveToward(_playerEntity.Instance.transform);
                break;

            case ENEMY_STATES.ES_ATTACK:
                if (!IsPlayerInAttackRange || !_playerInSight)
                {
                    _currentState = ENEMY_STATES.ES_PURCHASE;
                    break;
                }
                _movement.Move(Vector2.zero);
                //Attack
                _attack.Fire((_playerEntity.Instance.transform.position - this.transform.position).normalized, _fireSpeed) ;
                //Debug.Log(_playerEntity.Instance.transform.position);
                break;
        }
    }


}
