using Microsoft.UI.Xaml.Data;

namespace UnexpectedApis.Apis;

public enum PlantKind { Fruit, Vegetable }

[Bindable]
public sealed partial class Plant : ObservableObject
{
    public Plant(string name, int count, PlantKind kind, DateTimeOffset plantDate, bool isWatered)
    {
        PlantName = name;
        PlantsCount = count;
        Kind = kind;
        PlantDate = plantDate;
        IsWatered = isWatered;
    }

    public PlantKind[] Kinds { get; } = Enum.GetValues<PlantKind>();

    [ObservableProperty]
    public partial string PlantName { get; set; }

    [ObservableProperty]
    public partial int PlantsCount { get; set; }

    [ObservableProperty]
    public partial PlantKind Kind { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlantDateText))]
    public partial DateTimeOffset PlantDate { get; set; }

    [ObservableProperty]
    public partial bool IsWatered { get; set; }

    public string PlantDateText => PlantDate.ToString("d");

    public static List<Plant> CreateAll() =>
    [
        Create("Tomato", 12, PlantKind.Vegetable, 2021, 5, 29, true),
        Create("Cucumber", 10, PlantKind.Vegetable, 2021, 6, 15, true),
        Create("Bell Pepper", 15, PlantKind.Vegetable, 2021, 4, 15, false),
        Create("Lettuce", 8, PlantKind.Vegetable, 2021, 4, 1, true),
        Create("Strawberry", 35, PlantKind.Fruit, 2021, 5, 1, true),
        Create("Carrot", 5, PlantKind.Vegetable, 2021, 3, 15, false),
        Create("Watermelon", 1, PlantKind.Fruit, 2021, 6, 1, true),
        Create("Blueberries", 31, PlantKind.Fruit, 2021, 6, 20, true),
        Create("Eggplant", 7, PlantKind.Vegetable, 2021, 5, 10, false),
        Create("Broccoli", 9, PlantKind.Vegetable, 2021, 4, 20, false),
        Create("Raspberry", 18, PlantKind.Fruit, 2021, 5, 25, false),
        Create("Pumpkin", 3, PlantKind.Fruit, 2021, 6, 10, true),
        Create("Green Beans", 50, PlantKind.Vegetable, 2021, 5, 5, true),
        Create("Kale", 10, PlantKind.Vegetable, 2021, 4, 15, true),
        Create("Grapes", 25, PlantKind.Fruit, 2021, 9, 1, true),
        Create("Potato", 6, PlantKind.Vegetable, 2021, 10, 15, false),
        Create("Orange", 20, PlantKind.Fruit, 2021, 11, 1, true),
        Create("Zucchini", 11, PlantKind.Vegetable, 2021, 8, 20, false),
        Create("Apple", 30, PlantKind.Fruit, 2021, 10, 1, true),
        Create("Spinach", 5, PlantKind.Vegetable, 2021, 7, 15, true),
        Create("Pear", 15, PlantKind.Fruit, 2021, 8, 1, true),
        Create("Celery", 7, PlantKind.Vegetable, 2021, 9, 10, true),
        Create("Pineapple", 2, PlantKind.Fruit, 2021, 12, 1, true),
        Create("Cherry", 40, PlantKind.Fruit, 2021, 6, 1, true),
        Create("Mango", 8, PlantKind.Fruit, 2021, 9, 15, true),
        Create("Cabbage", 12, PlantKind.Vegetable, 2021, 11, 15, true),
    ];

    private static Plant Create(string name, int count, PlantKind kind, int year, int month, int day, bool watered) =>
        new(name, count, kind, new DateTimeOffset(year, month, day, 0, 0, 0, TimeSpan.Zero), watered);
}
