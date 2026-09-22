Deadline 25/9/69

# Day 0 (22/9/69) Planning
- init git repo
- understanding the requirements
- find free asset

	Feature
	0. player can move, jump
		[] use CharacterController2D for movement/jump
		[] implement FSM for clean code.

	1. time hop triggers
		1. player walk into collider to trigger the time skip 
			[] simple collider trigger logic
		2. a clock show current time
			[] simple overlay using only number
		3. time is divided into 3 periods: Morning, Afternoon, and Evening. 
		4. the day was also separated into weekday Mon/Tue/Wed/etc...
		5. day is tracked infinitely: day0 => day1 => ... => day100+.
			[] Use "time" float var to track in-game time.
			[] Let say 5 minutes = each interval (Morning/Afternoon/Evening)
			[] When the time is met, the light turn brighter/dimmer ? - no idea how to make daytime/nighttime in 2D game.
			[] When the day cross between Evening => Morning, +1 to "today" int var
	
	2. Inventory system 
        1. player can collect/use/remove/organize items
            [] make simple grid UI overlay for inventory window
			[] Each cell can be put the item sprite inside
            [] When player collect item, the item was added into the very first available grid.
			[] Player can drag items sprite around in the inventory, can replace/swap each items by dragging on top of them

        2. inventory can hold resources, tools, crafted objects, and seeds.
            [] Have several items in the game. Those items also was categorized.
            [] make at least 3 items in each category
                resource = lumber, carrot, red berry, corn
                tools = watering can, shovel, axes
                seed = carrot seed, red berry seed, corn seed
                crafted item = Storage Chest, Golden Veggie, Broccoli
        
        3. inventory bar - a overlay bar at the bottom for quick access e.g. equipable, usable, or placeable
            [] use simple grid UI overlay like inventory window 
            [] drag seeds from a bar on the ground to place it
            [] drag shovel from a bar on the seed, make the seed grow to flower
            [] that mean a seed is the class of it own. It have several variation of sprite when it was growth.

        4. items can stack and have limit at 10 max
            [] Each cell in inventory keep a list<Stack>. Stack is class that keep a items variable and a number of the stack. 

    3. crafting system
        1. crafting is available from the "inventory" and "station crafting"
            [] Make a Recipe class that hold every combination of each items. Let assume the crafting use only 3 item in total. The order while crafting doesn't matter.  
            [] Let station craftign have 3 craft cell
            [] Let Inventory have 2 craft cell 
        2. requirement
            [] Craft a Storage Chest directly from items in the Inventory.
            [] A Storage Chest requiring 10x Lumber, which can hold 30 items when crafted and placed
            [] Let the Storage Chest use the same logic to inventory window but bigger. Inventory have 10 cell, Storage Chest have 30 cell.

    4. combat system
        1. Player is equip with Wand. It can shoot projectile that does 5 damage.
            [] when player attack, create a cube that drive itself forward non-stop. Let it destroy itself after 5 sec.
            [] the cube has collider for doing damage

        2. Slime that can patrol, and attack player on found. When die, split into smaller slimes with 5 health. 
            [] make a clone statemachine for slime. add that he can patrol around, and attack player on found.
                [] let him walk by jumping, his hitbox should be the same size as his sprite. We have animation of him jumping, could we make the animation drive hitbox? 
                [] player have 30 health. if slimes hit player, player lose 3 health.
                [] how are we going to make slime patrol? 
            [] making slime split when he die. when slime die, create another 3 slime but scaling its size down.
            [] when player die, teleport player back to home, reset HP

# Day 1  
I am not confidence in making inventory. So I'll start with what I am confidence with which is:
    1. player controller
    2. combat system.
    3. lastly, time hop system because it seem easy.

## Design player controller 
    implement CharacterController2D for movement/jump.
	implement FSM for clean code. 
    (1hr)

## Design combat system

# Day 2
Inventory
Inventory is the biggest mechanic for me that I wasn't confidence at

# Day 3 Wrap thing up
