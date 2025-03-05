using StudentPath.DAL.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.DAL.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IUserRepo User { get; }
        public Task Save();

    }
}
