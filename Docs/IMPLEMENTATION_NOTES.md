# Implementation notes

## Why the race is a runtime scene

The original game has large `.unity` files and numerous saved prefab references. Editing
those files without opening Unity risks breaking GUIDs. Instead, the source code introduces
`RaceBootstrap`, which adds an Online Race entry point to `Start_Screen` at runtime.
`RaceLobby` uses Photon PUN matchmaking and synchronized room properties. `RaceWorld`
creates a separate Unity scene with a compact course, camera, HUD, and avatars.

## Networking design

- Room limit: six players; host can start with two or more players.
- Private rooms: hidden from random matchmaking, join by six-character code.
- Quick Match: Photon room custom property `mode = race`.
- Synchronization: Photon `Room.CustomProperties["raceAt"]` sets shared countdown.
- Transform updates: PUN RaiseEvent code 51, sent unreliably every 70 milliseconds.
- Finishes: each user sets `Player.CustomProperties["finishTime"]` to a Photon network timestamp.
- Roster: sorted by ActorNumber; each player gets a different tint from a six-color palette.
- Rendering: existing Player Variant sprite, no generated/replaced character art.

## Future hardening

- Use PhotonView-instantiated network prefabs and lag-compensated position validation.
- Move finish ordering and false-start validation to a trusted authoritative server.
- Add disconnected-player recovery, host migration, spectator mode and tie-breaking.
- Test on simulated packet loss and high RTT before shipping.
- Art-direct a dedicated race track and use character animation controllers for run/jump.
- Improve accessibility (remappable buttons, screen-safe-area, visual state cues).
- Add automated Unity Test Framework run in CI once a Unity license is available.

## Save migration

Existing PlayerPrefs keys `Coins`, `StoredCoins`, `Score`, `HighScore`, and `BestTime`
are retained. New keys start with `TreasureQuest.`. Enemy kills and death penalties are
stored independently and scores recalculated consistently. Players who already earned
non-coin points in older versions may experience a one-time score recalculation because
the old code did not save the enemy-kill counter.

## Asset/performance review

The existing repository contains roughly 608 MB of tracked files and bundled audio/Photon
demos. No assets were deleted automatically because some may be loaded dynamically or
required by prefab references. Unity's menu option **Tools > Treasure Quest > Export Asset
Usage Report** exports a CSV of dependency-review candidates for a safe manual cleanup.

## Code visibility and verification

This package was created from a source-level read of the public repository. There is no
Unity Editor or Photon multiplayer simulator available in the execution environment.
**Compilation, scene binding, playable gameplay, and real multiplayer connectivity are not
claimed as verified**. The provided test plan is necessary before a release or merge.
