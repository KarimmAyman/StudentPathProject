using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.UserServices
{
    public interface IUserService
    {
        public Task<ApiResponse<IEnumerable<UserReadDTO>>> getUsersAsync();
        public Task<IEnumerable<UserReadDTO>> getUsersAsync(int page, int pagesize);
        public Task<ApiResponse> UpdateUserAsync(UserUpdatedDTO userDto);
        public Task<ApiResponse<UserDetailsDTO>> getUserDetilsAsync(string id);
        public Task<ApiResponse<UserReadDTO>> getUserAsync(string id);


        public Task<ApiResponse> SoftDeleteUserAsync(string id);

        public Task<ApiResponse> CreateUserAsync(UserAddDTO user);


    }
}
