using System.ComponentModel.DataAnnotations;

namespace LUMS.Web.Models;

public class PolicyForm
{
    [Required(ErrorMessage = "Nhập số ngày mượn tối đa.")]
    [Range(1, 90, ErrorMessage = "Số ngày mượn từ 1 đến 90.")]
    [Display(Name = "Số ngày mượn tối đa")]
    public int MaxLoanDays { get; set; }

    [Required(ErrorMessage = "Nhập số sách mượn cùng lúc.")]
    [Range(1, 20, ErrorMessage = "Số sách mượn cùng lúc từ 1 đến 20.")]
    [Display(Name = "Số sách mượn cùng lúc")]
    public int MaxBooksPerReader { get; set; }

    [Required(ErrorMessage = "Nhập mức phạt mỗi ngày.")]
    [Range(0, 1_000_000, ErrorMessage = "Mức phạt từ 0 đến 1.000.000 đồng.")]
    [Display(Name = "Mức phạt / ngày (VND)")]
    public decimal FinePerDay { get; set; }
}
