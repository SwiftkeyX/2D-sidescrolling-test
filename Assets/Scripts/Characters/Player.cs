using SideScroller.Characters.States;
using SideScroller.Combat;
using SideScroller.Crafting;
using SideScroller.Equipments;
using SideScroller.Input;
using SideScroller.Inventories;
using UnityEngine;
using UnityEngine.Serialization;

namespace SideScroller.Characters
{
    /// <summary>
    /// Player don't have any logic inside it BUT:
    /// 1) It's the ONLY Monobehavior for the Player, so it's here so we could make player interact with Unity.
    /// 2) it act like a glue, which mean itself don't contain any real logic.
    /// Player is split over several files (partial class):
    /// - Player.cs              : Inspector fields, Unity life cycle, input, equipment, crafting, aim
    /// - Player.StateMachine.cs : what the states machine use - movement, ground check, respawn, animation
    /// - Player.Interact.cs     : pressing E on the closest interactable thing
    /// - Player.Inventory.cs    : backpack & hotbar, drop/use an item from a slot
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public partial class Player : MonoBehaviour
    {
        // ======================================== Dependency ========================================
        private PlayerStateMachine _stateMachine;
        private Stat _stat;
        // === equipment ===
        private EquipmentSlot _equipmentSlot;
        [SerializeField] private EquipmentSO _startingEquipment;
        [SerializeField] private SpriteRenderer _equipmentRenderer;
        // === backpack ===
        [FormerlySerializedAs("_inventorySize")]
        [SerializeField] private int _backpackSize = 10;
        // === hotbar ===
        [SerializeField] private int _hotbarSize = 5;
        [SerializeField] private float _dropDistance = 1.5f;
        // === crafting ===
        private Inventory _craftGrid;
        [SerializeField] private int _craftGridSize = 2;
        [SerializeField] private RecipeBookSO _recipeBook;

        // ===== unity =====
        private Collider2D _collider;
        private Rigidbody2D _body;
        private SpriteRenderer _playerSprite;
        private Camera _camera;

        // ======================================== Etc ========================================
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 9f;
        [SerializeField] private Transform _checkpoint;
        [SerializeField] private float _respawnDelay = 1.5f;
        [SerializeField] private float _interactReach = 1.5f;

        // ======================================== getter ========================================
        // ====== 1. input ======
        public float MoveInput => PlayerInputSystem.MoveAxis;
        public bool JumpPressed => PlayerInputSystem.JumpPressedThisFrame;
        public bool LeftClickPressed => PlayerInputSystem.AttackPressedThisFrame;
        public bool InteractPressed => PlayerInputSystem.InteractPressedThisFrame;

        // ====== 2. state ======
        public void ChangeState(PlayerStateEnum next) => _stateMachine.ChangeState(next);
        public PlayerStateEnum PreviousStateType => _stateMachine.PreviousType;
        public PlayerStateEnum StateType => _stateMachine.CurrentType;

        // ====== 3. equipment ======
        public EquipmentSO EquippedItem => _equipmentSlot.SO;
        public bool HasEquipment => _equipmentSlot.HasEquipment;

        public void Equip(EquipmentSO equipment) => _equipmentSlot.Equip(equipment);
        public void Unequip() => _equipmentSlot.Unequip();
        public bool ToolsLocked { get; set; }

        // ====== 4. crafting ======
        public Inventory CraftGrid => _craftGrid ??= new Inventory(_craftGridSize);
        public RecipeBookSO RecipeBook => _recipeBook;

        public Recipe LookupRecipe(Inventory grid) => _recipeBook == null ? null : _recipeBook.LookupRecipe(grid);
        public bool TryCraft(Inventory grid) => _recipeBook != null && _recipeBook.TryCraft(grid, Backpack);

        // ====== 5. aim ======
        public float FacingDirection => _playerSprite != null && _playerSprite.flipX ? -1f : 1f;

        // get direction from the player toward the pointer
        public Vector2 GetAimDirection()
        {
            if (_camera == null) return new Vector2(FacingDirection, 0f);

            Vector2 aim = GetPointerWorldPosition() - (Vector2)transform.position;

            return aim.normalized;
        }

        // where the pointer is in the world 
        public Vector2 GetPointerWorldPosition()
        {
            if (_camera == null) return transform.position;

            Vector3 screen = PlayerInputSystem.PointerScreenPosition;
            screen.z = -_camera.transform.position.z;

            return _camera.ScreenToWorldPoint(screen);
        }

        // ======================================== unity ========================================
        void Awake()
        {
            _body = GetComponent<Rigidbody2D>();
            _playerSprite = GetComponent<SpriteRenderer>();
            _camera = Camera.main;
            _collider = GetComponent<Collider2D>();
            _stat = GetComponent<Stat>();

            InitGroundCheck();

            _equipmentSlot = new EquipmentSlot(_equipmentRenderer, transform);
            if (_startingEquipment != null) _equipmentSlot.Equip(_startingEquipment);

            // init starting tool into the hotbar, the equipped tool lives in the quick access bar
            if (_startingEquipment != null) Hotbar.TryAdd(_startingEquipment);

            _stateMachine = new PlayerStateMachine(this, _stat);
        }

        void OnDestroy()
        {
            // destroy non-unity class
            _stateMachine?.Detach();
        }

        void Start()
        {
            _stateMachine.Start(PlayerStateEnum.Idle);
        }

        void Update()
        {
            _stateMachine.Tick();

            if (StateType == PlayerStateEnum.Dead) return;

            // try activate tool e.g. wand, axes
            if (LeftClickPressed && !ToolsLocked) ActivateTool();

            // try interact with Iinteractable e.g. Storage, Plant
            if (InteractPressed) TryInteract();
        }

        // ======================================== combat ========================================
        // every tool activates the same way, the tool decides what that means
        public void ActivateTool()
        {
            // the equipped item's behaviour, spawned by the slot
            Equipment tool = _equipmentSlot.CurrentEquipment;
            if (tool == null) return;

            tool.Activate(GetAimDirection());
        }
    }
}
