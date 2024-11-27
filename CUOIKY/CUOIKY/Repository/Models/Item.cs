using System;
using System.Collections.Generic;

namespace CUOIKY.Repository.Models;

public partial class Item
{
    public int ItemId { get; set; }

    public string? ItemName { get; set; }

    public virtual ICollection<Description> Descriptions { get; set; } = new List<Description>();
}
