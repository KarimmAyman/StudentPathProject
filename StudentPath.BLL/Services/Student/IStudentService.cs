using StudentPath.BLL.Dtoes;
using StudentPath.BLL.Dtoes.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentPath.BLL.Services.Student
{
    public interface IStudentService
    {
        public Task<ApiResponse<IEnumerable<StudentReadDTO>>> getStudentsAsync();
        public Task<IEnumerable<StudentReadDTO>> getStudentsAsync(int page, int pagesize);
        public Task<ApiResponse> UpdateStudentAsync(StudentUpdatedDTO studentDto);
        public Task<ApiResponse<StudentDetailsDTO>> getStudentDetilsAsync(string id);
        public Task<ApiResponse<StudentReadDTO>> getStudentAsync(string id);


        public Task<ApiResponse> SoftDeleteStudentAsync(string id);

        public Task<ApiResponse> CreateStudentAsync(StudentAddDTO student);


    }
}
