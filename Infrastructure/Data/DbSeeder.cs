using Domain.Entities;
using Infrastructure.Data;

namespace WebApi.Seeders;

public static class DbSeeder
{
    public static void SeedDatabase(AppDbContext context)
    {
        if (context.Products.Any() || context.Orders.Any() || context.OrderItems.Any())
            return;

        var rnd = new Random();

        // 🔢 Listas para combinar nombres únicos
        string[] categorias = { "Artesanal", "Orgánico", "Tradicional", "Premium", "Natural", "Gourmet", "Rústico", "Ecológico" };
        string[] productos = { "Café", "Mezcal", "Miel", "Camisa", "Sombrero", "Chocolate", "Aretes", "Pulsera", "Taza", "Jabón", "Pan", "Queso" };
        string[] origenes = { "de Oaxaca", "de Yucatán", "de Chiapas", "del Bajío", "de Guerrero", "de Puebla", "de Michoacán", "de Veracruz" };

        var products = new List<Product>();
        var usedNames = new HashSet<string>();

        while (products.Count < 100)
        {
            string nombre = $"{categorias[rnd.Next(categorias.Length)]} {productos[rnd.Next(productos.Length)]} {origenes[rnd.Next(origenes.Length)]}";

            if (usedNames.Contains(nombre)) continue;

            usedNames.Add(nombre);
            products.Add(new Product
            {
                Name = nombre,
                Price = Math.Round((decimal)(rnd.NextDouble() * 900 + 100), 2),
                Stock = rnd.Next(5, 30)
            });
        }

        context.Products.AddRange(products);
        context.SaveChanges();

        // 🟡 25 Órdenes
        var orders = new List<Order>();
        for (int i = 1; i <= 25; i++)
        {
            var status = i <= 9 ? Domain.Enums.OrderStatus.Pendiente :
                        (i <= 17 ? Domain.Enums.OrderStatus.Pagado : Domain.Enums.OrderStatus.Enviado);

            orders.Add(new Order { Status = status });
        }
        context.Orders.AddRange(orders);
        context.SaveChanges();

        // 🔵 100 OrderItems
        var orderItems = new List<OrderItem>();
        int totalItems = 0;

        foreach (var order in orders)
        {
            int itemCount = rnd.Next(2, 5);
            var usedProductIds = new HashSet<int>();

            for (int j = 0; j < itemCount && totalItems < 100; j++)
            {
                int productId;
                do
                {
                    productId = products[rnd.Next(products.Count)].Id;
                } while (usedProductIds.Contains(productId));

                usedProductIds.Add(productId);

                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = rnd.Next(1, 6)
                });

                totalItems++;
            }

            if (totalItems >= 100) break;
        }

        context.OrderItems.AddRange(orderItems);
        context.SaveChanges();
    }
}
