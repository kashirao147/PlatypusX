# PlatypusX Unity Game - Codebase Analysis

## Overview
PlatypusX is a Unity-based submarine/underwater game with comprehensive monetization, social features, and progression systems. The game appears to be a mobile-first title with extensive integration for ads, in-app purchases, leaderboards, and daily rewards.

## Core Game Systems

### 1. Game Management (`GameManager.cs`)
- **Main Controller**: Central game state management (Menu, Playing, Pause, GameOver)
- **Level System**: Dynamic level spawning with increasing difficulty
- **Score Tracking**: Distance-based scoring system
- **Ad Integration**: Banner ad display with removal option
- **Player Management**: Handles player spawning and character selection

### 2. Player System (`Player.cs`)
- **Health System**: 100 HP with damage reduction mechanics
- **Movement**: Physics-based movement with rotation limits
- **Power-ups**:
  - Shield system with energy management
  - Magnet for coin collection
  - Rocket launcher with fire rate control
  - Gun system with bullet management
- **Audio**: Engine sound with volume control
- **Visual Effects**: Blink effect for damage indication

### 3. Global Values (`GlobalValue.cs`)
- **Persistent Data**: PlayerPrefs-based save system
- **Currency**: Coin system with default 100 coins
- **Progression**: Best score, character unlocks, mission tracking
- **Settings**: Sound/music toggles, ad removal status
- **Inventory**: Rocket and bullet counts

## Monetization Systems

### 1. In-App Purchases (`Purchaser.cs`)
- **Unity IAP Integration**: Full Unity Purchasing system
- **Product Types**:
  - Coin packs (3 tiers)
  - Remove ads (non-consumable)
  - Subscription support
- **Platform Support**: iOS, Android, WebGL
- **Event System**: Purchase result callbacks

### 2. Ad System
- **Gley Mobile Ads**: Primary ad management
- **Google Mobile Ads**: Direct integration
- **Ad Types**: Banner ads with bottom positioning
- **Ad Removal**: Persistent removal through IAP

### 3. Shop System
- **ShopItemUI.cs**: IAP item management
- **ShopItems_Submarine.cs**: Character unlock system
- **ShopItems_Bullet.cs**: Ammunition purchases
- **ShopItems_Rocket.cs**: Rocket purchases
- **ShopUI.cs**: Panel switching for different shop sections

## Social & Engagement Features

### 1. Daily Rewards System (`DailyRewards.cs`)
- **Time-based Rewards**: 24-hour cycle tracking
- **Persistent Storage**: PlayerPrefs for reward state
- **Debug Support**: Time manipulation for testing
- **Integration**: Coin rewards with sound effects

### 2. Leaderboard System (`PlayFabManager.cs`)
- **PlayFab Integration**: Cloud-based leaderboards
- **Device Identification**: Cross-platform unique IDs
- **User Profiles**: Display name and email management
- **Score Submission**: Automatic score upload

### 3. Mission System (`MissionManager.cs`)
- **Task Types**:
  - Shark killing
  - Bomb destruction
  - Distance traveled
  - Power-up usage
  - Shield usage
- **Progression**: Mission completion tracking
- **Rewards**: Coin rewards for completed missions
- **UI Integration**: Mission display and status updates

## Audio System (`SoundManager.cs`)
- **Music Management**: Background music with volume control
- **Sound Effects**: Click, collect, power-up, explosion sounds
- **Volume Control**: Separate music and SFX volume
- **Global Access**: Static methods for easy sound playing

## UI & Control Systems

### 1. Hand Control Switcher (`HandControlSwitcher.cs`)
- **Accessibility**: Left/right hand control switching
- **Persistent Settings**: PlayerPrefs for hand preference
- **UI Updates**: Button text changes based on current mode

### 2. Menu Systems
- **MainMenu.cs**: Main menu controller
- **MainMenu_UI.cs**: UI event handling
- **MainMenu_Controller.cs**: Menu navigation logic
- **HomeMenu.cs**: Home screen management

## Technical Architecture

### 1. Namespace Organization
- **PhoenixaStudio**: Main game namespace
- **NiobiumStudios**: Daily rewards system
- **PlayFab**: Leaderboard integration

### 2. Singleton Patterns
- **GameManager**: Main game controller
- **SoundManager**: Audio management
- **MissionManager**: Mission system
- **Purchaser**: IAP management

### 3. Event Systems
- **Delegates**: Purchase callbacks, daily reward events
- **Unity Events**: UI button interactions
- **PlayFab Events**: Leaderboard updates

## Asset Organization

### 1. Game Assets (`Assets/Game/`)
- **Script/**: Core game logic (40+ scripts)
- **Prefab/**: Game objects and UI elements
- **Sprite/**: Visual assets organized by category
- **Animation/**: Character and effect animations
- **Audio/**: Sound effects and music

### 2. Third-party Integrations
- **DailyRewards/**: Niobium Studios daily rewards
- **Gley/MobileAds/**: Mobile advertising system
- **GoogleMobileAds/**: Google AdMob integration
- **PlayFabSDK/**: PlayFab cloud services
- **ExternalDependencyManager/**: Dependency management

## Key Features Summary

### Core Gameplay
- Submarine movement and combat
- Power-up collection and usage
- Enemy interaction (sharks, bombs)
- Coin collection and economy
- Progressive difficulty system

### Monetization
- Multiple IAP tiers for coins
- Ad removal option
- Banner ad integration
- Subscription support

### Social Features
- Cloud-based leaderboards
- Daily reward system
- Mission progression
- Character customization

### Technical Features
- Cross-platform support (iOS, Android, WebGL)
- Persistent data management
- Audio system with volume controls
- Accessibility features (hand switching)
- Debug tools (screenshot system)

## Code Quality Observations

### Strengths
- Well-organized namespace structure
- Comprehensive save system using PlayerPrefs
- Modular design with clear separation of concerns
- Extensive third-party integration
- Mobile-optimized architecture

### Areas for Improvement
- Some hardcoded values could be configurable
- Error handling could be more robust
- Code documentation could be enhanced
- Some scripts have high coupling

## Development Recommendations

1. **Documentation**: Add XML documentation to public methods
2. **Error Handling**: Implement try-catch blocks for network operations
3. **Configuration**: Move hardcoded values to ScriptableObjects
4. **Testing**: Add unit tests for core systems
5. **Performance**: Optimize Update loops and object pooling
6. **Security**: Implement server-side validation for purchases

This codebase represents a mature mobile game with comprehensive monetization and social features, suitable for commercial release across multiple platforms. 