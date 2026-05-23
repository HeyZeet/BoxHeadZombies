# Deadbox Arena

Deadbox Arena is a top-down arena survival shooter inspired by classic Flash-era zombie rooms. The prototype target is quick to start, readable under pressure, and built from reusable S&box GameObjects, Components, Scenes, and Prefabs.

## Core Loop

1. Player spawns in a compact room.
2. Zombies enter from doors, portals, vents, or arena edges.
3. Player kills zombies for score, cash, XP, and combo pressure.
4. Waves escalate with more enemies, faster enemies, and special types.
5. Weapons, traps, and deployables unlock over time.
6. The run ends on death or a target wave clear.
7. Score, kills, survival time, and unlocks are shown before retry.

## Feel

The game should feel like classic arcade survival rebuilt for S&box: simple inputs, strong feedback, smoother movement, mouse-aim potential, co-op-friendly architecture, and modern progression.

## Prototype Pillars

- Simple survival: kill zombies, avoid corners, keep moving, spend resources wisely.
- Escalating chaos: every wave gets denser, faster, or less forgiving.
- Weapon progression: pistol first, then shotgun, SMG, rifle, grenades, launcher, traps, and turrets.
- Blocky identity: boxy characters, chunky enemies, bright colors, readable splats, and clean effects.

## Starting Controls

- WASD: move
- Left click: shoot
- R: reload
- E: interact or buy
- 1-5: weapon slots
- Shift: future dash or sprint

## First Milestone

The first playable milestone should prove the room survival loop:

- Angled top-down camera follows the player.
- Player moves on the arena plane and fires a pistol.
- Basic zombies chase and damage the player.
- Wave manager spawns enemies at configurable points.
- Score increments on kills.
- Health and wave state are available for HUD work.

## Content Roadmap

- Weapons: pistol, shotgun, SMG, assault rifle, grenades, rocket launcher.
- Deployables: barrels, mines, turrets, electric traps.
- Enemies: walker, runner, tank, exploder, spitter, mini-boss.
- Rooms: single-room test arena, door-spawn arena, multi-room survival map.
- Progression: score unlocks, cash buys, XP upgrades, combo rewards.
