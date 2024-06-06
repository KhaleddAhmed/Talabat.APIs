using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications.Employee_Specs;

namespace Talabat.APIs.Controllers
{

	public class EmployeeController : BaseApiController
	{
		private readonly IGenericRepository<Employee> _empRepo;

		public EmployeeController(IGenericRepository<Employee> empRepo) 
		{
			_empRepo = empRepo;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
		{
			var spec = new EmployeeWithDepartmentSpecifications();
			var employees = await _empRepo.GetAllWithSpec(spec);

			return Ok(employees);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Employee>> GetEmployee(int id)
		{
			var spec = new EmployeeWithDepartmentSpecifications(id);

			var employee=_empRepo.GetWithSpec(spec);

			if(employee is null)
			  return NotFound(new ApiResponse(404));
			

			return Ok(employee);
		}

	}
}
