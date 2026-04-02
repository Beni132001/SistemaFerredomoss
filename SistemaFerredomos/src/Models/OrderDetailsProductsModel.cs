using SistemaFerredomos.src.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public class OrderDetailsProductsModel : INotifyPropertyChanged
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public OrdersModel Orders { get; set; }
    public ProductsModel Products { get; set; }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Subtotal));
        }
    }

    public decimal Subtotal => Quantity * UnitPrice;

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}