﻿using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IRepositories
{
    public interface ICardRepo : IGenericRepo<Card>
    {
        public Task<List<Card>> GetCardsByBoardId(int boardId);

    }
}
