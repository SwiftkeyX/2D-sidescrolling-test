Deadline 25/9/69

# Day 0 (22/9/69) Planning

- init git repo
- understanding the requirements
- find free asset

Feature 0. player can move, jump

- [ ] use CharacterController2D for movement/jump
- [ ] implement FSM for clean code.

1. time hop triggers
   1. player walk into collider to trigger the time skip
      - [ ] simple collider trigger logic
   2. a clock show current time
      - [ ] simple overlay using only number
   3. time is divided into 3 periods: Morning, Afternoon, and Evening.
   4. the day was also separated into weekday Mon/Tue/Wed/etc...
   5. day is tracked infinitely: day0 => day1 => ... => day100+.
      - [ ] Use "time" float var to track in-game time.
      - [ ] Let say 5 minutes = each interval (Morning/Afternoon/Evening)
      - [ ] When the time is met, the light turn brighter/dimmer ? - no idea how to make daytime/nighttime in 2D game.
      - [ ] When the day cross between Evening => Morning, +1 to "today" int var

2. Inventory system
   1. player can collect/use/remove/organize items
      - [ ] make simple grid UI overlay for inventory window
      - [ ] Each cell can be put the item sprite inside
      - [ ] When player collect item, the item was added into the very first available grid.
      - [ ] Player can drag items sprite around in the inventory, can replace/swap each items by dragging on top of them
   2. inventory can hold resources, tools, crafted objects, and seeds.
      - [ ] Have several items in the game. Those items also was categorized.
      - [ ] make at least 3 items in each category  
             resource = lumber, carrot, red berry, corn  
             tools = watering can, shovel, axes  
             seed = carrot seed, red berry seed, corn seed  
             crafted item = Storage Chest, Golden Veggie, Broccoli
   3. inventory bar - a overlay bar at the bottom for quick access e.g. equipable, usable, or placeable
      - [ ] use simple grid UI overlay like inventory window
      - [ ] drag seeds from a bar on the ground to place it
      - [ ] drag shovel from a bar on the seed, make the seed grow to flower
      - [ ] that mean a seed is the class of it own. It have several variation of sprite when it was growth.
   4. items can stack and have limit at 10 max
      - [ ] Each cell in inventory keep a list<Stack>. Stack is class that keep a items variable and a number of the stack.

3. crafting system
   1. crafting is available from the "inventory" and "station crafting"
      - [ ] Make a Recipe class that hold every combination of each items. Let assume the crafting use only 3 item in total. The order while crafting doesn't matter.
      - [ ] Let station craftign have 3 craft cell
      - [ ] Let Inventory have 2 craft cell
   2. requirement
      - [ ] Craft a Storage Chest directly from items in the Inventory.
      - [ ] A Storage Chest requiring 10x Lumber, which can hold 30 items when crafted and placed
      - [ ] Let the Storage Chest use the same logic to inventory window but bigger. Inventory have 10 cell, Storage Chest have 30 cell.

4. combat system
   1. Player is equip with Wand. It can shoot projectile that does 5 damage.
      - [ ] when player attack, create a cube that drive itself forward non-stop. Let it destroy itself after 5 sec.
      - [ ] the cube has collider for doing damage
   2. Slime that can patrol, and attack player on found. When die, split into smaller slimes with 5 health.
      - [ ] make a clone statemachine for slime. add that he can patrol around, and chase player on found.
      - [ ] let him walk by jumping, his hitbox should be the same size as his sprite. We have animation of him jumping, could we make the animation drive hitbox?
      - [ ] player have 30 health. if slimes hit player, player lose 3 health.
      - [ ] how are we going to make slime patrol?
      - [ ] making slime split when he die. when slime die, create another 3 slime but scaling its size down.
      - [ ] when player die, teleport player back to home, reset HP

# Day 1

I am not confidence in making inventory. So I'll start with what I am confidence with which is: 1. player controller 2. combat system. 3. lastly, time hop system because it seem easy.

## Design player controller

**implement FSM for player (2hr):**

- [x] implement FSM for clean code = idle, walk, jump, attack
  - (7:00) I am trying to move my old FSM code to this project.  
    Let's make idle/walking work first.
  - (7:30) implement CharacterController2D for movement/jump.
  - (8:00) change to Rigidbody2D because there's no CharacterController2D.  
    implement new input system
  - (8:20) implement jump state  
    use collider.cast to check ground state  
    left the attack state alone, let implement weapon first

## Design combat system

**implement a equipment system (1hr):**

- [x] a equip weapon will show on player's head indicate that player was holding it.
  - (8:50) implement Equipment class & ScritableObject for the equipment
- [x] implement a wand:
  - (9:15) make it shoot a cube that drive itself forward  
    the cube have its own class, "bullet". it drive itself forward and destroy after 5 sec.
  - (9:30) implement wand class  
    make wand a child to equipment class.  
    implement shoot direction using mouse pointer

**implement slime part 1 (4hr):**

- [x] copy player controller to slime = idle, patrol, Chase. how are we going to make slime patrol?
  - (10:00) implement slime by copying from player controller: implement idle and chase state
  - (10:50) implement patrol state  
    make slime patrol by walking straight forward, if found wall, turn around
  - (11:00) implement cinemachine to the player  
    re-test slime patrol
  - (11:30) implement attack to slime  
    make him jump as a attack move
  - (13:00) Organize file mimicing namespace structure
- [x] attach hitbox to slime
  - (13:00) add collider to slime. turn on option isTrigger.  
    make SlimeHitbox class for doing damage using collider.
  - (13:25) make general class, "Hitbox" for doing damage to opposite team  
    opposite team is compared using team enum  
    implement new inferface IDamageable
  - (13:30) implement simple health to player and slime (no UI)  
    player have 30 health. slime have 20 health
  - (14:00) slime also push player when jump on the player  
    when slime hit player, slime should fly pass through the player.  
    okay, how about having 2 collider:
    1. hitbox - using isTrigger - put this in the child for easy time searching
    2. collider - separating collider layer for player and enemies using matrix.  
       now player and enemies don't collide, but the hitbox still does.  
       now player is hurt and slime fly pass through the player while doing so.
- [x] attach hitbox to bullet
  - (14:40) attach hitbox to bullet. adjust bullet collision layer.

**implement health to player and slime (1hr):**

- [x] use UI document for player's health (this overlay will be later used for inventory and more)
  - (15:00) I am trying to move my old UI code to this project.  
    Rename Health.cs to Stat.cs because it could be misleading that it was a UI.
- [x] use legacy UI for world UI healthbar which used by slime
  - (15:30) Implement world healthbar for slime
  - (16:00) Change setting to "Reload Scene" only.

**implement dead logic (1hr):**

- [x] when slime die, create another 3 slime but scaling its size down.
  - (16:00) add dead state to slime. create 3 slime on dead.  
    scaling the size down.  
    wire dead event from stat.cs to statemachine
- [x] when player die, teleport player back to home, reset HP
  - (16:40) add dead state to player. when player die he no longer can move or interact.  
    create gameobject for home.

## Design time hop mechanic

**implement time hop (2hr):**

- [x] make simple overlay using UI document to show current time. (Morning/Afternoon/Evening)
  - (18:30) add text panel to UI document  
    implement time interval (Morning/Afternoon/Evening). 5 minute is 1 interval.  
    When the time is met, the light turn brighter/dimmer ? - no idea how to make daytime/nighttime in 2D game.  
    when the new day come, let text UI show current day in the center of the screen. And slowly fade away.
- [x] simple collider for testing time hop mechanic. when player walk into it, skip to next interval.
  - (19:00) use the same trick to slime

# Day 2

Inventory is the biggest mechanic for me that I wasn't confidence at

## Design inventory

**player can collect/use/remove/organize items (6.30hr)**

- [x] make simple grid UI overlay for inventory window (2.30hr)
  - (7:50) Each cell can be put the item sprite inside - make a simple interface inventoryable - need sprite to put in the cell  
    let open it with "O"
  - (8:00) When player collect item, the item was added into the very first available grid.  
    player pick item up by walking on top of them.
  - (8:30) Player can drag items sprite around in the inventory, can replace/swap each items by dragging on top of them  
    trying to get my old dragging code port here.  
    let player drop the item too by dragging it out of inventory bound.  
    player can organize the inventory by dragging item to each cell.
  - (10:30) the drop item should be drop to the floor, not delete.  
    the ghost doesn't work correctly. the ghost doesn't follow player's mouse.

- [x] Inventory have 2 part: 1. the big inventory window open by "O" and 2. a overlay bar at the bottom for quick access  
       The idea is (4hr)
  - When the inventory is open:  
    1\) player can drag item from inventory & access bar to re-arrange item, drop item.  
    2\) but player can't use any active item: tool/seed/etc...
  - if inventory is closed: player can use drag item from the access bar.  
    1\) (deferred) drag tool outside of bound to equip it  
    2\) (deferred) drag seed on to the ground to plant it  
    3\) but player can't drag item to re-arrage or drop.
  - (10:40) Make additional quick access bar at bottom right of the screen. Let it have 5 cell.  
    When holding the item in inventory, make quick access bar green to indicate it can be interact with by putting item inside it.
  - (11:00) make dragging between inventory and hotbar possible
    I need to check new dragging.cs and check SlotGroup.cs too.
    SlotGroup.cs look weird. Why does the inventory get init in the UI? it was init in player which is alright.
  - (13:00) naming convetion is too confusing. We now have both inventory.cs that was inventory and hotbar. we also have UI that was name inventory.
    The dragging.cs become too messy. It's no longer readable. Let discarded this.
    Inventory and Hotbar are both inventory. Need better name to distinct them.
  - (13:05) Okay, the functionality of inventory window & hotbar are similar but hotbar can do a bit more:
    Inventory window can drag for re-arrange.
    Hotbar can drag re-arrange & drag for item activation too.
    Functionality are from the UI part. So let make InventoryPanel a mother.
    Make 2 child: InventoryWindow & InventoryHotbar.
    Rename the player's inventory to Backpack to prevent confusion.
  - (13:45) Implement Hotbar panel to re-arrage with Backpack panel
  - (14:20) Get Dragging.cs init out of backpack since it's no longer belong there since hotbar also use Dragging.cs
  - (14:30) Add quality of lift: Block attacks while the backpack is open, and when clicking the hotbar

**items can be separated into resources, tools, crafted objects, and seeds (3hr)**

- [x] inventory can hold resources, tools, crafted objects, and seeds.
  - (14:40) make at least 3 items in each category  
    resource = lumber, carrot, red berry, corn  
    tools = wand, watering can, axes  
    seed = carrot seed, red berry seed, corn seed  
    crafted item = Storage Chest, Golden Veggie, Broccoli
    (2hr break)
  - (17:00) let ItemSO be a parent of all 4 categorize. Begin by making existing EquipmentSO a child to ItemSO.
  - (17:15) add each ItemSO as planned.
    implement SeedSO.
    re-organized file for ItemSO using their categorized.
- [x] drag tools/seed/resource/crafted item from a bar outside the bound to use them
  - (17:30) active for each tools  
    draggin tool out of bound, equip the tool to the player. when equip, player activate tool using left click.
    wand - shoot cube  
    watering can - water seed - grow vegetable  
    axes - cut tree - for lumber
  - (18:30) behaviour for seed  
    place the seed on the ground near releasing pointer.
    all seed act the same. put on the ground. can grow by watering. if grow max, can be keep by player.  
    that mean a seed is the class of it own. It have several variation of sprite when it was growth.
  - resource does nothing but can be crafted
  - (deferred) crafted item was other mechanic
  - (deferred) make recipe  
    x10 lumber = storage chest  
    carrot + red berry + corn = golden veggie  
    x3 golden veggie = broccoli


# Day 3

## Design inventory

**cleanup (2hr)**

- [x] cleanup from yesterday
  - (7:30) continue make seed be able to planted/grow/harvest.
  - (7:50) implment axes behaviour to cut the tree
    implement tree prefab
    when the tree is cut, the tree should drop x3 Lumber

**item can stack inside inventory (3hr)**

- [x] items can stack and have limit at 10 max
  - (8:30) each cell in inventory keep a list<Stack>. Stack is class that keep a items variable and a number of the stack.
    change to let the Item itself have a amount variable.
    for simplicity, let Lumber the only 1 can stack more than 1.
    when pickup item, it should be added to the very first cell. if the cell happen to be the same item, add them together.
  - (9:50) implement storage chest
  - (12:00) cleanup and refactor some code

## Design Craft mechanic

**crafting mechanic overall (2hr)**

- [x] crafting is available from the "inventory" and "station crafting"
  - (12:20) implement RecipeSO for listing all crafting recipe
    implement Craft to check if the ingredient match the Recipe
    implement Ingredient for a ingredient in crafting mechanic.
    implement craft panel: 1) backpack 2) stationary

- [ ] cleanup:
  - (14:00) make other seed be able to stack too
  - when seed was grow and pickup, let make it drop the additional x3 seed too.
  - the character can jump off the tree which is not intented.
  - Item can be drop on the world - when the item was drop let it float up and down a little
  - player is too big, let separate it into smaller class.
  - implement .asmdef to force clean architecture

# NOTE
What is the difference between letting a item be a prefab and SO.
  a prefab can have position and exist in the world. SO can't.
  In this case, ItemSO don't need the position in the world so it don't have to be prefab.
  If it was a prefab, most of the item would just be a same gameobject with different sprite.
  Only some of the item could be a prefab because it have different behaviour e.g. wand shooting cube.
