using System;
using System.Collections.Generic;

namespace CUOIKY.Repository.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public decimal? TotalPrice { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
