using System;
using System.Collections.Generic;

namespace CUOIKY.Repository.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public int? DescriptionId { get; set; }

    public string ImageName { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? DeletedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Description? Description { get; set; }
}
