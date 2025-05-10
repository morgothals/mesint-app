# Rolling Cube Solver – Unity UI Toolkit & Pathfinding Algorithms

Made by Péter Borbás

This project is an interactive educational visualization built with **Unity** and **UI Toolkit**, designed to demonstrate various **search algorithms** (like DFS, BFS, Backtrack, A*) by solving a constrained pathfinding problem on a grid-based board.

## 🧩 The Problem

On a **10x10 board**, there are:

- 🔲 **10 gray cubes** placed as obstacles (fixed, cannot be moved)
- 🟥🟦 **1 colored cube**, with:
  - 5 blue faces
  - 1 red face (initially on a random side)

The goal is to **roll the cube** to a designated goal cell (`C`) by tilting it onto adjacent empty cells (up/down/left/right). However:

> ⚠️ **Constraint**: The red face must never touch the board surface during the movement.

## 🎯 Objectives

- Solve the puzzle using various **search algorithms** (DFS, BFS, Backtracking, A\*, Random)
- Avoid illegal states (e.g. red face on the bottom)
- Visualize each step with a UI-enhanced animated cube
- Allow users to interactively:
  - Generate new boards
  - Select algorithm type
  - Set max search depth / restart count
  - Monitor solution progress
  - Pause / Resume / Reset the game

## 🧠 Core Concepts

- Search nodes and states are tracked using a custom `BoardState` and `CubeState` system.
- The red face constraint is strictly enforced by validation in each algorithm.
- The Unity UI is built entirely with **UI Toolkit (UXML + USS)** 

## 🎮 Features

- 👁️‍🗨️ Live step-by-step cube movement and color updates
- 🔄 Restart / Pause / Exit functionality
- 📊 Solver UI with step counter, solver type, and dynamic solver settings
- 🧱 Grid-based obstacle generation (random or fixed)
- 💡 World-space labels display the step number directly on the board
- 🌫️ Fade-out markers and resettable movement path

## 🛠️ Technologies Used

- Unity 6.1 with UI Toolkit
- Custom search algorithm implementations (DFS, BFS, Backtrack, A*, Random)
- Runtime component communication via event-driven architecture

## 🧪 How to Run

1. Clone this repository into your Unity project's `Assets/` folder.
2. Open the project in **Unity 2023.2+**.
3. Open the main scene and enter Play Mode.
4. Use the in-game UI to:
   - Generate a board
   - Select a solver algorithm
   - Adjust parameters (max steps, restarts, etc.)
   - Start solving and watch the cube animate step-by-step

> ⚠️ **Note:**  
> This project relies on a few **third-party assets** (for visual enhancements like shaders, materials, fonts, and decorative UI elements) that cannot be redistributed publicly.  
> As a result:
> - Some shaders may fail to compile or appear pink  
> - Some scene elements may appear missing or unstyled  

This does not affect the core **functionality**, algorithms, or UI logic.  
You're encouraged to replace the missing visuals with your own or use open-source alternatives.

![image](https://github.com/user-attachments/assets/b584830e-00f3-4c62-a193-bbace48228fe)

---

Made with 💙 for the *Artificial Intelligence Basics* course project.






