using InMemoryCaching_WebAPI.Data;
using InMemoryCaching_WebAPI.Models;
using InMemoryCaching_WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCaching_WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DriverController : ControllerBase
    {
        private const string DriversKey = "drivers";
        
        private readonly ICacheService _cacheService;
        private readonly ApiDbContext _dbContext;

        public DriverController(ICacheService cacheService, ApiDbContext dbContext)
        {
            _cacheService = cacheService;
            _dbContext = dbContext; 
        }

        [HttpGet("get-driver")]
        public async Task<IActionResult> GetDriver([FromQuery] int id)
        {
            if (TryGetCachedDrivers(out var cachedDrivers))
            {
                var cacheDriver = cachedDrivers.ToList().FirstOrDefault(d => d.Id == id);
                if (cacheDriver is not null)
                {
                    return Ok(cacheDriver);
                }

                return NotFound(id);
            }

            var dbDriver = await _dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == id);
            if (dbDriver is not null)
            {
                return Ok(dbDriver);
            }

            return NotFound(id);
        }
        
        [HttpGet("get-drivers")]
        public async Task<IActionResult> GetDrivers()
        {
            if (TryGetCachedDrivers(out var cacheDrivers))
            {
                return Ok(cacheDrivers);
            }

            var drivers = await _dbContext.Drivers.ToListAsync();
            var expiryTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<IEnumerable<Driver>>(DriversKey, drivers, expiryTime);

            return Ok(drivers);
        }

        [HttpPost("add-driver")]
        public async Task<IActionResult> AddDriver(Driver driverData)
        {
            await _dbContext.Drivers.AddAsync(driverData);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDriver), new { driverData.Id }, driverData);
        }

        private bool TryGetCachedDrivers(out IEnumerable<Driver> drivers)
        {
            drivers = _cacheService.GetData<IEnumerable<Driver>>(DriversKey);
            return drivers is not null && drivers.Any();
        }
    }
}