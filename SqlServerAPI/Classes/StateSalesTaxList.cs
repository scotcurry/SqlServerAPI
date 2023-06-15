using System.Text.Json.Serialization;

namespace SqlServerAPI.Classes
{
    public class CompleteSalesTaxList
    {
        public List<StateSalesTax>? stateSalesTaxList { get; set; }
    }
}
