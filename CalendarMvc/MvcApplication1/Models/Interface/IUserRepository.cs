using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcApplication1.Models.Interface
{
    public interface IUserRepository
    {
        /// <summary>
        /// User register
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        UserInfo Register(UserInfo userInfo);

        /// <summary>
        /// user login
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        UserInfo Login(UserInfo userInfo);
    }
}
