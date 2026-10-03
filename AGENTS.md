Project: "Habitually", a desktop-only habit tracker with a GitHub-style heatmap.
Stack: Blazor WebAssembly (C#). Style with Tailwind CSS utility classes.
Design source: my Figma screenshots. Match colors, spacing, and layout closely.
Rules:
- Put all design tokens in the @theme block in Habitually/Styles/tailwind.input.css, with dark mode overrides using [data-theme="dark"].
- Build reusable components in /Components (Sidebar, HabitCard, StatCard, HeatmapGrid, ProgressRing, Modal, ToggleSwitch).
- Put logic in /Services (HabitService, StreakCalculator), not inside .razor files.
- Keep code simple and commented, since I am a student who must explain it.
- Work on one feature at a time and do not touch unrelated files.
