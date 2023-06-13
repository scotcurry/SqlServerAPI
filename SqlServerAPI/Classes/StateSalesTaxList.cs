using System.Text.Json.Serialization;

namespace SqlServerAPI.Classes
{
    public class CompleteSalesTaxList
    {
        public List<StateSalesTaxList>? stateSalesTaxList { get; set; }
    }
}
