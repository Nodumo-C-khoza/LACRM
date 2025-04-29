using SingularSystemsAssessment.Models;

namespace SingularSystemsAssessment.Entities
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public decimal SalePrice { get; set; }
        public int SaleQty { get; set; }
        public DateTime SaleDate { get; set; }
    }
}

