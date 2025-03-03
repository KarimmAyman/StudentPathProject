using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentPath.BLL.Dtoes.Students;
using StudentPath.BLL.Services.Student;
using StudentPath.DAL.Data.Models;

namespace StudentPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase


    {

        #region Prop
        private readonly IStudentService studentService;
        private readonly IWebHostEnvironment webHostEnvironment;
        #endregion

        #region Ctor
        public StudentController(IStudentService studentService, IWebHostEnvironment webHostEnvironment)
        {
            this.studentService = studentService;
            this.webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region GetAllStudents
        [HttpGet("GetAllStudents")]
        public async Task<IActionResult> GetAll()
        {
            var result = await studentService.getStudentsAsync();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, Data = result.Data });
        }
        #endregion


        #region GetStudentById
        [HttpGet("ById/{id}")]
        public async Task<IActionResult> GetStudentById(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Message = "Id cannot be null or empty" });
            }
            var result = await studentService.getStudentAsync(id);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new { Message = result.Message, Data = result.Data });

        }
        #endregion



        #region CreateStudent

        [HttpPost("AddStudent")]

        public async Task<IActionResult> Add([FromForm] StudentAddDTO student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Invalid request data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors)
                              .Select(e => e.ErrorMessage)
                              .ToList()
                });
            }

            string imageUrl = null;
            if (student.ProfileImage != null && student.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(student.ProfileImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await student.ProfileImage.CopyToAsync(stream);
                }

                imageUrl = $"/Uploads/{uniqueFileName}";
            }
            else
            {

                imageUrl = student.Gender == GenderType.Male
                    ? "/Uploads/default/Male_Photo.jpg"
                    : "/Uploads/default/Female_Photo.jpg";
            }


            student.ImgUrl = imageUrl;

            var result = await studentService.CreateStudentAsync(student);
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, new
                {
                    Message = result.Message,
                    Data = student
                });
            }

            return StatusCode(result.StatusCode, new { Message = result.Message });
        }
        #endregion

        #region UpdateStudent
        [HttpPut("EditStudent/{id}")]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] StudentUpdatedDTO student)
        {

            if (id == null || id != student.Id)
            {
                return BadRequest(new { Message = "Invalid student ID" });
            }

            var existingStudent = await studentService.getStudentAsync(id);
            if (existingStudent == null)
            {
                return NotFound(new { Message = "Student not found" });
            }

            if (student.ProfileImage != null && student.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Uploads");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(student.ProfileImage.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await student.ProfileImage.CopyToAsync(stream);
                }

                if (!string.IsNullOrEmpty(existingStudent.Data.ImgUrl))
                {
                    string oldFilePath = Path.Combine(webHostEnvironment.WebRootPath, existingStudent.Data.ImgUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                student.ImgUrl = $"/Uploads/{uniqueFileName}";
            }
            else
            {
                student.ImgUrl = existingStudent.Data.ImgUrl;
            }


            var result = await studentService.UpdateStudentAsync(student);
            if (result.Success)
            {
                Response.Headers.Add("X-Message", result.Message);

                return NoContent();
            }
            else
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });

            }


        }
        #endregion


        #region DeleteStudent

        [HttpDelete("DeleteStudent/{id}")]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest(new { Message = "Id cannot be null or empty" });
            }

            var result = await studentService.SoftDeleteStudentAsync(id);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, new { Message = result.Message });
            }

            return Ok(new { Message = result.Message });

        }

        #endregion



    }
}
