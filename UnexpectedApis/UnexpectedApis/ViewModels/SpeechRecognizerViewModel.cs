using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.UI.Dispatching;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;

namespace UnexpectedApisDemo.Shared.ViewModels;

public class SpeechRecognizerViewModel : ViewModelBase, IDisposable
{
    private readonly SpeechRecognizer _speechRecognizer = new SpeechRecognizer(new Windows.Globalization.Language("en-US"));
    private readonly DispatcherQueue _dispatcher;
    private string _lastResult = "";
    private bool _canRecognize = true;

    public SpeechRecognizerViewModel(DispatcherQueue dispatcher)
    {
        _speechRecognizer.HypothesisGenerated += HypothesisGenerated;
        _dispatcher = dispatcher;
    }

    public ICommand RecognizeCommand => GetOrCreateCommand(Recognize);

    public string LastResult
    {
        get => _lastResult;
        set
        {
            _lastResult = value;
            OnPropertyChanged();
        }
    }

    public bool CanRecognize
    {
        get => _canRecognize;
        set
        {
            _canRecognize = value;
            OnPropertyChanged();
        }
    }

    private async void Recognize()
    {
        try
        {
            CanRecognize = false;
            await _speechRecognizer.CompileConstraintsAsync();
            var result = await _speechRecognizer.RecognizeAsync();
            LastResult = result.Text;                
        }
        finally
        {
            CanRecognize = true;
        }
    }

    private async void HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
    {
        _dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () =>
        {
            LastResult = args.Hypothesis.Text;
        });
    }

    public void Dispose()
    {
        _speechRecognizer.HypothesisGenerated -= HypothesisGenerated;
        _speechRecognizer.Dispose();
    }
}
