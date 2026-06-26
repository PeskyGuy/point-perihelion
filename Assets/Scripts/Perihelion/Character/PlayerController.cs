using UnityEngine;
using UnityEngine.InputSystem;

namespace Perihelion.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float defaultMoveSpeed = 5f;

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;

        private Rigidbody2D _rb;
        private CharacterRenderer _renderer;
        private CharacterEntity _entity;
        private Vector2 _moveInput;
        private Vector2 _lastMoveDirection = Vector2.down;
        private Vector2 _aimDirection = Vector2.down;
        private InputActionMap _playerMap;
        private InputAction _moveAction;
        private InputAction _skill1Action;
        private InputAction _skill2Action;

        private float _lastSkill1Time = -999f;
        private float _lastSkill2Time = -999f;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;

            _renderer = GetComponent<CharacterRenderer>();
            if (_renderer == null) _renderer = gameObject.AddComponent<CharacterRenderer>();

            _entity = GetComponent<CharacterEntity>();

            if (inputActions != null)
            {
                _playerMap = inputActions.FindActionMap("Player");
                _moveAction = _playerMap?.FindAction("Move");
                _skill1Action = _playerMap?.FindAction("Skill1");
                _skill2Action = _playerMap?.FindAction("Skill2");
            }
        }

        private void Start()
        {
            if (_entity != null && !string.IsNullOrEmpty(_entity.defName))
            {
                _renderer.Initialize(_entity.defName);
            }
        }

        private void OnEnable()
        {
            _playerMap?.Enable();
            if (_skill1Action != null) _skill1Action.performed += OnSkill1;
            if (_skill2Action != null) _skill2Action.performed += OnSkill2;
        }

        private void OnDisable()
        {
            if (_skill1Action != null) _skill1Action.performed -= OnSkill1;
            if (_skill2Action != null) _skill2Action.performed -= OnSkill2;
            _playerMap?.Disable();
        }

        private void Update()
        {
            if (_moveAction != null)
            {
                _moveInput = _moveAction.ReadValue<Vector2>();
                if (_moveInput.sqrMagnitude > 0.01f)
                {
                    _lastMoveDirection = _moveInput.normalized;
                }
            }

            if (Camera.main != null && Mouse.current != null)
            {
                Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 0f));
                _aimDirection = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
            }
            else
            {
                _aimDirection = _lastMoveDirection;
            }

            if (_renderer != null)
            {
                bool isMoving = _moveInput.sqrMagnitude > 0.01f;
                _renderer.SetDirection(_aimDirection, isMoving);
            }
        }

        private void FixedUpdate()
        {
            float speed = _entity != null && _entity.Def != null ? _entity.Def.moveSpeed : defaultMoveSpeed;
            _rb.linearVelocity = _moveInput * speed;
        }

        private void OnSkill1(InputAction.CallbackContext context)
        {
            if (_entity == null || _entity.Def == null || string.IsNullOrEmpty(_entity.Def.defaultProjectileRef))
                return;

            string projectileRefId = _entity.Def.defaultProjectileRef;
            var projDef = Perihelion.Data.RefDatabase.Instance.Get<Perihelion.Refs.ProjectileRef>(projectileRefId);
            if (projDef == null || string.IsNullOrEmpty(projDef.prefabPath)) return;

            if (Time.time - _lastSkill1Time < projDef.cooldown) return;
            _lastSkill1Time = Time.time;

            GameObject prefab = Resources.Load<GameObject>(projDef.prefabPath);
            if (prefab != null)
            {
                GameObject proj = Instantiate(prefab, transform.position, Quaternion.identity);
                
                float charSize = _entity != null && _entity.Def != null ? _entity.Def.size : 1f;
                proj.transform.localScale = new Vector3(charSize, charSize, 1f);

                var entity = proj.GetComponent<Perihelion.Combat.ProjectileEntity>();
                if (entity != null)
                {
                    entity.Initialize(projectileRefId, _aimDirection, gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"[PlayerController] Could not load projectile prefab at '{projDef.prefabPath}' from Resources.");
            }
        }

        private void OnSkill2(InputAction.CallbackContext context)
        {
            if (_entity == null || _entity.Def == null || string.IsNullOrEmpty(_entity.Def.defaultSecondaryAttack))
                return;

            string projectileRefId = _entity.Def.defaultSecondaryAttack;
            var projDef = Perihelion.Data.RefDatabase.Instance.Get<Perihelion.Refs.ProjectileRef>(projectileRefId);
            if (projDef == null || string.IsNullOrEmpty(projDef.prefabPath)) return;

            if (Time.time - _lastSkill2Time < projDef.cooldown) return;
            _lastSkill2Time = Time.time;

            GameObject prefab = Resources.Load<GameObject>(projDef.prefabPath);
            if (prefab != null)
            {
                GameObject proj = Instantiate(prefab, transform.position, Quaternion.identity);
                
                float charSize = _entity != null && _entity.Def != null ? _entity.Def.size : 1f;
                proj.transform.localScale = new Vector3(charSize, charSize, 1f);

                var entity = proj.GetComponent<Perihelion.Combat.ProjectileEntity>();
                if (entity != null)
                {
                    entity.Initialize(projectileRefId, _aimDirection, gameObject);
                }
            }
            else
            {
                Debug.LogWarning($"[PlayerController] Could not load secondary attack prefab at '{projDef.prefabPath}' from Resources.");
            }
        }
    }
}
