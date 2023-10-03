using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationWait : CustomYieldInstruction
{
    private Animation a;

    public AnimationWait(Animation a)
    {
        this.a = a;

        if (a.clip.isLooping) throw new System.Exception();

        a.Play();
    }

    public override bool keepWaiting => a.isPlaying;
}

public static class AnimationExtension
{
    public static AnimationWait PlayAndWaitCompletion(this Animation @this)
        => new AnimationWait(@this);
}



public class PlayerBrain : MonoBehaviour
{
    [SerializeField, BoxGroup("Dependencies")]
    EntityMovement _movement;

    [SerializeField, BoxGroup("Dependencies")]
    EntityAttack _attack;

    [SerializeField, BoxGroup("Input")] InputActionProperty _moveInput;
    [SerializeField, BoxGroup("Input")] InputActionProperty _dashInput;
    [SerializeField, BoxGroup("Input")] InputActionProperty _attackInput;
    [SerializeField, BoxGroup("Input")] InputActionProperty _fireInput;
    [SerializeField, BoxGroup("Input")] InputActionProperty _spawnSphere;

    [SerializeField, BoxGroup("Reference")]
    Transform _aim;

    [SerializeField, BoxGroup("Power Sphere")]
    GameObject _sphere;

    [SerializeField, BoxGroup("Power Sphere")]
    float _sphereForce;

    private bool _sphereAlreadySpawned;

    private GameObject _powerSphere;

    [SerializeField] private float _fireSpeed = 10f;

    private void Start()
    {
        // Move
        _moveInput.action.started += UpdateMove;
        _moveInput.action.performed += UpdateMove;
        _moveInput.action.canceled += StopMove;

        //Dash
        _dashInput.action.started += Dash;

        // Attack
        //_attackInput.action.started += Attack;
        _fireInput.action.started += Fire;

        //Power Sphere
        _spawnSphere.action.started += UpdateSphere;
    }




    void run()
    {
        var speedbase = 10;
        var armurespeed = 1.3;
        var coffeefactor = 1.2f;


        var s = speedbase * armurespeed * coffeefactor;
    }





    Coroutine _maCoroutine;

    public void RunCoucou()
    {
        if (_maCoroutine != null) return;

        int i = 10;
        _maCoroutine = StartCoroutine(coucouRoutine());

        IEnumerator coucouRoutine()
        {
            Animation a = GetComponent<Animation>();
            yield return new AnimationWait(a);
            yield return a.PlayAndWaitCompletion();

            var wait = new WaitForSeconds(10f);
            i++;
            yield return wait;
            i++;
            yield return wait;
            i++;
            yield return wait;

            _maCoroutine = null;
            yield break;
        }
    }

    private void OnDestroy()
    {
        // Move
        _moveInput.action.started -= UpdateMove;
        _moveInput.action.performed -= UpdateMove;
        _moveInput.action.canceled -= StopMove;
    }


    private void UpdateMove(InputAction.CallbackContext obj)
    {
        _movement.Move(obj.ReadValue<Vector2>().normalized);
    }

    private void StopMove(InputAction.CallbackContext obj)
    {
        _movement.Move(Vector2.zero);
    }

    private void Dash(InputAction.CallbackContext obj)
    {
        _movement.Dash();
    }

    private void Fire(InputAction.CallbackContext obj)
    {
        if (this == null) return;
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition) - this.transform.position;
        _attack.Fire(target, _fireSpeed);
    }

    private void UpdateSphere(InputAction.CallbackContext obj)
    {
        Vector3 dir = (_aim.position - transform.position).normalized;

        if (!_sphereAlreadySpawned)
        {
            _powerSphere = Instantiate(_sphere, transform.position + dir, Quaternion.identity);
            _sphereAlreadySpawned = true;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 1.5f, layerMask: LayerMask.GetMask("Power"));

            Debug.DrawLine(transform.position, transform.position + dir * 1.5f);

            if (hit.collider == null) return;

            var sphereRb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
            sphereRb.AddForce(dir * _sphereForce);
        }
    }
}
