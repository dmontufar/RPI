using PI.Common;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.DataAccess.Abstract
{
    public interface IMarcaRepository : IRepository<Marca>
    {
        /// <summary>
        /// Search Titulares 
        /// </summary>
        PagedList searchTitular(string textToSearch);
       
    }
}
