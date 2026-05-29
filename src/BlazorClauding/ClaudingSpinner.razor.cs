using Microsoft.AspNetCore.Components;

namespace BlazorClauding;

/// <summary>
/// A whimsical Claude-Code-style loading spinner: an animated glyph next to a
/// playful present-participle word such as <c>Clauding</c> or <c>Reticulating</c>.
/// The word can be fixed, random, or cycle through the full word list.
/// </summary>
public partial class ClaudingSpinner : ComponentBase, IDisposable
{
    /// <summary>
    /// Default frames: a pulsing star that grows from a dot to a full sparkle and
    /// back, matching the Claude Code spinner.
    /// </summary>
    public static readonly string[] StarFrames = BuildStarFrames();

    // Append U+FE0E (text variation selector) so browsers render these dingbats
    // as monochrome text glyphs that honor CSS `color`, not as colored emoji.
    private static string[] BuildStarFrames()
    {
        const string vs15 = "︎"; // VS15 text presentation selector
        char[] stars = ['·', '✢', '✳', '✶', '✻', '✺', '✷', '✶', '✳', '✢'];
        var frames = new string[stars.Length];
        for (int i = 0; i < stars.Length; i++)
            frames[i] = stars[i] + vs15;
        return frames;
    }

    /// <summary>Alternate braille spinner frames.</summary>
    public static readonly string[] BrailleFrames =
        ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"];

    private Timer? _frameTimer;
    private Timer? _wordTimer;
    private int _frameIndex;
    private string _word = "Clauding";
    private readonly Random _random = new();

    /// <summary>
    /// Fixed word to display. When set, takes precedence over <see cref="Random"/>
    /// and <see cref="Cycle"/> for the initial word.
    /// </summary>
    [Parameter] public string? Word { get; set; }

    /// <summary>Pick a single random word at startup. Ignored when <see cref="Word"/> is set.</summary>
    [Parameter] public bool Random { get; set; }

    /// <summary>Cycle through words on an interval (see <see cref="WordInterval"/>).</summary>
    [Parameter] public bool Cycle { get; set; }

    /// <summary>How often the word changes when <see cref="Cycle"/> is enabled. Default 1.2s.</summary>
    [Parameter] public TimeSpan WordInterval { get; set; } = TimeSpan.FromSeconds(1.2);

    /// <summary>How often the spinner glyph advances. Default 120ms.</summary>
    [Parameter] public TimeSpan FrameInterval { get; set; } = TimeSpan.FromMilliseconds(120);

    /// <summary>Custom set of words to draw from. Defaults to <see cref="ClaudingWords.All"/>.</summary>
    [Parameter] public IReadOnlyList<string>? Words { get; set; }

    /// <summary>Custom animation frames for the glyph. Defaults to <see cref="StarFrames"/>.</summary>
    [Parameter] public IReadOnlyList<string>? Frames { get; set; }

    /// <summary>Text appended after the word. Default <c>"…"</c>.</summary>
    [Parameter] public string Suffix { get; set; } = "…";

    /// <summary>Extra CSS class(es) applied to the root element.</summary>
    [Parameter] public string? CssClass { get; set; }

    /// <summary>Splatted attributes (e.g. <c>style</c>, <c>data-*</c>).</summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? AdditionalAttributes { get; set; }

    private IReadOnlyList<string> WordPool => Words is { Count: > 0 } ? Words : ClaudingWords.All;
    private IReadOnlyList<string> FramePool => Frames is { Count: > 0 } ? Frames : StarFrames;

    private string CurrentFrame => FramePool[_frameIndex % FramePool.Count];
    private string CurrentWord => _word;

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        if (!string.IsNullOrWhiteSpace(Word))
            _word = Word!;
        else if (Random || Cycle)
            _word = PickWord();
    }

    /// <inheritdoc />
    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender)
            return;

        _frameTimer = new Timer(_ => Advance(ref _frameIndex, FramePool.Count, animateWord: false),
            null, FrameInterval, FrameInterval);

        if (Cycle)
            _wordTimer = new Timer(_ => CycleWord(), null, WordInterval, WordInterval);
    }

    private string PickWord() => WordPool[_random.Next(WordPool.Count)];

    private void Advance(ref int index, int modulo, bool animateWord)
    {
        index = modulo == 0 ? 0 : (index + 1) % modulo;
        _ = InvokeAsync(StateHasChanged);
    }

    private void CycleWord()
    {
        _word = PickWord();
        _ = InvokeAsync(StateHasChanged);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _frameTimer?.Dispose();
        _wordTimer?.Dispose();
        GC.SuppressFinalize(this);
    }
}

