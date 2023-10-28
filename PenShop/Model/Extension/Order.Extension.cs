namespace PenShop.Model
{
    partial class Order
    {
        private decimal _allPrice;
        public decimal AllPrice
        {
            get
            {
                _allPrice = Pen.Price * Count;

                return _allPrice;
            }
        }
    }
}
