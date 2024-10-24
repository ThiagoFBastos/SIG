using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities.Abstract;

namespace Domain.Entities
{
    [Table("professores")]
    public class Professor: Funcionario
    {
        public List<Turma> Turmas { get; } = [];
    }
}