# Speed Boost Powerup Setup Guide

## Overview
The speed boost powerup has been successfully added to the PlatypusX game. This guide explains how to set up the prefab and integrate it into your game.

## What Was Added

### 1. Scripts Created/Modified:
- **PowerUpSpeedBoost.cs** - New powerup script (created)
- **Player.cs** - Added speed boost functionality (modified)
- **GlobalValue.cs** - Added speed boost tracking variables (modified)
- **Mission.cs** - Added CollectSpeedBoostPowerUp task type (modified)
- **MissionManager.cs** - Added speed boost mission handling (modified)

### 2. Speed Boost Features:
- **Ramp Up Time**: 1 second to gradually reach double speed
- **Duration**: 10 seconds at double speed
- **Ramp Down Time**: 1 second to gradually return to normal speed
- **Multiplier**: 2x speed boost (configurable)
- **Sound**: Uses shield powerup sound effect
- **Mission Integration**: Tracks collection for missions

## How to Create the Speed Boost Prefab

### Step 1: Create the GameObject
1. In Unity, create a new GameObject
2. Name it "PowerUp SpeedBoost"
3. Add a SpriteRenderer component
4. Add a Collider2D component (set as trigger)
5. Add a Rigidbody2D component (set to kinematic)

### Step 2: Add the Script
1. Add the `PowerUpSpeedBoost.cs` script to the GameObject
2. The script will automatically handle:
   - Movement with game speed
   - Player collision detection
   - Powerup collection tracking

### Step 3: Set Up the Sprite
1. Use the same sprite as other powerups (shield, magnet, gun)
2. You can tint it with a different color to distinguish it
3. Set the sorting layer appropriately

### Step 4: Configure Collider
1. Set the Collider2D as a trigger
2. Adjust the size to match the sprite
3. Ensure it's properly positioned

### Step 5: Create Prefab
1. Drag the configured GameObject into the `Assets/Game/Prefab/PowerUp/` folder
2. Name it "PowerUp SpeedBoost.prefab"
3. Delete the scene GameObject

## Integration with Spawning System

### Option 1: Manual Placement
- Place the prefab in your level designs
- Position it strategically in the game world

### Option 2: Random Spawning
- Add the prefab to your random spawn system
- Configure spawn rates and conditions

### Option 3: Level Integration
- Add the prefab to your level prefab arrays
- Include it in the level generation system

## Configuration Options

### In Player.cs:
```csharp
[Header("Speed Boost")]
public float speedBoostMultiplier = 2f;        // Speed multiplier
public float speedBoostRampUpTime = 1f;       // Time to reach double speed
public float speedBoostDuration = 10f;         // Duration at double speed
public float speedBoostRampDownTime = 1f;     // Time to return to normal
```

### In GlobalValue.cs:
```csharp
public static int CollectSpeedBoostPowerUp
public static bool isStartCollectSpeedBoostPowerUp
```

## Mission Integration

### Creating Speed Boost Missions:
1. In the MissionManager, add a new mission
2. Set the task type to `CollectSpeedBoostPowerUp`
3. Configure target amount and reward coins
4. The system will automatically track progress

### Example Mission:
```csharp
Mission speedBoostMission = new Mission();
speedBoostMission.task = Task.CollectSpeedBoostPowerUp;
speedBoostMission.mission = "Speed Demon";
speedBoostMission.message = "Collect speed boost powerups";
speedBoostMission.targetAmount = 5;
speedBoostMission.rewardCoin = 50;
```

## Testing the Speed Boost

### In-Game Testing:
1. Spawn the speed boost prefab in your scene
2. Play the game and collect the powerup
3. Verify the speed gradually increases to double over 1 second
4. Verify the speed stays at double for 10 seconds
5. Verify the speed gradually returns to normal over 1 second
6. Check that the collection is tracked in GlobalValue

### Debug Information:
- The speed boost uses the shield powerup sound effect
- Collection is tracked in `GlobalValue.CollectSpeedBoostPowerUp`
- Mission progress is automatically updated

## Troubleshooting

### Common Issues:
1. **Powerup not moving**: Check GameManager.State is Playing
2. **No speed boost effect**: Verify Player.UseSpeedBoost() is called
3. **Sound not playing**: Check SoundManager.soundPowerUpShield is assigned
4. **Mission not tracking**: Verify GlobalValue variables are properly set

### Debug Tips:
- Add Debug.Log statements in PowerUpSpeedBoost.OnTriggerEnter2D
- Check Player.UseSpeedBoost() for proper speed modification
- Verify GameManager.Instance.Speed is being modified correctly

## Performance Considerations

- The speed boost uses a Coroutine for smooth transitions
- Speed changes are applied to GameManager.Instance.Speed
- The effect is temporary and doesn't persist between game sessions
- Multiple speed boosts won't stack (prevents overlapping effects)

## Future Enhancements

### Possible Improvements:
1. Add visual effects during speed boost
2. Create unique sound effect for speed boost
3. Add particle effects for speed boost collection
4. Implement speed boost UI indicator
5. Add different speed boost tiers (1.5x, 2x, 3x)

### Code Extensions:
```csharp
// Add visual effect support
public GameObject speedBoostEffect;

// Add UI indicator
public GameObject speedBoostUI;

// Add different boost types
public enum SpeedBoostType { Small, Medium, Large }
```

This speed boost powerup is now fully integrated into the PlatypusX game system and ready for use! 