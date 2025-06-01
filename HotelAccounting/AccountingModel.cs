using System;

namespace HotelAccounting
{
    public class AccountingModel : ModelBase
    {
        private double price;
        private int nightsCount;
        private double discount;
        private double total;
        private bool isUpdating;

        public double Price
        {
            get => price;
            set
            {
                if (value < 0) throw new ArgumentException("Price cannot be negative");
                if (Math.Abs(price - value) < double.Epsilon) return;
                
                price = value;
                UpdateTotalFromProperties();
                Notify(nameof(Price));
            }
        }

        public int NightsCount
        {
            get => nightsCount;
            set
            {
                if (value < 0) throw new ArgumentException("Nights count cannot be negative");
                if (nightsCount == value) return;
                
                nightsCount = value;
                UpdateTotalFromProperties();
                Notify(nameof(NightsCount));
            }
        }

        public double Discount
        {
            get => discount;
            set
            {
                if (Math.Abs(discount - value) < double.Epsilon) return;
                
                discount = value;
                UpdateTotalFromProperties();
                Notify(nameof(Discount));
            }
        }

        public double Total
        {
            get => total;
            set
            {
                if (value < 0) throw new ArgumentException("Total cannot be negative");
                if (Math.Abs(total - value) < double.Epsilon) return;
                if (price <= 0 || nightsCount <= 0) 
                    throw new InvalidOperationException("Price and NightsCount must be positive");
                
                total = value;
                UpdateDiscountFromTotal();
                Notify(nameof(Total));
            }
        }

        private void UpdateTotalFromProperties()
        {
            if (isUpdating) return;
            isUpdating = true;
            
            Total = price * nightsCount * (1 - discount / 100);
            
            isUpdating = false;
        }

        private void UpdateDiscountFromTotal()
        {
            if (isUpdating) return;
            isUpdating = true;
            
            var newDiscount = 100 * (1 - total / (price * nightsCount));
            discount = newDiscount;
            Notify(nameof(Discount));
            
            isUpdating = false;
        }
    }
}