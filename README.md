# Multiplayer FPS | Unity | C# | Photon PUN 2

A wave-based zombie shooter with networked multiplayer supporting up to 6 players. Built with Photon PUN 2 for real-time synchronization of player actions, enemy spawning, and game state across clients. Implements client-server architecture with authoritative master client handling enemy AI and round progression.

**Tech Stack:** Unity 3D | C# | Photon PUN 2 | RPCs | Custom Properties | NavMesh

---

## Development Approach

I structured the multiplayer system around Photon's Master Client pattern - one client acts as the authoritative server for enemy spawning and AI while all clients handle their own player input and weapon firing. The GameManager only spawns enemies on the Master Client, then uses `PhotonNetwork.Instantiate()` to replicate them across all clients. Player actions use RPCs (Remote Procedure Calls) to synchronize events like taking damage or firing weapons.

The challenge was handling both single-player and multiplayer modes with the same codebase. Every manager class checks `PhotonNetwork.InRoom` to determine which code path to execute - single-player uses direct method calls while multiplayer uses RPC broadcasts. This eliminated duplicate logic while maintaining deterministic behavior across network conditions.

![Image](https://github.com/user-attachments/assets/83cc0213-95bd-49ca-9f86-28b047584ca0)

---

## Key Technical Systems

### Photon Master Client Authority

Only the Master Client spawns enemies and manages wave progression to prevent duplicate spawns. When GameManager detects `enemiesAlive == 0`, it increments the round counter and calls `NextWave()`, which loops round times to spawn enemies at random spawn points. The Master Client uses `PhotonNetwork.Instantiate()` instead of Unity's `Instantiate()` - this automatically replicates the GameObject across all clients with synchronized transforms and PhotonView components.

The tricky part was synchronizing round numbers. When the Master Client updates the round, it stores the value in Custom Properties using `PhotonNetwork.LocalPlayer.SetCustomProperties()`. All other clients receive `OnPlayerPropertiesUpdate()` callbacks and update their UI accordingly. This ensured the round counter stayed synchronized even if players joined mid-game.

```cpp
// GameManager.cs - Master Client spawns, all clients display
if (!PhotonNetwork.InRoom || PhotonNetwork.IsMasterClient && photonView.IsMine)
{
    if (enemiesAlive == 0)
    {
        round++;
        NextWave(round);
        
        Hashtable hash = new Hashtable();
        hash.Add("CurrentRound", round);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
```

### RPC System for Damage Synchronization

When a player shoots an enemy, the hit detection happens locally on the shooter's client (to minimize input lag), but damage must apply across all clients. I used RPCs with ViewID matching to solve this. The shooter calls `enemyManager.Hit(damage)`, which triggers an RPC broadcast to `RpcTarget.All`. Each client receives `TakeDamage()` and checks if `photonView.ViewID` matches - only the correct enemy instance processes the damage.

The same pattern applies to player damage. When an enemy collides with a player, it calls `PlayerManager.Hit()` which broadcasts `PlayerTakeDamage` RPC. This kept health values synchronized without constant network updates - damage only syncs on impact events.

### Weapon System with Visual Sync

Weapon firing has two phases: gameplay (raycasting, damage) and visuals (muzzle flash, audio). The shooter's client handles raycasting locally for instant feedback, but the muzzle flash and sound must play on all clients. When a player fires, `WeaponManager.Shoot()` calculates the raycast hit, applies damage, then calls `photonView.RPC("WeaponShootVFX")` to broadcast visual effects.

I separated the RPC call from the damage logic to avoid sending redundant data. The RPC only transmits the `photonView.ViewID` - each client's WeaponManager receives this and checks if it matches its own ViewID before playing the ParticleSystem and AudioClip. This reduced network traffic while maintaining visual synchronization.

### Custom Properties for Weapon Switching

Players can switch weapons mid-game, and this must replicate to other clients so they see the correct weapon model. I used Custom Properties instead of RPCs because weapon switching doesn't need guaranteed delivery - if a packet drops, the next switch will overwrite it anyway. When `PlayerManager.weaponSwitch()` runs, it stores the weaponIndex in the player's Custom Properties.

Remote clients receive `OnPlayerPropertiesUpdate()` and check if the changedProps contains "weaponIndex". If the property belongs to another player (`!photonView.IsMine`), they call `weaponSwitch()` to toggle the correct child GameObject. This approach automatically handles late-joining players since Custom Properties persist in the room.

### Enemy AI with Master Client Pathfinding

Enemies use NavMeshAgent for pathfinding, but only the Master Client calculates paths. Each enemy's `EnemyManager.Update()` checks `PhotonNetwork.IsMasterClient` before updating `navMeshAgent.destination`. Non-master clients skip AI logic entirely - they just receive transform updates from Photon's automatic synchronization.

The challenge was targeting the closest player in multiplayer. `GetClosestPlayer()` loops through all GameObjects tagged "Player" and calculates distances. I stored `playersInScene` in `Start()` using `FindGameObjectsWithTag()`, but players spawn dynamically after scene load. To handle this, I call `FindGameObjectsWithTag()` every time `GetClosestPlayer()` runs, accepting the minor performance cost to ensure accurate targeting when players join/leave.

### Room Management and Scene Persistence

RoomManager uses `DontDestroyOnLoad()` to persist across scene transitions. It subscribes to `SceneManager.sceneLoaded` and spawns the player prefab when the game scene loads. The spawn position randomizes between -3 and 3 on X/Z axes to prevent players from spawning inside each other.

The singleton pattern here had a bug - I initially used `if (Instance) Destroy(Instance)`, which destroys the new instance instead of the old one. This caused multiple RoomManagers to exist across scenes. I fixed it by destroying `gameObject` instead of `Instance`, but the proper solution would be `Destroy(gameObject); return;` to prevent further execution.

---

## Technical Challenges

**Single-Player Compatibility:** Every script needed to handle both networked and local modes. I added `PhotonNetwork.InRoom` checks before all RPC calls and network instantiations. For single-player, I used `Resources.Load()` to spawn prefabs directly instead of `PhotonNetwork.Instantiate()`.

**Master Client Migration:** If the Master Client disconnects, Photon automatically migrates authority to another client. However, this broke enemy spawning since the new Master Client didn't have the current round value. I solved this by storing round in Custom Properties so the new Master Client could read it and continue wave progression.

**Weapon VFX Timing:** Initially, muzzle flashes played before damage applied, causing visual desync. I restructured `Shoot()` to calculate damage first, then call the VFX RPC. This ensured enemies reacted to damage before the shooter saw the muzzle flash on their end.

---

## What I Learned

Using Master Client authority eliminated the need for a dedicated server while maintaining deterministic gameplay - only one client makes decisions, preventing desyncs. RPCs with ViewID matching taught me to minimize network data by only sending identifiers instead of full state. Custom Properties work better than RPCs for non-critical data like UI updates since they don't guarantee packet delivery. The `photonView.IsMine` check pattern became essential - every input-handling script needs it to prevent remote clients from processing local input.

## **Play Link**
- https://sayannandi.itch.io/unity-multiplayer-photonpun2

[![Gameplay Video](https://img.youtube.com/vi/j0BsWwCx9QE/maxresdefault.jpg)](https://youtu.be/j0BsWwCx9QE)
### [Watch this video on YouTube](https://youtu.be/j0BsWwCx9QE)

<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/7870529b-9033-4ea6-a0f4-9f6b47bd829e" />

<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/7f90d6da-ff3c-4d0b-a4d5-a8557808bc1e" />

<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/0cccb880-280f-41aa-ac2e-3ef97d0447f9" />

<img width="1920" height="1080" alt="Image" src="https://github.com/user-attachments/assets/d2b6ae9b-b379-43f2-b72d-ece1a4d291dd" />
