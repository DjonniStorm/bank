using System.Xml.Linq;

namespace BankContracts.BindingModels;

public class CurrencyBindingModel
{
    public string? Id { get; set; } 

    public string? Name { get; set; } 

    public string? Abbreviation { get; set; } 

    public decimal Cost { get; set; } 

    public string? StorekeeperId { get;  set; } 
}
