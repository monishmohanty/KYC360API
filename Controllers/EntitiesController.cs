using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

[ApiController]
[Route("[controller]")]
public class EntitiesController : ControllerBase
{
    private readonly EntityService _entityService;

    public EntitiesController(EntityService entityService)
    {
        _entityService = entityService;
    }

    // GET /entities
    [HttpGet]
    public ActionResult<IEnumerable<Entity>> GetEntities(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sortBy = "Id",
        [FromQuery] string sortOrder = "asc")
    {
        // Get all entities from the service
        var entities = _entityService.GetAllEntities();

        // Apply sorting
        entities = SortEntities(entities, sortBy, sortOrder);

        // Calculate total number of pages
        int totalItems = entities.Count();
        int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        // Apply pagination
        entities = entities.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        // Add pagination metadata to response headers
        Response.Headers.Add("X-Total-Items", totalItems.ToString());
        Response.Headers.Add("X-Total-Pages", totalPages.ToString());

        return Ok(entities);
    }

    private IEnumerable<Entity> SortEntities(IEnumerable<Entity> entities, string sortBy, string sortOrder)
    {
        // Default sorting by Id ascending if invalid sortBy parameter
        if (string.IsNullOrEmpty(sortBy) || typeof(Entity).GetProperty(sortBy) == null)
            sortBy = "Id";

        // Sort the entities based on sortBy and sortOrder parameters
        if (sortOrder.ToLower() == "desc")
            return entities.OrderByDescending(e => e.GetType().GetProperty(sortBy).GetValue(e, null));
        else
            return entities.OrderBy(e => e.GetType().GetProperty(sortBy).GetValue(e, null));
    }

    // GET /entities/{id}
    [HttpGet("{id}")]
    public ActionResult<Entity> GetEntity(string id)
    {
        var entity = _entityService.GetEntityById(id);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(entity);
    }

    // POST /entities
    [HttpPost]
    public ActionResult<Entity> CreateEntity(Entity entity)
    {
        _entityService.AddEntity(entity);
        return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
    }

    // PUT /entities/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateEntity(string id, Entity entity)
    {
        if (id != entity.Id)
        {
            return BadRequest();
        }
        _entityService.UpdateEntity(entity);
        return NoContent();
    }

    // DELETE /entities/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteEntity(string id)
    {
        _entityService.DeleteEntity(id);
        return NoContent();
    }

    // GET /entities/search?query={query}
    [HttpGet("search")]
    public ActionResult<IEnumerable<Entity>> SearchEntities([FromQuery] string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return BadRequest("Search query is required.");
        }
        
        var entities = _entityService.GetAllEntities().Where(e =>
        e.Names != null && e.Names.Any(n => 
            n.FirstName != null && n.FirstName.Contains(query) || 
            n.MiddleName != null && n.MiddleName.Contains(query) || 
            n.Surname != null && n.Surname.Contains(query)) ||
        e.Addresses != null && e.Addresses.Any(a => 
            a.AddressLine != null && a.AddressLine.Contains(query) || 
            a.City != null && a.City.Contains(query) || 
            a.Country != null && a.Country.Contains(query))
    );


        return Ok(entities);
    }

    // GET /entities/filter?gender={gender}&startDate={startDate}&endDate={endDate}&countries={countries}
    [HttpGet("filter")]
    public ActionResult<IEnumerable<Entity>> FilterEntities(
        [FromQuery] string gender,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] IEnumerable<string> countries)
    {
        var entities = _entityService.GetAllEntities();

        if (!string.IsNullOrEmpty(gender))
        {
            entities = entities.Where(e => e.Gender == gender);
        }

        if (startDate != null)
        {
            entities = entities.Where(e => e.Dates.Any(d => d.DateValue >= startDate));
        }

        if (endDate != null)
        {
            entities = entities.Where(e => e.Dates.Any(d => d.DateValue <= endDate));
        }

        if (countries != null && countries.Any())
        {
            entities = entities.Where(e => e.Addresses.Any(a => countries.Contains(a.Country)));
        }

        return Ok(entities);
    }
}
