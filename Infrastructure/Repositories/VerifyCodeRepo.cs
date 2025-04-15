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
    public class VerifyCodeRepo : GenericRepo<VerifyCode>, IVerifyCodeRepo
    {
        public VerifyCodeRepo(ApiContext context) : base(context)
        {
        }
    }
}
