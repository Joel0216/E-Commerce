using Domain.Entities;
using Infrastructure.Data;

namespace WebApi.Seeders;

public static class DbSeeder
{
    public static void SeedDatabase(AppDbContext context)
    {
        // Evita ejecutar si ya hay datos
        if (context.Products.Any() || context.Orders.Any() || context.OrderItems.Any())
            return;

        var rnd = new Random();

        string[] categorias = { "Artesanal", "Orgánico", "Tradicional", "Premium", "Natural", "Gourmet", "Rústico", "Ecológico" };
        string[] productos = { "Café", "Mezcal", "Miel", "Camisa", "Sombrero", "Chocolate", "Aretes", "Pulsera", "Taza", "Jabón", "Pan", "Queso" };
        string[] origenes = { "de Oaxaca", "de Yucatán", "de Chiapas", "del Bajío", "de Guerrero", "de Puebla", "de Michoacán", "de Veracruz" };

        var usedNames = new HashSet<string>();
        var productList = new List<Product>();

        while (productList.Count < 100)
        {
            string nombre = $"{categorias[rnd.Next(categorias.Length)]} {productos[rnd.Next(productos.Length)]} {origenes[rnd.Next(origenes.Length)]}";

            if (usedNames.Contains(nombre)) continue;

            usedNames.Add(nombre);
            productList.Add(new Product
            {
                Name = nombre,
                Price = Math.Round((decimal)(rnd.NextDouble() * 900 + 100), 2),
                Stock = 40
            });
        }

        context.Products.AddRange(productList);
        context.SaveChanges();

        // Recuperar productos ya con IDs
        var products = context.Products.ToList();

        // Crear órdenes
        var orders = new List<Order>();
        for (int i = 1; i <= 25; i++)
        {
            var status = i <= 9 ? Domain.Enums.OrderStatus.Pending :
                         (i <= 17 ? Domain.Enums.OrderStatus.Paid : Domain.Enums.OrderStatus.Shipped);

            orders.Add(new Order { Status = status });
        }

        context.Orders.AddRange(orders);
        context.SaveChanges();

        // Crear items de órdenes
        var orderItems = new List<OrderItem>();
        int totalItems = 0;

        foreach (var order in orders)
        {
            int itemCount = rnd.Next(2, 5);
            var usedProductIds = new HashSet<int>();

            for (int j = 0; j < itemCount && totalItems < 100; j++)
            {
                Product product;
                int productId;

                do
                {
                    product = products[rnd.Next(products.Count)];
                    productId = product.Id;
                } while (usedProductIds.Contains(productId));

                usedProductIds.Add(productId);

                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = rnd.Next(1, 6),
                    UnitPrice = product.Price
                });

                totalItems++;
            }

            if (totalItems >= 100) break;
        }

        context.OrderItems.AddRange(orderItems);
        context.SaveChanges();
    }
}
