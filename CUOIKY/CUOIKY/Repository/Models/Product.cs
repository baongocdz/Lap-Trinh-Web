using System;
using System.Collections.Generic;

namespace CUOIKY.Repository.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public int? ProductTypeId { get; set; }

    public int? DescriptionId { get; set; }

    public int? Quantity { get; set; }

    public bool? StatusSell { get; set; }

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public string? Gmail { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Description? Description { get; set; }

    public virtual ICollection<Description> Descriptions { get; set; } = new List<Description>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ProductType? ProductType { get; set; }
}
