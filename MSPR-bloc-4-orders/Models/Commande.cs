using System;
using System.Collections.Generic;

namespace MSPR_bloc_4_orders.Models;

public partial class Commande
{
    public int IdCommande { get; set; }

    public DateTime Createdate { get; set; }

    public int? IdClient { get; set; }
}
