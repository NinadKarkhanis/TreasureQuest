#!/usr/bin/env python3
"""Quick project-structure validation. This is NOT Unity compilation."""
import json
import pathlib
import re
import sys

root = pathlib.Path(__file__).resolve().parents[1]
errors = []

def verify(condition, message):
    if not condition:
        errors.append(message)

version = root / "ProjectSettings/ProjectVersion.txt"
verify(version.is_file() and "2022.3.16f1" in version.read_text(encoding="utf-8"),
       "This CI requires Unity 2022.3.16f1")
manifest = root / "Packages/manifest.json"
verify(manifest.is_file(), "Missing Unity package manifest")
if manifest.is_file():
    deps = json.loads(manifest.read_text(encoding="utf-8"))["dependencies"]
    verify("com.unity.inputsystem" in deps, "Unity Input System package not found")
    verify("com.unity.ugui" in deps, "Unity UGUI package not found")

settings = root / "ProjectSettings/EditorBuildSettings.asset"
verify(settings.is_file(), "Missing scene build settings")
if settings.is_file():
    lines = settings.read_text(encoding="utf-8")
    scenes = re.findall(r"^\s*path: (Assets/Scenes/[^\n]+\.unity)$", lines, re.M)
    for name in ["Start_Screen", "End Screen"]:
        path = "Assets/Scenes/" + name + ".unity"
        verify(path in scenes and (root / path).is_file(), "Build scene missing: " + path)

paths = [
    "Assets/Photon/PhotonUnityNetworking/Code/PhotonNetwork.cs",
    "Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset",
    "Assets/Scripts/GameRules.cs",
    "Assets/Scripts/Multiplayer/RaceLobby.cs",
    "Assets/Scripts/Multiplayer/RaceWorld.cs",
    "Assets/Scripts/Multiplayer/RaceUi.cs",
    "Assets/Scripts/Multiplayer/RaceHoldButton.cs",
]
for path in paths:
    verify((root / path).is_file(), "Missing dependency: " + path)
for directory in ["Assets/Scripts/Multiplayer", "Assets/Editor"]:
    for code in (root / directory).glob("*.cs"):
        verify((root / (str(code.relative_to(root)) + ".meta")).is_file(),
               "Missing Unity meta: " + str(code.relative_to(root)))
print("PASS: Unity project preflight" if not errors else "\n".join("ERROR: " + e for e in errors))
if errors:
    sys.exit(1)
print("NOTE: Compiling Unity C# and building binaries requires the separate licensed Unity job.")
