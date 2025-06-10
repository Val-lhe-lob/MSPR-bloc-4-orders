using System;
using System.Collections.Generic;

namespace MSPR_bloc_4_orders.Models;

public partial class ProduitCommande
{
    public int IdProduitcommande { get; set; }

    public int IdCommande { get; set; }

    public int IdProduit { get; set; }

    public string? Nom { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal? Prix { get; set; }

    public string? Description { get; set; }

    public string? Color { get; set; }

    public int? Quantite { get; set; }
}
