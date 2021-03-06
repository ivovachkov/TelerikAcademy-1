﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Wintellect.PowerCollections;

class Product : IComparable<Product>
{
    public string Name { get; private set; }
    public double Price { get; private set; } // TODO
    public string Producer { get; private set; }

    public Product(string name, double price, string producer)
    {
        this.Name = name;
        this.Price = price;
        this.Producer = producer;
    }

    public int CompareTo(Product other)
    {
        int result = 0;

        result = this.Name.CompareTo(other.Name);
        if (result != 0) return result;

        result = this.Price.CompareTo(other.Price);
        if (result != 0) return result;

        result = this.Producer.CompareTo(other.Producer);
        if (result != 0) return result;

        return result;
    }

    public override string ToString()
    {
        return "{" + string.Join(";",
            this.Name, this.Producer, this.Price.ToString("0.00")
        ) + "}";
    }
}

class Program
{
    static Dictionary<string, Func<string[], string>> commands =
        new Dictionary<string, Func<string[], string>>()
    {
        { "AddProduct", AddProduct },
        { "DeleteProducts", DeleteProducts },
        { "FindProductsByName", FindProductsByName },
        { "FindProductsByProducer", FindProductsByProducer },
        { "FindProductsByPriceRange", FindProductsByPriceRange },
    };

    static MultiDictionary<string, Product> productsByName =
        new MultiDictionary<string, Product>(true);

    static MultiDictionary<string, Product> productsByProducer =
        new MultiDictionary<string, Product>(true);

    static MultiDictionary<Tuple<string, string>, Product> productsByNameAndProducer =
        new MultiDictionary<Tuple<string, string>, Product>(true);

    static OrderedMultiDictionary<double, Product> productsByPrice =
        new OrderedMultiDictionary<double, Product>(true);

    static string AddProduct(string[] parameters)
    {
        var product = new Product(
            parameters[0],
            double.Parse(parameters[1]),
            parameters[2]
        );

        productsByName.Add(product.Name, product);

        productsByProducer.Add(product.Producer, product);

        productsByNameAndProducer.Add(
            new Tuple<string, string>(product.Name, product.Producer),
            product
        );

        productsByPrice.Add(product.Price, product);

        return "Product added";
    }

    static string DeleteProducts(string[] parameters)
    {
        int result = 0;

        if (parameters.Length == 1)
        {
            var matchedProducts = productsByProducer[parameters[0]];

            foreach (var product in matchedProducts)
            {
                productsByName.Remove(product.Name, product);
                productsByNameAndProducer.Remove(
                    new Tuple<string, string>(product.Name, product.Producer),
                    product
                );
                productsByPrice.Remove(product.Price, product);
            }

            result = matchedProducts.Count;
            productsByProducer.Remove(parameters[0]);
        }

        else if (parameters.Length == 2)
        {
            var nameAndProducer = new Tuple<string, string>(parameters[0], parameters[1]);

            var matchedProducts = productsByNameAndProducer[nameAndProducer];

            foreach (var product in matchedProducts)
            {
                productsByName.Remove(product.Name, product);
                productsByProducer.Remove(product.Producer, product);
                productsByPrice.Remove(product.Price, product);
            }

            result = matchedProducts.Count;
            productsByNameAndProducer.Remove(nameAndProducer);
        }

        if (result == 0)
            return "No products found";

        return string.Format("{0} products deleted", result);
    }

    static string FindProductsByName(string[] parameters)
    {
        var result = productsByName[parameters[0]].OrderBy(p => p);

        if (!result.Any())
            return "No products found";

        return string.Join(Environment.NewLine, result);
    }

    static string FindProductsByProducer(string[] parameters)
    {
        var result = productsByProducer[parameters[0]].OrderBy(p => p);

        if (!result.Any())
            return "No products found";

        return string.Join(Environment.NewLine, result);
    }

    static string FindProductsByPriceRange(string[] parameters)
    {
        var min = double.Parse(parameters[0]);
        var max = double.Parse(parameters[1]);

        var result = productsByPrice.Range(min, true, max, true).Values.OrderBy(p => p);

        if (!result.Any())
            return "No products found";

        return string.Join(Environment.NewLine, result);
    }

    static void Main()
    {
#if DEBUG
        Console.SetIn(new System.IO.StreamReader("../../input.txt"));
#endif

        var date = DateTime.Now;

        var result = string.Join(Environment.NewLine, Enumerable.Range(0, int.Parse(Console.ReadLine()))
            .Select(i => Regex.Match(Console.ReadLine(), @"^(\w+) (.+)$").Groups)
            .Select(group => commands[group[1].Value](group[2].Value.Split(';')))
        );

        //Console.WriteLine(result);

#if DEBUG
        Console.WriteLine(DateTime.Now - date); // Naive List<Product> implementation got 80/100 points.
#endif
    }
}
