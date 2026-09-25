using SideScroller.Characters.States;
using SideScroller.Combat;
using SideScroller.Crafting;
using SideScroller.Equipments;
using SideScroller.Farming;
using SideScroller.Input;
using SideScroller.Interactions;
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
    /// - Player.cs              : Inspector fields, Unity life cycle, input, equipment, inventory
    /// - Player.StateMachine.cs : what the states machine use - movement, ground check, respawn, animation
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
        private Inventory _backpack;
        [FormerlySerializedAs("_inventorySize")]
        [SerializeField] private int _backpackSize = 10;
        // === hotbar ===
        private Inventory _hotbar;
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


        private const float DropSpread = 0.3f;              // gap between items when a stack is dropped

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

        // ====== 4. inventory ======
        public Inventory Backpack => _backpack ??= new Inventory(_backpackSize);
        public Inventory Hotbar => _hotbar ??= new Inventory(_hotbarSize);

        // ====== 5. crafting ======
        public Inventory CraftGrid => _craftGrid ??= new Inventory(_craftGridSize);
        public RecipeBookSO RecipeBook => _recipeBook;

        public Recipe LookupRecipe(Inventory grid) => _recipeBook == null ? null : _recipeBook.LookupRecipe(grid);
        public bool TryCraft(Inventory grid) => _recipeBook != null && _recipeBook.TryCraft(grid, Backpack);

        // ====== 6. aim ======
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

        // ======================================== interact ========================================
        // press E on the closest interactable thing within reach
        public void TryInteract()
        {
            Vector2 from = transform.position;
            IInteractable closest = null;
            float best = float.MaxValue;

            // chcek if my position reach any interactable 
            foreach (Collider2D hit in Physics2D.OverlapCircleAll(from, _interactReach))
            {
                IInteractable target = hit.GetComponentInParent<IInteractable>();
                if (target == null) continue;

                // measured to the collider's edge
                float distance = Vector2.Distance(from, hit.ClosestPoint(from));
                if (distance >= best) continue;

                best = distance;
                closest = target;
            }

            // interact with the cloest one
            closest?.Interact(this);
        }

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

        // use the item in the slot. what "use" means depends on what kind of item it is (its type, not its category):
        // EquipmentSO => equip it. it stays in the slot, left click uses it
        // SeedSO      => plant it where the pointer is
        // PlaceableSO => put it down where the pointer is, e.g. storage chest
        // anything else, e.g. Lumber, Golden Veggie => nothing, it stays in the slot
        public void UseItem(Inventory from, int slot)
        {
            switch (from.Get(slot))
            {
                case EquipmentSO tool:
                    Equip(tool);
                    break;

                case SeedSO seed:
                    TryPlant(seed, from, slot, GetPointerWorldPosition());
                    break;

                case PlaceableSO placeable:
                    TryPlace(placeable, from, slot, GetPointerWorldPosition());
                    break;
            }
        }

        // plant the seed on the ground near the pointer. 
        private void TryPlant(SeedSO seed, Inventory from, int slot, Vector2 worldPos)
        {
            // try plant it
            if (Plant.TryPlant(seed, worldPos) == null) return;

            // if success, take one seed off the stack
            from.Remove(slot);
        }

        // put the item down on the ground near the pointer 
        // e.g. a storage chest
        private void TryPlace(PlaceableSO placeable, Inventory from, int slot, Vector2 worldPos)
        {
            // try place 
            if (placeable.TryPlace(worldPos) == null) return;

            // if success, the placed item leaves the slot
            from.Remove(slot);
        }
    }
}
