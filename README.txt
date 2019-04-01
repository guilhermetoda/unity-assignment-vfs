###############################
# VFS - Vancouver Film School #
###############################
Unity 2 (C#) - Assignment I
Guilherme Toda

1) Clamp the Move Speed
-  Script: PlayerController.cs (from line #135)

2) Clamp Camera Rotation
- Script: PlayerController.cs (from line #87)

- New Features
When the AI has a Health Lower than 1/3 of his MaxHealth (The initial health). The AI will shoot a Bomb that will explode and generates 180 little bullets,
after that, the AI will run (increasing the speed 5 times) to the "HealPost" to heal.
When the AI hits 100% of his health, the AI will return to the Idle state and or find an Outpost to capture or an enemy to attack.

New AI States:
Moving To Heal (AIController.cs:109)
Healing (AIController.cs:125)

New Classes:
CrazyBomb.cs (Class that Instantiate the little bullets)
Bullet.cs (Little bullet class, that class, checks if a bullet hits an AIUnit and take damage from)
Healpost.cs (Class responsible for heal the Unit every SECOND, NOT every FRAME)

Important New Functions:
LookForHealPost (AIController.cs:201)
CheckingIfHealthIsLow (AIController.cs:168)
Healing to the AIUnit (AIUnit.cs:111)
ShootCrazyBomb (AIUnit.cs:85) 

