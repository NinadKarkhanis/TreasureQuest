# Treasure Quest — Gameplay and Online Race Upgrade

This is an **additive source-code upgrade** for the existing Unity project:
`https://github.com/NinadKarkhanis/TreasureQuest`.

The original repository is not included in this archive. Keep its existing scenes, graphics,
audio, Photon PUN 2 plugin, TextMesh Pro plugin, and `Packages/` directory intact.

## Installation

1. Back up your project and create a Git branch (recommended):
   `git checkout -b feature/online-race`
2. Extract this archive somewhere outside the project.
3. Run from a terminal:
   `python /path/to/TreasureQuest-upgrade/apply_upgrade.py /path/to/TreasureQuest`
4. The installer backs up overwritten files to a sibling folder **outside** the repository.
5. Open the upgraded project using **Unity 2022.3.16f1** (the version in `ProjectVersion.txt`).
6. For an **opt-in** Android ARM64/IL2CPP conversion, add `--enable-android-arm64` to the installer command
   (requires Android IL2CPP Build Support in Unity Hub).
7. Wait for compilation and inspect the Console. Resolve any asset/plugin issues before building.
8. Open the Start_Screen scene and press Play. An **ONLINE RACE** button will appear.
9. Build two distinct standalone or Android clients to test network joining. One Unity Editor
   plus one standalone build is fine; two instances from one Editor are not supported.
10. Run **Tools > Treasure Quest > Run Logic Tests** and
   **Tools > Treasure Quest > Export Asset Usage Report**.

The online mode builds its own scene, terrain and touch controls **at runtime**, so no
existing gameplay scene or prefab needs to be manually edited. It reuses the already-
installed Photon PUN 2 integration and loads the existing character's sprite from
`Assets/Resources/Player Variant.prefab` when available. Other elements are fallback shapes.

## Online race features

- Create a private room and share its six-character join code.
- Enter a friend's code, or choose Quick Match for public matchmaking.
- 2–6 players can participate; the room holds at most six players.
- Everyone sees a synchronized countdown based on `PhotonNetwork.Time`.
- The same sprite has a different hue/tint for each player (six distinct palette entries).
- Each character has a name label above their head.
- Keyboard: A/D or arrows to move, Space/Up to jump. Android: on-screen left/right/jump.
- A side-scrolling obstacle course with checkpoints and a visible gold finish line.
- Live positional updates and finishing order/time shown in race standings.
- Return to the single-player main menu using **LEAVE**.

**Service prerequisite:** The repo contains `Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset`
with a Photon Realtime App ID. It must be an active Photon app with enough CCU capacity.
The race relies on a third-party online service; it cannot be tested without connectivity.

**Fair-play limitation:** Position and finish submissions are controlled by clients, not an
authoritative multiplayer server. This is suitable for friendly races, **not** prize money
or anti-cheat competitive rankings. Host reconnect, mobile suspension and packet-loss
behaviors also need real-world testing.

## Single-player changes

- Delta-time-independent Rigidbody2D movement, buffered jumps and coyote time.
- Death lockout and brief post-respawn immunity.
- Correct score persistence across scene changes and death penalties.
- Checkpoint/lives persistence and reset logic.
- Non-duplicate coin collection after revisiting a scene (position-based IDs).
- Safer leaderboard input parsing and reusable game-rule logic tests.
- Safer scene-name navigation and timer display/formatting.
- Null checks for optional sound references, smoother camera and safer teleports.
- Editor asset-review report to find potential unused assets **without auto-deletion**.
- Optional Android package-identifier update, with opt-in ARM64/IL2CPP in the installer.

## Known risks and manual validation

- Scene/prefab inspector references are inherited from the original game. Verify them in Unity.
- The original `SceneManager.cs` class name `level1` is retained to avoid breaking scene bindings.
- The same coin coordinates within one scene produce the same automatically generated save ID.
- `PlayerPrefs` is appropriate for casual offline progress, not tamper-proof cloud saving.
- Scene-specific animations, artwork, and touch controls must be reviewed on actual Android devices.
- Android signing, target SDK and store compliance must be finalized inside Unity before publication.
- Unity and Photon packages have not been upgraded blindly; test major version changes separately.
- This package does **not** remove imported third-party audio or networking demos automatically.
  Use the asset report and delete only after checking scene/prefab/runtime dependencies.

See `Docs/TEST_PLAN.md`, `Docs/IMPLEMENTATION_NOTES.md`, and `Docs/THIRD_PARTY_ASSETS.md`.
