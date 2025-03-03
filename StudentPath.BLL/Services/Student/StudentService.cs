using AutoMapper;
using StudentPath.BLL.Dtoes.Students;
using StudentPath.BLL.Dtoes;
using StudentPath.DAL.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.Student
{
    public class StudentService : IStudentService
    {

        #region Prop
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;



        #endregion


        #region ctor
        public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        #endregion


        #region CreateStudent
        public async Task<ApiResponse> CreateStudentAsync(StudentAddDTO student)
        {
            if (student == null) return ApiResponse.ErrorResponse("Student is null", 400);

          
            var existingStudentNum = await unitOfWork.Student.GetFirstOrDefaultAsync(x => x.PhoneNumber == student.PhoneNumber);
            if (existingStudentNum != null)
            {
                // PhoneNumber already exists, return conflict response
                return ApiResponse.ErrorResponse("PhoneNumber already exists. Please use a unique Number.", 409); // 409 Conflict            }
            }

            var obj = mapper.Map<StudentPath.DAL.Data.Models.Student>(student);
            await unitOfWork.Student.CreateOrUpdateAsync(obj);
            await unitOfWork.Save();
            return ApiResponse.SuccessResponse("Student created successfully", 201);



        }
        #endregion


        #region UpdateStudent
        public async Task<ApiResponse> UpdateStudentAsync(StudentUpdatedDTO studentDto)
        {
            if (studentDto == null)
            {
                return ApiResponse.ErrorResponse("Invalid student data", 400);

            }


            var std = await unitOfWork.Student.GetFirstOrDefaultAsync(x => x.Id == studentDto.Id);
            if (std == null)
            {
                return ApiResponse.ErrorResponse("Student not found", 404);

            }
            var existingStudentNum = await unitOfWork.Student.GetFirstOrDefaultAsync(x => x.PhoneNumber == studentDto.PhoneNumber);
            if (existingStudentNum != null && existingStudentNum.Id != studentDto.Id)
            {
                // PhoneNumber already exists, return conflict response
                return ApiResponse.ErrorResponse("PhoneNumber already exists. Please use a unique Number.", 409); // 409 Conflict            }
            }

            mapper.Map(studentDto, std);


            await unitOfWork.Student.CreateOrUpdateAsync(std);
            await unitOfWork.Save();
            return ApiResponse.SuccessResponse("Student Is Updated Successfully", 204); // 204 NoContent


        }
        #endregion


        #region GetStudentById
        public async Task<ApiResponse<StudentReadDTO>> getStudentAsync(string id)
        {
            if (id == null)
            {
                return ApiResponse<StudentReadDTO>.ErrorResponse("Invalid student data", 400);
            }
            var result = await unitOfWork.Student.GetFirstOrDefaultAsync(x => x.Id == id, false);

            if (result == null) return ApiResponse<StudentReadDTO>.ErrorResponse("Student not found", 404);

            var studentReadDto = mapper.Map<StudentReadDTO>(result);
            return ApiResponse<StudentReadDTO>.SuccessResponse("Student found", 200, studentReadDto);


        }

        public async Task<ApiResponse<StudentDetailsDTO>> getStudentDetilsAsync(string id)
        {
            if (id == null)
            {
                return ApiResponse<StudentDetailsDTO>.ErrorResponse("Invalid student data", 400);
            }
            var result = await unitOfWork.Student.GetFirstOrDefaultAsync(x => x.Id == id, false);

            if (result == null) return ApiResponse<StudentDetailsDTO>.ErrorResponse("Student not found", 404);

            var studentDetailDto = mapper.Map<StudentDetailsDTO>(result);
            return ApiResponse<StudentDetailsDTO>.SuccessResponse("Student found", 200, studentDetailDto);
        }

        #endregion

       

        #region GetAllStudents
        public async Task<ApiResponse<IEnumerable<StudentReadDTO>>> getStudentsAsync()
        {
            var result = await unitOfWork.Student.GetAsync(null, orderBy: q => q.OrderBy(s => s.UserName), 1, 10, false);
            if (result == null || !result.Any())
            {
                return ApiResponse<IEnumerable<StudentReadDTO>>.ErrorResponse("No students found", 404);
            }

            var studentReadDtos = mapper.Map<IEnumerable<StudentReadDTO>>(result);

            return ApiResponse<IEnumerable<StudentReadDTO>>.SuccessResponse("Students retrieved successfully", 200, studentReadDtos);
        }
        public Task<IEnumerable<StudentReadDTO>> getStudentsAsync(int page, int pagesize)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region DeleteStudent
        public async Task<ApiResponse> SoftDeleteStudentAsync(string id)
        {
            if (id == null)
            {
                return ApiResponse.ErrorResponse("Invalid student data", 400);
            }

            var std = await unitOfWork.Student.GetFirstOrDefaultAsync(x => x.Id == id);
            if (std == null) return ApiResponse.ErrorResponse("Student not found", 404);

            await unitOfWork.Student.SoftDeleteAsync(std);
            await unitOfWork.Save();
            return ApiResponse.SuccessResponse("Student Is Soft Deleted", 200);


        }
        #endregion



    }
}
