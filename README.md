# **FPS Zombie Shooter**

**FPS Zombie Shooter** is a first-person survival shooter built in Unity with robust multiplayer capabilities powered by Photon PUN 2. Players must survive endless waves of zombies in a cooperative multiplayer environment, utilizing diverse weapons, strategic positioning, and resource management to achieve maximum survival time.

## **🎮 Game Overview**

Step into a post-apocalyptic world where waves of zombies continuously spawn and grow stronger. Team up with up to 6 players online or brave the challenge solo. Each round brings more enemies, testing your shooting skills, resource management, and survival instincts. Purchase health and ammunition from strategic shop stations between waves to stay in the fight.

## **🚀 Key Features**

* **Seamless Online Co-op**: Up to 6 players can join forces using Photon PUN 2 with automatic matchmaking and room creation
* **Dynamic Wave System**: Escalating difficulty with each wave spawning more enemies, controlled by master client for consistency
* **Arsenal & Combat**: Multiple weapon types with realistic mechanics including reload systems, ammo management, and weapon sway
* **Cross-Platform Support**: Supports both desktop (mouse/keyboard) and mobile (touch) input systems
* **Survival Mechanics**: Health/damage system with visual feedback and shop system for purchasing upgrades

## **🌐 Multiplayer Architecture**

### **Photon PUN 2 Integration**
The game implements a sophisticated multiplayer foundation using Photon PUN 2 with automatic connection management, dynamic room creation, and synchronized scene loading. The networking system handles up to 6 concurrent players with smart matchmaking that creates new rooms when existing ones are full.

### **Network Synchronization Strategy**
**Authority Distribution**: Master client controls global game state like wave progression and enemy spawning, while individual clients handle personal player data to prevent conflicts.

**Real-Time Communication**: Critical events like player damage, enemy deaths, and weapon effects are synchronized using Remote Procedure Calls (RPCs), while persistent states like weapon selection use Photon's Custom Properties system.

**Ownership-Based Updates**: Each player only processes input and updates for their own character, with visual representations synchronized across all clients for consistency.

## **🏗️ Technical Implementation**

### **Component-Based Architecture**
The codebase follows Unity best practices with modular, single-responsibility components. Player management, weapon handling, enemy AI, and game state are cleanly separated, allowing for easy expansion and maintenance.

### **Advanced Unity Integration**
**Multi-Platform Input**: Separate input handlers for desktop and mobile ensure responsive controls across different platforms. The touch system intelligently splits screen areas for movement and camera control.

**AI & Navigation**: Zombies utilize Unity's NavMesh system for pathfinding and automatically target the closest player in multiplayer scenarios, with dynamic animations based on movement state.

**Audio-Visual Systems**: 3D spatial audio, particle systems for weapon effects, and synchronized UI elements create an immersive experience across all connected players.

### **Performance & Quality**
**Network Optimization**: Ownership validation prevents unnecessary processing on non-owned objects, while efficient update loops minimize network traffic to only essential data.

**Defensive Programming**: Consistent null checking, early returns for network scenarios, and proper cleanup of instantiated objects ensure stable multiplayer performance.

**Cross-Mode Compatibility**: The game gracefully handles both online multiplayer and offline single-player modes without code duplication or architectural compromises.

## **🎯 Technical Highlights**

This project demonstrates advanced understanding of networked game development with proper authority distribution, efficient synchronization mechanisms, and robust handling of connection states. The implementation showcases expertise in Unity's core systems including NavMesh AI, Coroutines, Animation systems, and Photon PUN 2 integration.

The modular architecture and cross-platform compatibility reflect strong software engineering principles, making the codebase maintainable and easily extensible for future features and content updates.
