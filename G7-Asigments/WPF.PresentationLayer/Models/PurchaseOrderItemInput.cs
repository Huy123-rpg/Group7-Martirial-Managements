using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF.PresentationLayer.Models;

public class PurchaseOrderItemInput : INotifyPropertyChanged
{
    private Guid _productId;
    private string _productName = string.Empty;
    private decimal _qtyOrdered;
    private decimal _unitCost;
    private string? _notes;

    public Guid ProductId 
    { 
        get => _productId; 
        set { if (_productId != value) { _productId = value; OnPropertyChanged(); } } 
    }
    public string ProductName 
    { 
        get => _productName; 
        set { if (_productName != value) { _productName = value; OnPropertyChanged(); } } 
    }
    public decimal QtyOrdered 
    { 
        get => _qtyOrdered; 
        set { if (_qtyOrdered != value) { _qtyOrdered = value; OnPropertyChanged(); OnPropertyChanged(nameof(LineTotal)); } } 
    }
    public decimal UnitCost 
    { 
        get => _unitCost; 
        set { if (_unitCost != value) { _unitCost = value; OnPropertyChanged(); OnPropertyChanged(nameof(LineTotal)); } } 
    }
    public string? Notes 
    { 
        get => _notes; 
        set { if (_notes != value) { _notes = value; OnPropertyChanged(); } } 
    }

    public decimal LineTotal => QtyOrdered * UnitCost;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
