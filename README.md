# Slime Evolution

A domain-layer implementation for the **《史莱姆进化》** project. This repository currently contains:

- A full game design document (`docs/slime-evolution-gdd.md`).
- A C# domain library (`src/SlimeEvolution.Core`) that models core systems such as
  slime generation, mutation, breeding grounds, the economy, and the task system.

The code is written as a .NET 8 class library so it can be reused inside a Unity
project or any other host runtime.

## Repository layout

```
├── docs/
│   ├── slime-evolution-gdd.md     # High-level game design document (Chinese)
│   └── simulation-example.md      # Sample code illustrating the core services
├── src/
│   ├── SlimeEvolution.Core/
│   │   ├── Configuration/         # Balance configuration & default databases
│   │   ├── Domain/                # Core entities (slimes, traits, skills, etc.)
│   │   ├── Services/              # Mutation & economy services
│   │   ├── Tasks/                 # Lightweight quest/task evaluation logic
│   │   └── Utilities/             # Random helpers & weighted pickers
│   └── SlimeEvolution.Cli/        # Command-line playable prototype
└── README.md
```

## Getting started

> ℹ️ The `dotnet` CLI is not bundled with this environment. The instructions
> below target a local developer machine with the .NET SDK installed.

1. Install the .NET 8 SDK from [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
2. Restore and build the core library:

   ```bash
   dotnet build src/SlimeEvolution.Core/SlimeEvolution.Core.csproj
   ```

3. Reference the library from a Unity project (via an Assembly Definition) or a
   separate gameplay simulation project to start integrating the systems.

### Run the text prototype

Launch the command-line sandbox to try the core breeding and economy loops:

```bash
dotnet run --project src/SlimeEvolution.Cli/SlimeEvolution.Cli.csproj
```

The prototype lets you inspect the breeding ground, trigger split cycles,
review mutation logs, and sell slimes for gold directly from a terminal.

## Next steps

- Hook these domain services into Unity MonoBehaviours / ScriptableObjects for
  runtime integration.
- Extend the configuration database with additional traits, skills, arenas, and
  accessories.
- Implement persistence and UI layers that consume the domain logic provided by
  `SlimeEvolution.Core`.
