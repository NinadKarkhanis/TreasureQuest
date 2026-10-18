# Test plan — run this in Unity after installation

## Minimum verification

- [ ] Open project in Unity 2022.3.16f1 without C# compilation errors.
- [ ] Run Tools > Treasure Quest > Run Logic Tests; see 9 tests pass in Console.
- [ ] Main menu loads and existing single-player navigation is still functional.
- [ ] Player runs at comparable speed at 30, 60, and 120 fps.
- [ ] Player can jump from ground, coyote-jump briefly after leaving a ledge, and buffer a jump.
- [ ] Jump, landing, and animation work on Android and Windows controls.
- [ ] Checkpoint respawn loses one life, preserves original scale, and does not repeatedly take damage.
- [ ] Falling and using all 10 lives leads to End Screen and resets the active run.
- [ ] Collecting 2 coins and defeating 1 enemy gives 120 points; death reduces by 100.
- [ ] Game restart preserves high score but resets run score.
- [ ] Saved coin count does not duplicate when the same scene reloads.
- [ ] Return to a saved checkpoint after exiting and reopening the gameplay scene.
- [ ] Speedrun best time updates and stays saved; timers pause when the game pauses.
- [ ] Invalid leaderboard input is rejected without crashes.
- [ ] Finish reaches Credits reliably; tutorial remains playable.

## Online race verification (two or more devices)

- [ ] Online Race opens from Start_Screen.
- [ ] Private room is created; room code shown; second client joins using code.
- [ ] Quick Match joins an open matching room or creates one.
- [ ] Player names are visible and distinct colors appear over the existing player artwork.
- [ ] Room roster updates if a player joins/leaves; 7th player cannot enter a full room.
- [ ] Only room host sees enabled START RACE (requires at least two participants).
- [ ] All players start after the same synchronized countdown.
- [ ] Remote players move with smooth interpolation and are not controlled by local keys.
- [ ] Android virtual controls move and jump; desktop A/D/arrows/Space also work.
- [ ] Reaching the gold finish line records a finish time and correct placing.
- [ ] Losing the connection does not corrupt saved offline progress.
- [ ] LEAVE returns to single-player main menu with no duplicate EventSystems.
- [ ] Test with 2, 5, and 6 players across Android/Windows/WebGL builds.

## Performance testing

- [ ] Run Unity Profiler CPU, GC allocations, Physics2D, and Memory views on a real Android device.
- [ ] Run Tools > Treasure Quest > Export Asset Usage Report; manually inspect candidates.
- [ ] Verify no excessive per-frame Debug.Log calls in gameplay.
- [ ] Profile audio memory/decompression and startup scene time.
- [ ] Check UI alignment on phones with notches, unusual aspect ratios, and tablets.

## Release checks

- [ ] Android application identifier and signing keystore match your intended distribution.
- [ ] Latest target API requirements, ARM64 + IL2CPP build, permissions and privacy policy are verified.
- [ ] Photon service App ID and rate/CCU limits are appropriate for your user count.
- [ ] Third-party fonts, sprites, sound effects, music, and Photon assets have valid redistribution rights.
- [ ] Update the project README with screenshots, working builds, and attribution.
