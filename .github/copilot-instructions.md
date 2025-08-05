# GitHub Copilot Instruction File for [RF] Fishing-ReWork (Continued)

## Mod Overview and Purpose

The [RF] Fishing-ReWork (Continued) mod is a comprehensive rework of the original Rainbeau's fishing mod for RimWorld, updated by Si-Cafes and further modified by Mlie. This mod aims to enhance the fishing experience in RimWorld by introducing new mechanics, fixing existing issues, and improving textures while retaining the core essence of the original fishing mod.

## Key Features and Systems

- **Fishing as a Work Type**: Fishing operates as a custom work type primarily dependent on the "Animals" skill. Pawns with higher skill levels will fish more efficiently and have a greater likelihood of successful catches, which can also serve as a joy activity for pawns passionate about Animals.
  
- **Variety of Catchable Items**: Biome-dependent fish species and other aquatic creatures like eels and squids can be caught, with specific conditions like marsh vs. ocean tiles influencing the catch. Shellfish traps function autonomously to catch shellfish or small fish.
  
- **Ice Fishing**: Special ice fishing spots are added for use on frozen waters in conjunction with mods like "Permafrost", enhancing gameplay in cold biomes.
  
- **New Textures and Items**: Complete overhaul of textures for fish, fishing spots, and fishing tools, alongside the introduction of new fish and shellfish species.
  
- **Integration and Compatibility**: Works alongside other modded biomes and cooking mods, with special patches to use seafood as a substitute for raw meat across various mods.

- **Caravan and Trading Enhancements**: Introduction of fishmongers in caravans to ensure accessibility on maps without natural water sources, with fish carcasses and meat available through traders.

## Coding Patterns and Conventions

- **Object-Oriented Design**: The mod utilizes classes like `Building_FishingSpot`, `Building_ShellfishTrap`, and `Controller` to encapsulate functionality related to different components of the mod.
  
- **Modular and Extensible Code**: The use of abstract classes and methods, such as `PlaceWorker` and `WorkGiver_Scanner` derivatives, allows easy expansion and integration with other mod features.

- **Consistency in Naming**: Classes and methods follow a consistent naming convention, reflecting their purpose and relationship to the fishing system.

## XML Integration

- **Defs File Rework**: The Defs file has been rewritten to fix issues, including patching for compatibility with various mods and ensuring new fish and items are properly defined for spawning and processing.

- **Separation of Seafood**: Unique categorization and handling of seafood distinct from regular raw meat, reflecting gameplay changes in trading and processing.

## Harmony Patching

- **Patch Applications**: Harmony patches are used to modify base game behavior for new mod features and to ensure compatibility with other mods, such as applying patches to allow seafood to substitute for raw meat in existing meal recipes.

- **Non-Intrusive Modifications**: Patches avoid directly altering core game files, allowing the mod to work seamlessly with other modifications and reducing the likelihood of conflicts.

## Suggestions for Copilot

- **Autocomplete for Repetitive Structures**: Use Copilot to assist in generating repetitive parts of the mod, such as XML configurations for new fish types or item definitions.

- **Refactoring and Optimization**: Leverage Copilot for code optimization suggestions, especially in areas like job drivers (`JobDriver_CatchFish`) to enhance performance.

- **Harmony Optimization**: Get recommendations on effective Harmony patch patterns to ensure mod compatibility and minimize errors.

- **Documentation and Comments**: Encourage comprehensive documentation and inline comments for clarity and maintainability, ensuring future developers can easily understand and extend the mod.

- **Multi-Language Support**: Facilitate the addition of translations by suggesting structured approaches and best practices, enhancing the mod's accessibility for a global audience.

By following these guidelines and suggestions, developers can leverage GitHub Copilot to maintain and enhance the [RF] Fishing-ReWork mod, providing an engaging and seamless experience for RimWorld players.
