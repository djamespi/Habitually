Project: "Habitually", a desktop-only habit tracker with a GitHub-style heatmap.
Stack: Blazor WebAssembly (C#) with pure CSS only. No Bootstrap, Tailwind, or CSS frameworks.
Design source: my Figma screenshots. Match colors, spacing, and layout closely.
Rules:
- Put all colors, spacing, and radius values in wwwroot/css/tokens.css as CSS variables, with a dark mode override using [data-theme="dark"].
- Build reusable components in /Components (Sidebar, HabitCard, StatCard, HeatmapGrid, ProgressRing, Modal, ToggleSwitch).
- Put logic in /Services (HabitService, StreakCalculator), not inside .razor files.
- Persist data with localStorage.
- Keep code simple and commented, since I am a student who must explain it.
- Work on one feature at a time and do not touch unrelated files.