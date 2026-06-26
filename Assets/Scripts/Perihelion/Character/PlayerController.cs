using UnityEngine;
using UnityEngine.InputSystem;

namespace Perihelion.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;

        private Rigidbody2D _rb;
        private CharacterSpriteSwapper _spriteSwapper;
        private Vector2 _moveInput;
        private InputActionMap _playerMap;
        private InputAction _moveAction;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;

            _spriteSwapper = GetComponent<CharacterSpriteSwapper>();

            if (inputActions != null)
            {
                _playerMap = inputActions.FindActionMap("Player");
                _moveAction = _playerMap?.FindAction("Move");
            }
        }

        private void OnEnable()
        {
            _playerMap?.Enable();
        }

        private void OnDisable()
        {
            _playerMap?.Disable();
        }

        private void Update()
        {
            if (_moveAction != null)
                _moveInput = _moveAction.ReadValue<Vector2>();

            if (_spriteSwapper != null)
                _spriteSwapper.SetDirection(_moveInput);
        }

        private void FixedUpdate()
        {
            _rb.linearVelocity = _moveInput * moveSpeed;
        }
    }
}
