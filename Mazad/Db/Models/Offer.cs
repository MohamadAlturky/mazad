﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mazad.Core.Domain.Regions;
using Mazad.Core.Shared.Entities;

namespace Mazad.Models;

public class Offer : BaseEntity<int>
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string MainImageUrl { get; set; } = string.Empty;
    public List<OfferImage> ImagesUrl { get; set; } = [];
    public int CategoryId { get; set; }
    public int ProviderId { get; set; }
    public User Provider { get; set; }
    public double Price { get; set; }

    public Category Category { get; set; } = new();
    public List<OfferComment> Comments { get; set; } = [];

    public int RegionId { get; set; }
    public Region Region { get; set; } = new();
    public List<Favorite> Favorites { get; set; } = [];
    public int NumberOfViews { get; set; } = 0;
}

public class OfferImage : BaseEntity<int>
{
    public int OfferId { get; set; }
    public Offer Offer { get; set; } = new();

    [MaxLength(2000)]
    public string ImageUrl { get; set; } = string.Empty;
}

public class OfferComment : BaseEntity<int>
{
    public int OfferId { get; set; }
    public Offer Offer { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    [MaxLength(5000)]
    public string Comment { get; set; } = string.Empty;

    public int? ReplyToCommentId { get; set; }
    public OfferComment? ReplyToComment { get; set; }
    public List<OfferComment> ChildrenComments { get; set; } = [];
}
