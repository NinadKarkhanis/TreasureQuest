# Continuous integration / Unity compilation

This branch has an automatic workflow at `.github/workflows/unity-compile.yml`, triggered when changes are pushed to `feature/race-multiplayer-and-gameplay-polish`, or manually using the Actions tab.

The workflow runs a zero-dependency project preflight check, then uses the actual **Unity 2022.3.16f1 Editor** in [GameCI Unity Builder v4](https://game.ci/docs/github/builder/) to compile all Unity assemblies and produce a downloadable **Linux desktop build**.

## Required setup (one time)

Unity Editor is not licensed automatically by GitHub. Go to repository **Settings → Secrets and variables → Actions**, then add:
- **Unity Personal**: `UNITY_LICENSE` (the complete content of your activated Unity `.ulf` file), `UNITY_EMAIL`, and `UNITY_PASSWORD`.
- **Unity Pro**: `UNITY_SERIAL`, `UNITY_EMAIL`, and `UNITY_PASSWORD`.

Activate your license with Unity Hub first. On Windows a Unity license is commonly in `C:\ProgramData\Unity\Unity_lic.ulf`. Follow [GameCI Activation](https://game.ci/docs/github/activation/) for details. **Never share these values in chat or commit them to Git.** The workflow fails with an explicit setup message if secrets are not configured.

## Compile the game

1. Open **Actions → Treasure Quest - Unity compile and build**.
2. Select **Run workflow**, choosing branch `feature/race-multiplayer-and-gameplay-polish`.
3. Check **Unity project preflight**, then **Unity 2022.3.16f1 Linux build**.
4. If the build completes, download artifact `TreasureQuest-Linux-<commit SHA>` from the workflow summary.
5. If compilation fails, open the **Compile and build the game** step to see Unity's compiler errors.

The Linux build validates C# compilation and Unity scene serialization. It does **not** prove Android builds, online networking, performance, or complete gameplay behavior. Those need separate platform builds and multi-device playtesting. Android exports also require Android Build Support, ARM64 configuration, signing, and compatible SDK settings.
