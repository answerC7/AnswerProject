using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MvcApplication1.Models.Interface;

namespace MvcApplication1.Models
{
    public class UserRepository : IUserRepository
    {
        private  readonly CalendarDBEntities _dbcontext=new CalendarDBEntities();

        public UserInfo Register(UserInfo userInfo)
        {
            
            
                if (userInfo != null)
                {
                    UserInfo userModel = _dbcontext.UserInfo.Add(userInfo);
                    _dbcontext.SaveChanges();
                    return userModel;
                }
                return null;
        }

        public UserInfo Login(UserInfo userInfo)
        {          
                if (userInfo != null)
                {
                    IQueryable<UserInfo> userInfos = _dbcontext.UserInfo.Where(
                        x => x.UserName == userInfo.UserName && x.UserPassword == userInfo.UserPassword);

                    if (userInfos.Any())
                    {
                        return userInfos.First();
                    }
            }
            return null;
        }
    }
}