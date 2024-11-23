using System;
using System.Collections.Generic;

namespace CUOIKY.Repository.Models;

public partial class User
{
    public int UserId { get; set; }

    public int? RoleId { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? UserNameString { get; set; }

    public decimal? Balance { get; set; }

    public string? Gmail { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual Role? Role { get; set; }
}
