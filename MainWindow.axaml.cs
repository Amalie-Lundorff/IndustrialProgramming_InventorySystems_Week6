using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;

namespace InventorySystems_Week6;

//Partial: KObler til XAML-delen med samme klasse
    //Arver fra Window-filen 
public partial class MainWindow : Window
{ 
    //Simple domænemodeller
    public class Item
    {
        public string Name { get; set; } = "";
        public decimal PricePerUnit { get; set; }
        public override string ToString() => $"{Name}: {PricePerUnit} per unit";
    }
//Simpel vare: get; set; gør det nemt at ændre 
//ToString(): Er praktisk til visning 

//Ordre linjen - En linje i en ordre - Referer til Item + mængde
    public class OrderLine
    {
        public Item Item { get; set; } = new();
        public decimal Quantity { get; set; }
        public decimal LineTotal => Item.PricePerUnit * Quantity;
    }
//LineTotal: Er en beregnet størrelse 
    

//E ordre har kunde, tidspunkt og en samling af linjer 
    public class Order
    {
        public string Customer { get; set; } = "";
        public DateTime Time { get; set; } = DateTime.Now;
        public ObservableCollection<OrderLine> Lines { get; set; } = new();
        public decimal TotalPrice => Calc();

        private decimal Calc()
        {
            decimal sum = 0;
            foreach (var l in Lines) sum += l.LineTotal;
            return sum;
        }
    }
//ObservableCollection gør det let at udvide senere - Idet man kan opdatere linjerne 
//TotalPrice: Beregnes ud fra linjerne der vælges

// Data som GUI'en observerer 
    public ObservableCollection<Order> QueuedOrders { get; } = new();
    public ObservableCollection<Order> ProcessedOrders { get; } = new();
    public decimal TotalRevenue { get; private set; }

//Constructor    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;

        // Demo-data (to items, to orders i kø)
        var flour = new Item { Name = "Flour", PricePerUnit = 12m };
        var sugar = new Item { Name = "Sugar", PricePerUnit = 10.5m };

        QueuedOrders.Add(new Order
        {
            Customer = "Alice",
            Lines =
            {
                new OrderLine { Item = flour, Quantity = 5 },
                new OrderLine { Item = sugar, Quantity = 3 }
            }
        });

        QueuedOrders.Add(new Order
        {
            Customer = "Bob",
            Lines =
            {
                new OrderLine { Item = sugar, Quantity = 6 }
            }
        });
    }
//InitializeComponent(): Nødvendig for at XAML'en kan vises 
//DataContext = this: Gør at {Binding QueuedOrders} virker
// Jeg har lavet det sådan, at jeg fylder køen med to testordre 

//Implementere first-in-first-out     
    public void ProcessNext_OnClick(object? sender, RoutedEventArgs e)
    {
        if (QueuedOrders.Count == 0) return;

        var o = QueuedOrders[0];
        QueuedOrders.RemoveAt(0);
        ProcessedOrders.Add(o);
        TotalRevenue += o.TotalPrice;

        // Refresh bindings (nemt uden INotifyPropertyChanged)
        DataContext = null;
        DataContext = this;
    }
}
//Flytter ordren fra den ene ObservableCollection til den anden - Dertil opdateres TotalRevenue 
