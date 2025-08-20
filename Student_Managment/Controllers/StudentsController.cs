using Microsoft.AspNetCore.Mvc;
using Student_Management.Data;
using Student_Managment.Models;


namespace Student_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly DataAccess _dataAccess;

        // Constructor to get dataaccesee .
        public StudentsController(DataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        // GET: api/students?name=... student with filters of name roll or email
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetStudents(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] string? rollNumber)
        {
            try
            {
                var students = _dataAccess.GetStudents(name, email, rollNumber);
                return Ok(students);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/students/5 - student single id
        [HttpGet("{id}")]
        public ActionResult<Student> GetStudentById(int id)
        {
            try
            {
                var student = _dataAccess.GetStudentById(id);
                if (student == null)
                {
                    return NotFound($"Student with ID {id} not found.");
                }
                return Ok(student);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // POST: api/students - new student
        [HttpPost]
        public ActionResult<Student> CreateStudent([FromBody] Student student)
        {
            try
            {
                // use the apicpntorller to check the validation
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _dataAccess.CreateStudent(student);
                return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // PUT: api/students/5 - Update an alreday studnet
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, [FromBody] Student student)
        {
            if (id != student.Id)
            {
                return BadRequest("ID in URL does not match student's ID.");
            }

            try
            {
                var existingStudent = _dataAccess.GetStudentById(id);
                if (existingStudent == null)
                {
                    return NotFound($"Student with ID {id} not found.");
                }

                _dataAccess.UpdateStudent(student);
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // DELETE: api/students/5 - Delete
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            try
            {
                var existingStudent = _dataAccess.GetStudentById(id);
                if (existingStudent == null)
                {
                    return NotFound($"Student with ID {id} not found.");
                }

                _dataAccess.DeleteStudent(id);
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }
    }
}