using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIRGA.Application.Interfaces.Services
{
    public interface IEmailGeneratorService
    {
        string GenerateEstudianteEmail(string matricula);
        string GenerateProfesorEmail(string firstName, string lastName);
    }
}
