using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIRGA.Application.Interfaces.Base;

namespace SIRGA.API.Controllers.Base
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController<TDto, TResponse> : ControllerBase
        where TDto : class
        where TResponse : class
    {
        protected readonly IBaseServices<TDto, TResponse> _service;
        protected abstract string EntityRouteName { get; }

        protected BaseApiController(IBaseServices<TDto, TResponse> service)
        {
            _service = service;
        }

       
        [HttpGet("GetAll")]
        public virtual async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public virtual async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPost("Crear")]
        public virtual async Task<IActionResult> Create([FromBody] TDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(dto);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(
                nameof(GetById),
                new { id = GetEntityId(result.Data) },
                result);
        }

        [HttpPut("Actualizar/{id:int}")]
        public virtual async Task<IActionResult> Update(int id, [FromBody] TDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, dto);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("Eliminar/{id:int}")]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        protected virtual int GetEntityId(TResponse response)
        {
            var idProperty = response.GetType().GetProperty("Id");
            if (idProperty == null)
                throw new InvalidOperationException("Response type must have an 'Id' property");

            return (int)idProperty.GetValue(response);
        }
    }
}
