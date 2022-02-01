using JWTAuthTest.Entities;
using JWTAuthTest.Services;
using JWTAuthTest.Utils;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FurnitureController : ControllerBase
    {
        private readonly IFurnitureService _furnitureService;

        public FurnitureController(IFurnitureService furnitureService)
        {
            _furnitureService = furnitureService;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> PostFurniture(Furniture furniture)
        {
            await _furnitureService.CreateFurniture(furniture);
            return CreatedAtAction(nameof(GetFurniture), new { Id = furniture.Id }, furniture);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Furniture>>> GetFurniture()
        {
            var furnitures = await _furnitureService.GetAllFurniture();
            return Ok(furnitures);
        }
    }
}
