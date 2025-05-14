using System.Collections;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Data;
using UnexpectedApis.Model;
using Uno.Extensions;


namespace UnexpectedApis.ViewModels;

[Bindable]
public class TableViewModel : ViewModelBase
{
    public IEnumerable Plants { get => GetProperty<IEnumerable>(); set => SetProperty(value); }

    public TableViewModel(DispatcherQueue dispatcherQueue)
    {
        Plants = new PlantCollection().ToObservableCollection();
    }
}
