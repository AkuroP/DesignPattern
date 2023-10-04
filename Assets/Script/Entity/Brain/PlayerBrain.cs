using NaughtyAttributes;
using System.Collections;
using Game.Script;
using Game.Script.SoundManager;
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
    [SerializeField, BoxGroup("Input")] InputActionProperty _sphereUndo;

    [SerializeField, BoxGroup("Reference")]
    Transform _aim;

    [SerializeField, BoxGroup("Power Sphere")] private GameObject _spherePrefab;
    [SerializeField, BoxGroup("Power Sphere")] private float _sphereForce;
    private SphereMoveUndo _sphereMoveUndo;
    
    [SerializeField, BoxGroup("SFX")] private AudioClip _sphereSpawnSFX;
    [SerializeField, BoxGroup("SFX")] private AudioClip _dashSFX;
    [SerializeField, BoxGroup("SFX")] private AudioClip _hitSphereSFX;
    [SerializeField, BoxGroup("SFX")] private AudioClip _sphereUndoSFX;

    private GameObject _powerSphere;
    private Rigidbody2D _sphereRb;

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
        _sphereUndo.action.started += SphereUndo;
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
        _dashInput.action.started -= Dash;

        // Attack
        _fireInput.action.started -= Fire;

        //Power Sphere
        _spawnSphere.action.started -= UpdateSphere;
        _sphereUndo.action.started -= SphereUndo;
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
        ServiceLocator.Get().PlaySound(_dashSFX);
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

        if (!_powerSphere)
        {
            _powerSphere = Instantiate(_spherePrefab, transform.position + dir, Quaternion.identity);
            _sphereMoveUndo = _powerSphere.GetComponent<SphereMoveUndo>();
            _sphereRb = _powerSphere.GetComponent<Rigidbody2D>();
            
            ServiceLocator.Get().PlaySound(_sphereSpawnSFX);
            
            ICommand addForce = new SphereAddForce(_sphereRb, dir, _sphereForce);
            _sphereMoveUndo.AddCommand(addForce);
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 2f, layerMask: LayerMask.GetMask("Power")); ;

            if (hit.collider == null) return;

            if (_sphereRb.velocity.magnitude > 0.5f) return;
            
            ICommand addForce = new SphereAddForce(_sphereRb, dir, _sphereForce);
            _sphereMoveUndo.AddCommand(addForce);
            
            ServiceLocator.Get().PlaySound(_hitSphereSFX);
        }
    }

    private void SphereUndo(InputAction.CallbackContext obj)
    {
        if (_powerSphere == null || _sphereRb.velocity.magnitude > 0.5f) return;

        if (_sphereMoveUndo.CommandListCount == 0 || _sphereMoveUndo == null)
        {
            Destroy(_powerSphere);
            ServiceLocator.Get().PlaySound(_sphereSpawnSFX);
            
            return;
        }
        
        _sphereMoveUndo.UndoCommand();
        
        ServiceLocator.Get().PlaySound(_sphereUndoSFX);
    }
}
