using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WPF.PresentationLayer.Models;

public class GoodsIssueItemInput : INotifyPropertyChanged
{
    private Guid _productId;
    private string _productName = string.Empty;
    private decimal _qtyIssued;
    private decimal _unitPrice;
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
    public decimal QtyIssued
    {
        get => _qtyIssued;
        set { if (_qtyIssued != value) { _qtyIssued = value; OnPropertyChanged(); OnPropertyChanged(nameof(LineTotal)); } }
    }
    public decimal UnitPrice
    {
        get => _unitPrice;
        set { if (_unitPrice != value) { _unitPrice = value; OnPropertyChanged(); OnPropertyChanged(nameof(LineTotal)); } }
    }
    public string? Notes
    {
        get => _notes;
        set { if (_notes != value) { _notes = value; OnPropertyChanged(); } }
    }

    private decimal _qtyOrdered;
    public decimal QtyOrdered
    {
        get => _qtyOrdered;
        set { if (_qtyOrdered != value) { _qtyOrdered = value; OnPropertyChanged(); } }
    }

    public decimal LineTotal => QtyIssued * UnitPrice;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
