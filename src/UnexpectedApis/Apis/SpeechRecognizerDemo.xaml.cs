using Windows.Globalization;
using Windows.Media.SpeechRecognition;

namespace UnexpectedApis.Apis;

public sealed partial class SpeechRecognizerDemo : UserControl
{
    private SpeechRecognizer? _recognizer;

    public SpeechRecognizerDemo()
    {
        InitializeComponent();
        Unloaded += (_, _) => Release();
    }

    public static readonly DependencyProperty ResultProperty =
        DependencyProperty.Register(nameof(Result), typeof(string), typeof(SpeechRecognizerDemo), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(string), typeof(SpeechRecognizerDemo), new PropertyMetadata("Press the button and say something."));

    public static readonly DependencyProperty CanRecognizeProperty =
        DependencyProperty.Register(nameof(CanRecognize), typeof(bool), typeof(SpeechRecognizerDemo), new PropertyMetadata(true));

    public string Result
    {
        get => (string)GetValue(ResultProperty);
        set => SetValue(ResultProperty, value);
    }

    public string Status
    {
        get => (string)GetValue(StatusProperty);
        set => SetValue(StatusProperty, value);
    }

    public bool CanRecognize
    {
        get => (bool)GetValue(CanRecognizeProperty);
        set => SetValue(CanRecognizeProperty, value);
    }

    private async void OnRecognizeClick(object sender, RoutedEventArgs e)
    {
        CanRecognize = false;
        try
        {
            if (_recognizer is null)
            {
                _recognizer = new(new Language("en-US"));
                _recognizer.HypothesisGenerated += OnHypothesis;
            }

            Status = "Listening...";
            await _recognizer.CompileConstraintsAsync();
            var result = await _recognizer.RecognizeAsync();
            Result = result.Text;
            Status = string.IsNullOrEmpty(result.Text) ? "Didn't catch that." : "Done.";
        }
        catch (Exception ex) when (ex is NotImplementedException or NotSupportedException or UnauthorizedAccessException)
        {
            Status = "Speech recognition isn't available here, or the microphone is blocked.";
        }
        catch (Exception ex)
        {
            Status = $"Speech recognition failed: {ex.Message}";
        }
        finally
        {
            CanRecognize = true;
        }
    }

    private void OnHypothesis(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
    {
        string text = args.Hypothesis.Text;
        DispatcherQueue.TryEnqueue(() => Result = text);
    }

    private void Release()
    {
        if (_recognizer is not null)
        {
            _recognizer.HypothesisGenerated -= OnHypothesis;
            _recognizer.Dispose();
            _recognizer = null;
        }
    }
}
