# GitHub Copilot Instructions for [RF] Fishing-ReWork (Continued)
## Mod Overview and Purpose
[RF] Fishing-ReWork (Continued) is an update of the Si-Cafes mod that reworks the fishing system in the RimWorld game. The mod adds new fish, fixes several issues from the original mod, and updates the fishing textures. The mod is compatible with other fishing mods, but not with the original fishing mod.
## Key Features and Systems
The mod adds the following key features and systems:
- Ice fishing spots that work on frozen river or lake water tiles
- Shellfish traps that automatically catch shellfish or tiny fish
- Tiny fish that can be grilled at the campfire
- Seafood separated from the raw meat category
- Fishmongers that sell corpses of fish and fish meat
- Compatibility with several other mods, including Vanilla Cooking Expanded and Vanilla Gourmet Parade
## Coding Patterns and Conventions
The mod follows the standard RimWorld coding conventions for C# and XML. Key coding patterns include:
- Using the Harmony patching framework for modding
- Defining new game objects and behaviors using XML and C#
- Using the RimWorld API to interact with the game world
## XML Integration
The mod uses XML to define new game objects and behaviors. The key XML files include:
- ThingDefsBuildings.xml: Defines new buildings and items, such as fishing spots, shellfish traps, and fishmongers.
- About.xml: Contains information about the mod, such as the author and description.
- TranslationTemplate.xml: Contains templates for translation files, which can be used to localize the mod for different languages.
## Harmony Patching
The mod uses the Harmony patching framework to modify the game code. Key Harmony patches include:
- Modifying the fishing mechanic to use the "Animals" skill instead of the "Crafting" skill
- Adding new fish to the game
- Modifying the behavior of existing fish
## Suggestions for Copilot
When using Copilot to assist with [RF] Fishing-ReWork (Continued), consider the following:
- Use Copilot to generate code snippets for new game objects and behaviors defined in XML.
- Use Copilot to generate Harmony patches for modifying existing game code.
- Use Copilot to generate C# code for new behaviors and interactions.
- Use Copilot to generate translations for the mod in different languages.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

