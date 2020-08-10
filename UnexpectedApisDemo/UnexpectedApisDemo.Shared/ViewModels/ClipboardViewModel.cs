using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;

namespace UnexpectedApisDemo.Shared.ViewModels
{
    public class ClipboardViewModel : ViewModelBase, IDisposable
    {
        private bool _isObservingContentChanged = false;
        private string _lastContentChangedDate = "";
        private string _text = "";

        public ClipboardViewModel()
        {
        }

        public void Dispose()
        {
            if (_isObservingContentChanged)
            {
                Clipboard.ContentChanged -= Clipboard_ContentChanged;
            }
        }

        public bool IsObservingContentChanged
        {
            get => _isObservingContentChanged;
            set
            {
                _isObservingContentChanged = value;
                RaisePropertyChanged();
            }
        }

        public string LastContentChangedDate
        {
            get => _lastContentChangedDate;
            set
            {
                _lastContentChangedDate = value;
                RaisePropertyChanged();
            }
        }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged();
            }
        }

        public ICommand ClearCommand => GetOrCreateCommand(Clear);

        public ICommand CopyCommand => GetOrCreateCommand(Copy);

        public ICommand PasteCommand => GetOrCreateCommand(Paste);

        public ICommand FlushCommand => GetOrCreateCommand(Flush);

        public ICommand ToggleContentChangedCommand => GetOrCreateCommand(ToggleContentChange);

        private void Clear() => Clipboard.Clear();

        private void Copy()
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(Text);
            Clipboard.SetContent(dataPackage);
        }

        private async void Paste()
        {
            var content = Clipboard.GetContent();
            Text = await content.GetTextAsync();
        }

        private void Flush() => Clipboard.Flush();

        private void ToggleContentChange()
        {
            IsObservingContentChanged = !IsObservingContentChanged;
            if (IsObservingContentChanged)
            {
                Clipboard.ContentChanged += Clipboard_ContentChanged;
            }
            else
            {
                Clipboard.ContentChanged -= Clipboard_ContentChanged;
            }
        }

        private void Clipboard_ContentChanged(object sender, object e)
        {
            LastContentChangedDate = DateTime.UtcNow.ToLongTimeString();
        }
    }
}
