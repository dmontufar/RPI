﻿using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess;
using PI.DataAccess.Abstract;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public interface IOpcionesPorEventoManager : IManager<OpcionesPorEvento>{}

    public class OpcionesPorEventoManager : Manager<OpcionesPorEvento>, IOpcionesPorEventoManager
    {
        public OpcionesPorEventoManager(IOpcionesPorEventoRepository repository, ITransaction transaction) : base(repository, transaction) { }
    }
}
