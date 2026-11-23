# Game Demo – RPG Fantasy

A 3D third-person RPG fantasy game demo developed using **Unity and C#**. The project focuses on building a modular player controller, weapon and combat systems, inventory management, health mechanics, animations, and NPC functionality.

The main objective is to develop a reusable RPG gameplay foundation using Object-Oriented Programming (OOP), ScriptableObjects, events, delegates, and component-based architecture.

---

## 1. Project Summary

**Game-Demo-RPG-Fantasy** is a third-person RPG prototype featuring player movement, weapon interactions, combat mechanics, projectile systems, health management, and character animations.

The project currently focuses on:

- Third-person player controller.
- Character movement and animation system.
- Weapon equipment and switching.
- Melee and ranged combat.
- Inventory and item management.
- Health and damage systems.
- NPC character setup.
- UI elements for gameplay information.

### Development Technologies

| Technology | Purpose |
|---|---|
| Unity 6 | Game Engine |
| C# | Gameplay Programming |
| Blender / 3D Assets | Character and Game Models |
| Unity Animator | Character Animations |
| ScriptableObjects | Weapon and Item Data |
| Rigidbody / Physics | Movement and Collision |
| Cinemachine | Third-Person Camera |

---

## 2. Player System

The player system handles character movement, camera interaction, weapon selection, combat input, and character animations.

### 2.1 Player Introduction

The player uses a third-person character controller with physics-based movement and camera-relative gameplay.

Main features:

- Third-person character movement.
- Rigidbody-based movement.
- Jumping and ground detection.
- Sprinting and normal movement.
- Camera rotation and aiming.
- Weapon selection and interaction.
- Combat and defensive actions.

### 2.2 Player Input System

Player input is managed through a centralized `PlayerMovement` script.

| Input | Action |
|---|---|
| W / A / S / D | Character Movement |
| Left Shift | Sprint |
| Space | Jump |
| Mouse Movement | Camera Rotation |
| Left Mouse Button | Attack |
| Right Mouse Button | Block / Bow Load |
| E | Pick Up Item |
| G | Drop Equipped Weapon |
| 1 | Select Sword |
| 2 | Select Shield |
| 3 | Select Bow |
| 4 | Select Throwable |
| Escape | Cursor / Pause Controls |

Input actions communicate with other gameplay components using C# events.

### 2.3 Player Animation System

The `AnimScript` manages character animations using Unity Animator.

Implemented animation categories:

- Idle
- Walk
- Run
- Jump Begin
- Fall
- Jump Land
- Combat Idle
- Weapon Equip / Unequip
- Attack
- Block
- Bow Load / Release
- Throwable Animations

A Blend Tree is used for directional movement and locomotion.

Special animations use external animation control to prevent normal movement animations from interrupting combat actions.

### 2.4 Player Interaction

The player can interact with the game world through:

- Item pickup.
- Weapon selection.
- Weapon switching.
- Item dropping.
- Combat interactions.
- Projectile attacks.
- Health and damage interactions.

---

## 3. Weapon & Combat System

The weapon system uses ScriptableObjects to store weapon-related data and supports different weapon categories.

### 3.1 Weapon Categories

**Sword**
- Melee combat.
- Two alternating attack animations.
- Hitbox-based damage detection.
- Damage activation during attack animations.

**Shield**
- Defensive combat.
- Right-click blocking.
- Shield hitbox activation while blocking.
- Blocks incoming sword attacks and projectiles when correctly positioned.

**Bow & Arrow**
- Ranged combat.
- Arrow ammunition management.
- Bow loading and release animations.
- Camera-centre aiming.
- Projectile spawning and collision detection.
- Damage application on successful hits.

**Throwable Weapons**
- Snowball and rock projectile support.
- Throw animations.
- Animation-event-based projectile release.
- Inventory quantity management.

### 3.2 Weapon Data Management

Weapon information is managed through `WeaponData` and `ItemData` ScriptableObjects.

Data includes:

- Weapon name and category.
- Damage amount.
- Weapon prefabs.
- Projectile prefabs.
- Projectile speed.
- Attack animations.
- Equip and unequip animations.
- Ammunition requirements.
- Combat idle animations.

### 3.3 Combat Architecture

The `CombatController` manages weapon equipment, attacks, blocking, projectile firing, and combat animation transitions.

The `Damage`, `ShieldBlock`, `Projectile`, and `ArrowScript` components handle individual combat responsibilities.

---

## 4. NPC System

The NPC system is currently under development.

NPC characters use a similar character structure to the Player, allowing shared gameplay components to be reused.

### 4.1 NPC Features

Currently implemented or configured:

- NPC character model and Animator.
- Health component.
- Damage reception.
- Weapon and combat component setup.
- Death animation states.
- Respawn point assignment.

### 4.2 NPC Health & Respawn

NPCs use the shared `Health` component.

Planned behaviour:

- Receive damage from player attacks.
- Detect when HP reaches zero.
- Play the appropriate death animation.
- Respawn at an assigned respawn point.
- Restore maximum health.

### 4.3 NPC Animations

Two death animation states are configured:

- `Death` – Character dies without an equipped weapon.
- `CombatDeath` – Character dies while equipped for combat.

### 4.4 Future NPC Development

- NPC AI movement.
- Player detection.
- Enemy attack behaviour.
- NPC navigation.
- AI combat states.
- NPC weapon management.

---

## 5. UI System

The UI system provides gameplay information and visual feedback.

### 5.1 Health Bar

The health system uses a UI Image with a Filled image type.

Features:

- Displays current player health.
- Updates when damage is received.
- Supports health restoration.
- Uses health-change events to update the UI.

### 5.2 Other UI Elements

Additional UI elements and features:

- Inventory UI.
- Weapon information.
- Item quantities.
- Ammunition display.
- Combat feedback.
- Crosshair and aiming UI.

Some of these elements are still planned or under development.

---

## 6. Project Architecture

The project uses separate gameplay components to keep responsibilities organized.

| Script | Responsibility |
|---|---|
| `PlayerMovement` | Player movement and input |
| `AnimScript` | Character animation control |
| `CombatController` | Weapon and combat management |
| `PlayerInventory` | Inventory management |
| `Health` | Health, damage and death detection |
| `Damage` | Melee hitbox damage |
| `ShieldBlock` | Shield defensive collision |
| `ArrowScript` | Arrow projectile behaviour |
| `Projectile` | Throwable projectile behaviour |
| `ItemPickup` | World item pickup |
| `OnEnd` | Animation completion callbacks |

### Programming Concepts Used

- Object-Oriented Programming (OOP).
- Encapsulation.
- Inheritance.
- Polymorphism.
- Abstraction.
- C# Events and Delegates.
- ScriptableObjects.
- Component-based architecture.
- State-based animation control.

---

## 7. Development Status

| Feature | Status |
|---|---|
| Third-Person Player Movement | Implemented |
| Camera Control | Implemented |
| Jump / Fall / Land | Implemented |
| Locomotion Blend Tree | Implemented |
| Weapon Selection | Implemented |
| Sword Combat | Implemented |
| Shield Blocking | Implemented |
| Bow & Arrow | Implemented |
| Throwable Weapons | Implemented |
| Inventory Foundation | Implemented |
| Health System | Implemented |
| Health Bar | Implemented |
| Damage Testing Zone | Implemented |
| NPC Character Setup | Implemented |
| NPC AI | Pending |
| Death & Respawn | In Progress |
| Inventory UI | Pending |
| Aiming / Crosshair UI | Pending |

---

## 8. Future Improvements

- Complete Player and NPC death/respawn integration.
- Develop independent NPC AI movement and combat.
- Implement enemy behaviour and navigation.
- Expand inventory UI and equipment management.
- Add aiming and targeting systems.
- Improve combat animations and transitions.
- Add more weapons, items, and enemy types.
- Improve gameplay feedback and UI.

---

## 9. Project Goal

The goal of this project is to create a modular third-person RPG gameplay prototype that demonstrates practical Unity development skills, C# programming, gameplay architecture, animation integration, combat mechanics, and reusable game systems.

**Project Name:** Game-Demo-RPG-Fantasy  
**Engine:** Unity 6  
**Language:** C#  
**Project Type:** 3D Third-Person RPG Demo
