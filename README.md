# Habitually
This is a habit tracker with heatmap feature

## How to run

Install the .NET 10 SDK, then run these commands from the repository root:

```powershell
dotnet build Habitually/Habitually.csproj
dotnet run --project Habitually/Habitually.csproj --launch-profile http
```

Open http://localhost:5002. Data stays in memory and resets when you reload.
The generated `Habitually/wwwroot/css/tailwind.css` is kept in the repository,
so running the app does not require Node.js.

To edit styling, install Node.js and run from the repository root:

```powershell
npm ci
npm run css:build
```

Use `npm run css:watch` in a second terminal while editing Razor classes or
`Habitually/Styles/tailwind.input.css`. Run `npm run css:build` afterwards to
regenerate the minified CSS, and include that file with any styling changes.
All design tokens live in the input stylesheet's Tailwind v4 `@theme` block;
dark colors use `[data-theme="dark"]`.
