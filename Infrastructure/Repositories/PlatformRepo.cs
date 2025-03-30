using Application.IRepositories;
using Domain;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PlatformRepo : GenericRepo<Platform>, IPlatformRepo
    {
        public PlatformRepo(ApiContext context) : base(context)
        {
        }
    }
}
