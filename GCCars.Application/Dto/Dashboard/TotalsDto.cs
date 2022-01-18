namespace GCCars.Application.Dto.Dashboard
{
    /// <summary>
    /// Итоговая информация о продажах.
    /// </summary>
    public class TotalsDto
    {
        /// <summary>
        /// Общая стоимость всех выставленных на продажу машинок.
        /// </summary>
        public decimal CurrentCost { get; set; }

        /// <summary>
        /// общая стоимость всех уже проданных машинок.
        /// </summary>
        public decimal TotalSales { get; set; }

        public decimal TotalValue => (CurrentCost + TotalSales) * 3;
    }
}
