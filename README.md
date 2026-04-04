# Build Your PC 3D Project

## Overview

This project is a **3D PC building simulation** created using **Unity** and **Blender**.
The goal is to allow users to explore and assemble computer components inside a virtual environment.

## Features

* Interactive **3D PC components**
* **Gamer room environment** created in Blender
* Modular project structure
* Easy to expand with new components

## Project Structure

The project follows a standard Unity organization, with core game logic and assets located in the Assets/ directory:

* Core Folders
  
Scripts/: Contains C# source code, including player interaction and simulation logic.

Prefabs/: Reusable GameObjects (e.g., PC components, Settings Panel). The main UI is managed via the Canvas prefab.

Scenes/: All available scenes.

Room/: Specific 3D models, textures, and environment assets for the building workspace.

Components/: Specialized assets for the hardware simulation.

* Assets & Resources
  
Resources/: Assets that are dynamically loaded via script at runtime.

Images/ & Sounds/: All 2D textures, UI sprites, and audio clips.

Fonts/ & TextMesh Pro/: Typography and UI styling assets.

* Settings & Configuration
  
Settings/: Configuration files for the project.

InputSystem_Actions: Defines the control mapping for keyboard and mouse interactions.

MainMixer: Manages audio levels and effects routing.

## How to Run

1. Clone the repository
2. Open the project using **Unity Hub**
3. Load the main scene
4. Press **Play** to run the project
