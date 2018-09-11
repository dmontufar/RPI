using PI.Common;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface IMarcaManager : IManager<Marca>
    {
        /// <summary>
        /// Search Titulares 
        /// </summary>
        ResultInfo searchTitular(string textToSearch);

    }

}
