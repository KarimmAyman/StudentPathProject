using AutoMapper;
using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Users;
using StudentPath.DAL.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.UserServices
{
    public class UserService : IUserService
    {

        #region Prop
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;



        #endregion


        #region ctor
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        #endregion


        #region CreateUser
        public async Task<ApiResponse> CreateUserAsync(UserAddDTO User)
        {
            if (User == null) return ApiResponse.ErrorResponse("User is null", 400);

          
            var existingUserNum = await unitOfWork.User.GetFirstOrDefaultAsync(x => x.PhoneNumber == User.PhoneNumber);
            if (existingUserNum != null)
            {
                // PhoneNumber already exists, return conflict response
                return ApiResponse.ErrorResponse("PhoneNumber already exists. Please use a unique Number.", 409); // 409 Conflict            }
            }

            var obj = mapper.Map<StudentPath.DAL.Data.Models.User>(User);
            await unitOfWork.User.CreateOrUpdateAsync(obj);
            await unitOfWork.Save();
            return ApiResponse.SuccessResponse("User created successfully", 201);



        }
        #endregion


        #region UpdateUser
        public async Task<ApiResponse> UpdateUserAsync(UserUpdatedDTO UserDto)
        {
            if (UserDto == null)
            {
                return ApiResponse.ErrorResponse("Invalid User data", 400);

            }


            var std = await unitOfWork.User.GetFirstOrDefaultAsync(x => x.Id == UserDto.Id);
            if (std == null)
            {
                return ApiResponse.ErrorResponse("User not found", 404);

            }
            var existingUserNum = await unitOfWork.User.GetFirstOrDefaultAsync(x => x.PhoneNumber == UserDto.PhoneNumber);
            if (existingUserNum != null && existingUserNum.Id != UserDto.Id)
            {
                // PhoneNumber already exists, return conflict response
                return ApiResponse.ErrorResponse("PhoneNumber already exists. Please use a unique Number.", 409); // 409 Conflict            }
            }

            mapper.Map(UserDto, std);


            await unitOfWork.User.CreateOrUpdateAsync(std);
            await unitOfWork.Save();
            return ApiResponse.SuccessResponse("User Is Updated Successfully", 204); // 204 NoContent


        }
        #endregion


        #region GetUserById
        public async Task<ApiResponse<UserReadDTO>> getUserAsync(string id)
        {
            if (id == null)
            {
                return ApiResponse<UserReadDTO>.ErrorResponse("Invalid User data", 400);
            }
            var result = await unitOfWork.User.GetFirstOrDefaultAsync(x => x.Id == id, false);

            if (result == null) return ApiResponse<UserReadDTO>.ErrorResponse("User not found", 404);

            var UserReadDto = mapper.Map<UserReadDTO>(result);
            return ApiResponse<UserReadDTO>.SuccessResponse("user retrieved successfully ", 200, UserReadDto);


        }

        public async Task<ApiResponse<UserDetailsDTO>> getUserDetilsAsync(string id)
        {
            if (id == null)
            {
                return ApiResponse<UserDetailsDTO>.ErrorResponse("Invalid User data", 400);
            }
            var result = await unitOfWork.User.GetFirstOrDefaultAsync(x => x.Id == id, false);

            if (result == null) return ApiResponse<UserDetailsDTO>.ErrorResponse("User not found", 404);

            var UserDetailDto = mapper.Map<UserDetailsDTO>(result);
            return ApiResponse<UserDetailsDTO>.SuccessResponse("user retrieved successfully", 200, UserDetailDto);
        }

        #endregion

       

        #region GetAllUsers
        public async Task<ApiResponse<IEnumerable<UserReadDTO>>> getUsersAsync()
        {
            var result = await unitOfWork.User.GetAsync(null, orderBy: q => q.OrderBy(s => s.UserName), 1, 10, false);
            if (result == null || !result.Any())
            {
                return ApiResponse<IEnumerable<UserReadDTO>>.ErrorResponse("No Users found", 404);
            }

            var UserReadDtos = mapper.Map<IEnumerable<UserReadDTO>>(result);

            return ApiResponse<IEnumerable<UserReadDTO>>.SuccessResponse("users retrieved successfully", 200, UserReadDtos);
        }
        public Task<IEnumerable<UserReadDTO>> getUsersAsync(int page, int pagesize)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region DeleteUser
        public async Task<ApiResponse> SoftDeleteUserAsync(string id)
        {
            if (id == null)
            {
                return ApiResponse.ErrorResponse("Invalid User data", 400);
            }

            var std = await unitOfWork.User.GetFirstOrDefaultAsync(x => x.Id == id);
            if (std == null) return ApiResponse.ErrorResponse("User not found", 404);

            await unitOfWork.User.SoftDeleteAsync(std);
            await unitOfWork.Save();
            return ApiResponse.SuccessResponse("User Is Soft Deleted", 200);


        }
        #endregion



    }
}
