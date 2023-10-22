Students information:

	Kaan Pagie: 1776371
	Karel Boerstoel: 0233137

Game information:

	general keybinds:

		toggle fullscreen: F5
		play/pause song: T
		previous/replay song: Y
		skip song: U

	Keybinds in one player mode:

		rotate left: A
		rotate right: D
		move left: left arrow 
		move right: right arrow
		move down / 'soft drop': down arrow
		move down / 'hard drop': spacebar
		'hold': left shift

	Keybinds in two player mode:

		player 1:
			rotate left: A
			rotate right: D
			move left: C
			move right: B
			move down / 'soft drop': V
			move down / 'hard drop': S
			'hold': left shift
		player 2: 
			rotate left: I
			rotate right: P
			move left: left arrow 
			move right: right arrow
			move down / 'soft drop': down arrow
			move down / 'hard drop': right control
			'hold': O

Added Extras:
	
	Multiplayer:
		The opponents grid get pushed up one line if the other player triggers a level up or scores a tetris.

	Bonuses for special shapes:
		The player can try to make a special shape, if they succeed all lines the shape is part of get cleared.

	Special Blocks: 
		There are three special blocks, they have a random chance to spawn:
			Explosive block: explodes all blocks in a 2 cell radius.
			
			Push Down block: pushes all cells underneath the block all the way down.

			Pull Up block: pulls all cells underneath the block up to the level the block is placed at.
	Effects:
		effects get played when a line is cleared, a block is placed, and there are special block specific effects.
	
	Random Bag:
		instead of spawning the blocks completely randomly all 7 blocks are placed in a random order in a kind of queue.
		when the queue is empty the queue gets refilled. This system allows more strategy/predictability.

	Hold Key: 
		The player can hold/store one block and swap with it by pressing the hold key, this can't be done twice in a row.

	Custom Media Player
		The built in media player seemed slightly buggy, so we added a custom mediaplayer with fuctions such as:
			add song to queue
			Play/Pause
			Skip song
			Previous song/replay song, replays when the song is further then 2 seconds, skips song when less far than 2 seconds.

	Full screen functionality
		The player can toggle full screen mode with F5.

	Menus with buttons:
		A main menu where some of the extras can be toggled on or off and the player can choose between multi- or singleplayer.
		A game over menu where the player can return to the main menu or play again.
	