using SideScroller.Characters.States;
using SideScroller.Combat;
using SideScroller.Equipments;
using SideScroller.Farming;
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
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        // ======================================== Dependency ========================================
        private PlayerStateMachine _stateMachine;
        private Stat _stat;
        // === equipment ===
        private EquipmentSlot _equipmentSlot;
        [SerializeField] private EquipmentSO _startingEquipment;
        [SerializeField] private SpriteRenderer _equipmentRenderer;
        // === backpack ===
        // FIXLATER:
        private Inventory _backpack;
        [FormerlySerializedAs("_inventorySize")]
        [SerializeField] private int _backpackSize = 10;
        // === hotbar ===
        private Inventory _hotbar;
        [SerializeField] private int _hotbarSize = 5;
        [SerializeField] private float _dropDistance = 1.5f;

        // ===== unity =====
        private Collider2D _collider;
        private Rigidbody2D _body;
        private SpriteRenderer _playerSprite;
        private Camera _camera;
        private ContactFilter2D _groundFilter;
        private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[1];

        // ======================================== Etc ========================================
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private float _jumpForce = 9f;
        [SerializeField] private Transform _checkpoint;
        [SerializeField] private float _respawnDelay = 1.5f;


        private const float GroundCheckDistance = 0.05f;    // distance used by raycast to check the ground
        private const float DropSpread = 0.3f;              // gap between items when a stack is dropped

        // ======================================== getter ========================================
        // ====== 1. input ======
        public float MoveInput => PlayerInputSystem.MoveAxis;
        public bool JumpPressed => PlayerInputSystem.JumpPressedThisFrame;
        public bool AttackPressed => PlayerInputSystem.AttackPressedThisFrame;

        // ====== 2. state ======
        public void ChangeState(PlayerStateEnum next) => _stateMachine.ChangeState(next);
        public PlayerStateEnum PreviousStateType => _stateMachine.PreviousType;
        public PlayerStateEnum StateType => _stateMachine.CurrentType;

        // ====== 3. jump state ======
        // Cast collider below to check ground state
        public bool IsGrounded => _collider.Cast(Vector2.down, _groundFilter, _groundHits, GroundCheckDistance) > 0;
        public float VerticalVelocity => _body.linearVelocity.y;

        // ====== 4. dying ======
        public float RespawnDelay => _respawnDelay;

        // ====== 5. equipment ======
        public EquipmentSO EquippedItem => _equipmentSlot.SO;
        public bool HasEquipment => _equipmentSlot.HasEquipment;

        public void Equip(EquipmentSO equipment) => _equipmentSlot.Equip(equipment);
        public void Unequip() => _equipmentSlot.Unequip();

        // ====== 6. inventory ======
        public Inventory Backpack => _backpack ??= new Inventory(_backpackSize);
        public Inventory Hotbar => _hotbar ??= new Inventory(_hotbarSize);

        // while UI is open, player's tools is locked from activate
        // e.g. if backpack is open, wand can't attack 
        public bool ToolsLocked { get; set; }

        // ====== 7. aim ======
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

            _groundFilter = new ContactFilter2D();
            _groundFilter.useTriggers = false;

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

            if (AttackPressed && !ToolsLocked) ActivateTool();
        }

        // ======================================== state machine ========================================
        // ==== movement ====
        public void MoveHorizontal(float input)
        {
            _body.linearVelocity = new Vector2(input * _moveSpeed, _body.linearVelocity.y);
        }

        public void StopHorizontal()
        {
            _body.linearVelocity = new Vector2(0f, _body.linearVelocity.y);
        }

        public void ApplyJumpForce()
        {
            _body.linearVelocity = new Vector2(_body.linearVelocity.x, _jumpForce);
        }

        public void FaceMoveDirection(float input)
        {
            if (_playerSprite == null) return;
            if (Mathf.Approximately(input, 0f)) return;

            _playerSprite.flipX = input < 0f;
        }

        // ==== combat ====
        // every tool activates the same way, the tool decides what that means
        public void ActivateTool()
        {
            // the equipped item's behaviour, spawned by the slot
            Equipment tool = _equipmentSlot.CurrentEquipment;
            if (tool == null) return;

            tool.Activate(GetAimDirection());
        }

        // ==== dying ====
        // Respawn player at the checkpoint
        public void Respawn()
        {
            if (_checkpoint != null) transform.position = _checkpoint.position;

            if (_stat != null) _stat.ResetHealth();
        }

        // ==== animator ====
        // no Animator yet
        public void PlayAnimation(PlayerStateEnum state) { }


        // ======================================== inventory ========================================
        // take the whole stack out of the slot and drop it on the floor in front of the player
        public void DropItem(Inventory from, int slot)
        {
            ItemStack stack = from.GetStack(slot);
            if (stack == null) return;

            Vector2 front = (Vector2)transform.position + new Vector2(FacingDirection * _dropDistance, 0f);

            // drop item = spawn item into the world
            int dropped = 0;
            for (int i = 0; i < stack.Count; i++)
            {
                if (ItemPickup.Spawn(stack.Item, front + new Vector2(i * DropSpread, 0f)) == null) break;
                dropped++;
            }

            // remove dropped item from inventory
            if (dropped > 0) from.Remove(slot, dropped);
        }

        // use the item in the slot. what "use" means depends on the item's category:
        // Tool     => equip it. it stays in the slot, left click uses it
        // Seed     => plant it where the pointer is
        // Resource, Crafted => nothing, it stays in the slot
        public void UseItem(Inventory from, int slot)
        {
            IInventoryable item = from.Get(slot);
            if (item == null) return;

            switch (item.Category)
            {
                case ItemCategoryEnum.Tool:
                    if (item is EquipmentSO tool) Equip(tool);
                    break;

                case ItemCategoryEnum.Seed:
                    TryPlant(from, slot, GetPointerWorldPosition());
                    break;
            }
        }

        // plant the seed on the ground near the pointer. 
        private void TryPlant(Inventory from, int slot, Vector2 worldPos)
        {
            // check if it was a seed
            if (from.Get(slot) is not SeedSO seed) return;

            // try plant it
            if (Plant.TryPlant(seed, worldPos) == null) return;

            // if success, take one seed off the stack
            from.Remove(slot);
        }


    }
}
