using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDatabase.Models;

class DepositClient
{
    public required string DepositId { get; set; }

    public required string ClientId { get; set; }

    public Deposit? Deposit { get; set; }

    public Client? Client { get; set; }
}
