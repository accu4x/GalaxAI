GalaxAI Blazor WebAssembly PWA (scaffold)

To create the project locally:
1) dotnet new blazorwasm -o galaxai-web --pwa
2) Add Markdig or a Blazor markdown renderer to render lore markdown from Azure Blob Storage
3) Configure appsettings to point to the Game API base URL and the Azure Blob Storage base URL
4) Implement pages: Index, World Dashboard, Lore, How to Connect, Dev Docs
5) Use placeholder assets in wwwroot/images and CSS in wwwroot/css

PWA notes:
- Service worker enabled for offline reading of lore
- Use HTTP caching for lore blobs
