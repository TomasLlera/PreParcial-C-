using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        // Calcula el subtotal de este producto
        public decimal Subtotal => Price * Quantity;

        public Product() { }

        public Product(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Name} - ${Price} x {Quantity} = ${Subtotal}";
        }
    }
}