﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;

namespace Discount.Grpc.Extensions
{
  public static class HostExtensions
  {
    public static IHost MigrateDatabase<TContext>(this IHost host, int? retryAttempts = 0)
    {
      int retryForAvailbalilityAttempts = retryAttempts.Value;

      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        var configuration = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<TContext>>();

        try
        {
          logger.LogInformation("Migrating postgres db.");
          var connection = new NpgsqlConnection(configuration.GetValue<string>("NpgConnectionString"));
          connection.Open();

          using (var command = connection.CreateCommand())
          {
            command.CommandText = "DROP TABLE IF EXISTS Coupon";
            command.ExecuteNonQuery();

            command.CommandText = @"CREATE TABLE Coupon(Id SERIAL PRIMARY KEY, 
                                                                ProductName VARCHAR(24) NOT NULL,
                                                                Description TEXT,
                                                                Amount INT)";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('IPhone X', 'IPhone Discount', 150);";
            command.ExecuteNonQuery();

            command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES('Samsung 10', 'Samsung Discount', 100);";
            command.ExecuteNonQuery();

            logger.LogInformation("Migrated postresql database.");
          }
        }
        catch (NpgsqlException ex)
        {
          logger.LogError(ex, "An error occurred while migrating the postresql database");

          if (retryForAvailbalilityAttempts < 50)
          {
            retryForAvailbalilityAttempts++;
            System.Threading.Thread.Sleep(2000);
            MigrateDatabase<TContext>(host, retryForAvailbalilityAttempts);
          }
        }
      }
      return host;
    }
  }
}
