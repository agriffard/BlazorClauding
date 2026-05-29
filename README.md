# BlazorClauding

[![NuGet](https://img.shields.io/nuget/v/BlazorClauding.svg)](https://www.nuget.org/packages/BlazorClauding/)
[![NuGet downloads](https://img.shields.io/nuget/dt/BlazorClauding.svg)](https://www.nuget.org/packages/BlazorClauding/)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A whimsical **Claude-Code-style loading spinner** for Blazor. It pairs an animated
glyph with a playful present-participle word — `Clauding`, `Reticulating`,
`Pondering`, `Caramelizing` — that can be **fixed**, **random**, or **cycling**
through the full list of 187 words.

> Inspired by the rotating status words the Claude CLI shows next to its spinner.

## Install

```bash
dotnet add package BlazorClauding
```

## Usage

Add the namespace to your `_Imports.razor`:

```razor
@using BlazorClauding
```

Then drop the component anywhere:

```razor
<ClaudingSpinner />                       @* default: "Clauding…" *@
<ClaudingSpinner Word="Pondering" />      @* a fixed word *@
<ClaudingSpinner Random="true" />         @* one random word at startup *@
<ClaudingSpinner Cycle="true" />          @* cycle through words *@
```

## Parameters

| Parameter      | Type                      | Default            | Description                                                        |
| -------------- | ------------------------- | ------------------ | ------------------------------------------------------------------ |
| `Word`         | `string?`                 | `null`             | Fixed word to show. Takes precedence over `Random`/`Cycle`.        |
| `Random`       | `bool`                    | `false`            | Pick one random word at startup.                                   |
| `Cycle`        | `bool`                    | `false`            | Change the word on an interval.                                    |
| `WordInterval` | `TimeSpan`                | `1.2s`             | How often the word changes when cycling.                           |
| `FrameInterval`| `TimeSpan`                | `80ms`             | How often the spinner glyph advances.                              |
| `Words`        | `IReadOnlyList<string>?`  | full built-in list | Custom word pool to draw from.                                     |
| `Frames`       | `IReadOnlyList<string>?`  | star frames        | Custom glyph animation frames (`StarFrames` / `BrailleFrames` provided). |
| `Suffix`       | `string`                  | `"…"`              | Text appended after the word.                                      |
| `CssClass`     | `string?`                 | `null`             | Extra CSS class(es) on the root element.                           |

Any extra attributes (e.g. `style`, `data-*`) are splatted onto the root element.

### Styling

The component ships scoped CSS and exposes CSS variables for theming:

```razor
<ClaudingSpinner Cycle="true"
                 style="font-size: 1.5rem; --clauding-color: #6d4aff;" />
```

| Variable                 | Purpose               |
| ------------------------ | --------------------- |
| `--clauding-color`       | Base color            |
| `--clauding-glyph-color` | Spinner glyph color   |
| `--clauding-word-color`  | Word color            |

Respects `prefers-reduced-motion`.

### The word list

All words are exposed via the static `ClaudingWords` class:

```csharp
ClaudingWords.All;        // IReadOnlyList<string> — all 187 words
ClaudingWords.Count;      // 187
ClaudingWords.Random();   // a single random word
```

## Project layout

```
BlazorClauding/
├── src/BlazorClauding/            # the Razor class library (NuGet package)
├── samples/BlazorClauding.Docs/   # Blazor WebAssembly docs + live samples
├── .github/workflows/
│   ├── nuget.yml                  # pack & push to NuGet.org on v* tags
│   └── deploy.yml                 # publish docs to GitHub Pages on push to main
└── README.md
```

## Build & run locally

```bash
dotnet build
dotnet run --project samples/BlazorClauding.Docs
```

The docs site is published to GitHub Pages:
**https://agriffard.github.io/BlazorClauding/**

## Releasing

- **Docs** deploy automatically on every push to `main`.
- **NuGet** publishes when you push a `v*` tag (set the `NUGET_API_KEY` repo secret first):

  ```bash
  git tag v1.0.0
  git push origin v1.0.0
  ```

## License

MIT
