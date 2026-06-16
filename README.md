# BeeneticToolkit — Unity Test Harness

Validates the published [BeeneticToolkit](https://github.com/Beenetic-Studios/BeeneticToolkit)
NuGet packages **on the Unity platform** — the things the netstandard MSTest suite can't see
(Mono/IL2CPP behaviour, AOT/code-stripping, cross-platform determinism, allocations).

This is an internal test project. It ships nothing.

## Why this exists

The main repo's MSTest suite runs on CoreCLR and proves *logical* correctness. Unity introduces a
different runtime (Mono in the editor, **IL2CPP/AOT** in player builds) plus managed-code stripping.
This harness consumes the **exact published packages** a user would install and exercises them there.

Phases (this is **Phase 1 — Foundation**):
1. **EditMode contract tests** (this) — one suite per package, runs under the editor's Mono runtime.
2. PlayMode + cross-platform **determinism golden-vector** tests.
3. **IL2CPP build smoke** — a built player that runs the contract suite (validates AOT/stripping,
   especially `AutoEnumItem`'s reflection-based self-registration).
4. GameCI automation; revive the gizmo visualizer playground.

## Setup

1. **Unity 6.3 (6000.3.x)**, any render pipeline (tests don't need one; 2D URP matches the old harness).
2. Install **NuGetForUnity**: Package Manager → **+** → *Add package from git URL…* →
   `https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity`
3. Ensure the **Test Framework** package is installed (Package Manager → Unity Registry →
   *Test Framework*; usually present by default).
4. `Assets/packages.config` lists the BeeneticToolkit packages — NuGetForUnity restores them into
   `Assets/Packages/` on load (or run *NuGet → Restore Packages*).
5. Open **Window → General → Test Runner → EditMode** and **Run All**.

## Layout

```
Assets/
  packages.config                         # NuGetForUnity: which packages to restore
  Tests/EditMode/
    BeeneticToolkit.Unity.EditModeTests.asmdef
    RandomContractTests.cs
    CollectionsContractTests.cs
    NumericsContractTests.cs
    LoggingContractTests.cs
    SpatialContractTests.cs
```

> If the `BeeneticToolkit.*` types don't resolve, open the asmdef in the Inspector and confirm the
> five `BeeneticToolkit.*.dll` entries are listed under **Assembly References** (Override References)
> alongside `nunit.framework.dll`.
