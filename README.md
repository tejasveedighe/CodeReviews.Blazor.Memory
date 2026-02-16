# MemoryGame

- The simple memory game is a built using Blazor InteractiveServer Render Mode. This game was one of my practice projects to learn more about Blazor.

- Live App: [blazor-memory-app](blazormemorygame-gxgkdkddctgtdpd2.eastasia-01.azurewebsites.net)

- To run the code:
    1. Clone the repo
    2. Install the dotnet-sdk based on your OS.
    3. Run `dotnet restore`
    4. Run `dotnet run` from CLI or use your Visual Studio installation.
    
- Features:
    1. Interactive Game of memory to challenge your visual memory.
    2. Database that scores your game score, difficulty and time played.
    3. Interactive UI for card grid.
    4. Timer that changes dynamically for each difficulty.
    5. Score board for comparing your scores.

- Tech Stack:
    - Frontend: 
        - Framework: Blazor, Render Mode: InteractiveServer
        - UI - MudBlazor
    - Backend:
        - Simple JSON file for storing stores.
        <!-- - DB: SQL Lite, [sqlite-net-sqlcipher](https://www.nuget.org/packages/sqlite-net-sqlcipher) -->


- Project Architecture:
    - Project Directory:
        - MemoryGame (directory)
            - MemoryGame (Main Server App)
            - MemoryGame.Client 
        - MemoryGame.Shared (Services, Models and Store)

    - Component Communication:
        - Single Data Store (Singleton)
        - Event Base communication (Publisher Subscriber Model)

    - Project Routes - 
        - "/" - Home
        - "/about" - Aboute Page
        - "/game" - Game Page