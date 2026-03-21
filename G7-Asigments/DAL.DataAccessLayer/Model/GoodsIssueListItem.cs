using System;
using DAL.DataAccessLayer.Model;

namespace WPF.PresentationLayer.Models;

public class GoodsIssueListItem
{
    public GoodsIssue GoodsIssue { get; set; } = null!;
    public string GiNumber => GoodsIssue.GiNumber;
    public DateOnly IssueDate => GoodsIssue.IssueDate;
    public decimal TotalAmount => GoodsIssue.TotalAmount;
    public string? Notes => GoodsIssue.Notes;
    public int ItemCount => GoodsIssue.GoodsIssueItems?.Count ?? 0;
}