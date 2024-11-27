using System;
using System.Collections.Generic;

namespace CUOIKY.Repository.Models;

public partial class Description
{
    public int DescriptionId { get; set; }

    public int ProductId { get; set; }

    public string? Description1 { get; set; }

    public int? Server { get; set; }

    public int? Planet { get; set; }

    public bool? Portal { get; set; }

    public bool? Disciple { get; set; }

    public int? ItemId { get; set; }

    public decimal? Price { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Item? Item { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
