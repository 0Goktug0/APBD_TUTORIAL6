using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Tutorial5.Models;
using Tutorial5.Models.DTOs;

namespace Tutorial5.Services
{
    public class Configuration : IConfiguration
    {
        private string _connectionString;

        public Configuration(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Default");
        }

        public List<Animal> GetAnimals(string orderBy)
        {
            var animals = new List<Animal>();
            
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand();

            cmd.Connection = connection;
            connection.Open();

            orderBy = orderBy.ToLower();

            if (orderBy != "name" && orderBy != "description" && orderBy != "category" && orderBy != "area")
            {
                orderBy = "name";
            }
                
                cmd.CommandText = $"SELECT * FROM Animal ORDER BY {orderBy};";

                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    animals.Add(new Animal()
                    {
                        IdAnimal = reader.GetInt32("IdAnimal"),
                        Name = reader["Name"].ToString(),
                        Description = reader["Description"].ToString(),
                        Category = reader["Category"].ToString(),
                        Area = reader["Area"].ToString()
                    });
                }
            return animals;
        }

        public void DeleteAnimals(int IdAnimal)
        {
            SqlConnection connection = new SqlConnection(_connectionString);

            string sqlStatement = "DELETE FROM Animal WHERE IdAnimal = " + IdAnimal;

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sqlStatement, connection);
                cmd.Parameters.AddWithValue("@IdAnimal", "SomeValueHere");
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();

            }
            finally
            {
                connection.Close();
            }
        }

    }
}
