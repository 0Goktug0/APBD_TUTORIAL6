using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tutorial5.Models;
using Tutorial5.Models.DTOs;
using Tutorial5.Services;

namespace Tutorial5.Services
{
    public interface IConfiguration
    {
        List<Animal> GetAnimals(string orderBy);
        void DeleteAnimals(int IdAnimal);
    }
}