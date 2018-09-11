using PI.Common;
using PI.Core.Abstract;
using PI.DataAccess.Abstract;
using PI.Models;
using PI.Models.Composite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PI.Core
{
    public class UserManager : Manager<Usuario>, IUserManager
    {
        private ICryptoManager _cryptoManager;

        public UserManager(IUserRepository repository, ICryptoManager cryptoManager, ITransaction transaction) : base(repository, transaction)
        {
            _cryptoManager = cryptoManager;
        }

        public ResultInfo Find(string userAccount, string password)
        {
            var resultInfo = new ResultInfo();
            resultInfo.Succeeded = false;
            resultInfo.Result = Repository.Get(u => u.Email == userAccount);
            var usuario = (Usuario)resultInfo.Result;
            if (usuario != null)
                resultInfo.Succeeded = (usuario.Password == _cryptoManager.Encrypt(password));

            usuario.Password = "(O)^(_)";//
            if (resultInfo.Succeeded)
            {
                resultInfo.Result = new UserSettings()
                {
                    Usuario = usuario,
                    //UserRoleOptions = UserMenu.GetMenu(usuario.Role)
                };
            }
            else
            {
                resultInfo.Result = new UserSettings();
            }

            return resultInfo;
        }

        public ResultInfo SignIn(Usuario usuario)
        {
            usuario.Password = _cryptoManager.Encrypt(usuario.Password);
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                usuario = _repository.Add(usuario);
                return new ResultInfo() { Succeeded = true, Result = usuario };
            });
        }

        public ResultInfo SetPassword(Usuario usuario)
        {
            var dbUser = _repository.GetById(usuario.Id);
            dbUser.Password = _cryptoManager.Encrypt(usuario.Password);

            if (dbUser.Spk == usuario.Spk && dbUser.SpkExpiration > DateTime.Now)
            {
                return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
                {
                    usuario = _repository.Update(dbUser);
                    return new ResultInfo() { Succeeded = true, Result = usuario };
                });
            }
            return new ResultInfo() { Succeeded = false };
        }
    }


    /// <summary>
    /// MANAGER DE USUARIOS PUBLICOS
    /// </summary>
    public class UsuarioPublicoManager : Manager<UsuarioPublico>, IUsuarioPublicoManager
    {
        private ICryptoManager _cryptoManager;

        public UsuarioPublicoManager(IUsuarioPublicoRepository repository, ICryptoManager cryptoManager, ITransaction transaction)
            : base(repository, transaction)
        {
            _cryptoManager = cryptoManager;
        }

        /// <summary>
        /// Log-in
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ResultInfo Find(string userAccount, string password)
        {
            var resultInfo = new ResultInfo();
            resultInfo.Succeeded = false;
            resultInfo.Result = Repository.Get(u => u.Cuenta == userAccount);
            var usuario = (UsuarioPublico)resultInfo.Result;
            if (usuario != null)
            {
                resultInfo.Succeeded = (usuario.Pwd == _cryptoManager.Encrypt(password));
                usuario.Pwd = "(O)^(_)";//
            }

            if (resultInfo.Succeeded)
                resultInfo.Result = usuario;
            else
                resultInfo.Result = new UsuarioPublico();

            return resultInfo;
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <param name="usuario"></param>
        /// <returns></returns>
        public ResultInfo SignIn(UsuarioPublico usuario)
        {
            usuario.Pwd = _cryptoManager.Encrypt(usuario.Pwd);
            return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
            {
                usuario = _repository.Add(usuario);
                return new ResultInfo() { Succeeded = true, Result = usuario };
            });
        }

        public ResultInfo SetPassword(UsuarioPublico usuario)
        {
            var dbUser = _repository.GetById(usuario.Id);
            dbUser.Pwd = _cryptoManager.Encrypt(usuario.Pwd);

            if (dbUser.Spk == usuario.Spk && dbUser.SpkExpiration > DateTime.Now)
            {
                return _transaction.ExecuteInTransactionScope<ResultInfo>(() =>
                {
                    usuario = _repository.Update(dbUser);
                    return new ResultInfo() { Succeeded = true, Result = usuario };
                });
            }
            return new ResultInfo() { Succeeded = false };
        }
    }
}
