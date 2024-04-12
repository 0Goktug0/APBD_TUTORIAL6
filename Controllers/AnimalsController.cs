using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Tutorial5.Models;
using Tutorial5.Models.DTOs;
using Tutorial5.Services;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Tutorial5.Controllers;

[ApiController]
// [Route("api/animals")]
[Route("api/[controller]")]

public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    
    [HttpGet]
    public IActionResult GetAnimals([FromQuery] string orderBy = "name")
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
    
        string sqlQuery = "SELECT * FROM Animal";
        
        switch (orderBy.ToLower())
        {
            case "name":
                sqlQuery += " ORDER BY Name ASC";
                break;
            case "description":
                sqlQuery += " ORDER BY Description ASC";
                break;
            case "category":
                sqlQuery += " ORDER BY Category ASC";
                break;
            case "area":
                sqlQuery += " ORDER BY Area ASC";
                break;
            default:
                sqlQuery += " ORDER BY Name ASC";
                break;
        }

        using SqlCommand command = new SqlCommand(sqlQuery, connection);
        var reader = command.ExecuteReader();

        var animals = new List<Animal>();

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(reader.GetOrdinal("IdAnimal")),
                Name = reader.GetString(reader.GetOrdinal("Name")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Area = reader.GetString(reader.GetOrdinal("Area")),
            });
        }

        return Ok(animals);
    }


    
    [HttpPut("{IdAnimal}")]
    public IActionResult UpdateAnimals([FromRoute] int IdAnimal, [FromBody] UpdateAnimalDto updateAnimalDto)
    {
        // Open connection
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
    
        // Create command
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = @"
        UPDATE Animal SET 
        Name = @Name, 
        Description = @Description, 
        Category = @Category, 
        Area = @Area
        WHERE IdAnimal = @IdAnimal";
        
        command.Parameters.AddWithValue("@IdAnimal", IdAnimal);
        command.Parameters.AddWithValue("@Name", updateAnimalDto.Name);
        command.Parameters.AddWithValue("@Description", (object)updateAnimalDto.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("@Category", (object)updateAnimalDto.Category ?? DBNull.Value);
        command.Parameters.AddWithValue("@Area", (object)updateAnimalDto.Area ?? DBNull.Value);
        
        var result = command.ExecuteNonQuery();
        
        if (result > 0)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }

    
    [HttpDelete("{IdAnimal}")]
    public IActionResult DeleteAnimals([FromRoute] int IdAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
        command.Parameters.AddWithValue("@IdAnimal", IdAnimal);
        
        var result = command.ExecuteNonQuery();
        
        if (result > 0)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }


    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName,'','','')";
        command.Parameters.AddWithValue("@animalName", animal.Name);
        
        command.ExecuteNonQuery();

        return Created("", null);
    }
}