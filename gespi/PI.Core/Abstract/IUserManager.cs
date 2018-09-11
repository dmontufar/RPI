using PI.Common;
using PI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core.Abstract
{
    public interface IUserManager : IManager<Usuario>
    {
        ResultInfo Find(string userAccount, string password);
        ResultInfo SignIn(Usuario usuario);
        ResultInfo SetPassword(Usuario usuario);
    }

    public interface IUsuarioPublicoManager : IManager<UsuarioPublico>
    {
        ResultInfo Find(string userAccount, string password);
        ResultInfo SignIn(UsuarioPublico usuario);
        ResultInfo SetPassword(UsuarioPublico usuario);
    }


    //public interface IUserManager
    //{
    //    ResultInfo Create(Usuario usuario);
    //    ResultInfo Update(Usuario usuario);
    //    ResultInfo ChangePassword(string userId, string currentPassword, string newPassword);
    //    ResultInfo AddToRole(string userId, string role);
    //    ResultInfo Find(string userAccount, string password);
    //    ResultInfo SignIn(Usuario usuario);
    //}
}
