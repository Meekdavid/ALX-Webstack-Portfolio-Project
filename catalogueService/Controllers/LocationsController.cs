﻿using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using catalogueService.Interfaces;
using catalogueService.Repositories;
using AutoMapper;
using catalogueService.Database;
using catalogueService.Models;
using catalogueService.requestETresponse;
using System;
using Microsoft.AspNetCore.Authorization;
using catalogueService.Database.DBContextFiles;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace catalogueService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Super Admin")]
    public class LocationsController : Controller
    {
        private readonly ILocation _locationRep;
        private readonly IMapper _mapper;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ILocation locationRep, IMapper mapper, ILogger<LocationsController> logger)
        {
            this._locationRep = locationRep;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet]
        [Route("Get All Locations")]
        public async Task<IActionResult> GetAllWalkDifficulties()
        {
            var (IsSucess, mbokoDomain) = await _locationRep.GetLocationAsync();
            if (!IsSucess)
            {
                return NotFound();
            }
            var mbokoDTO = _mapper.Map<IEnumerable<locationModel>>(mbokoDomain);
            _logger.LogInformation("Locations Successfully fetched");
            return Ok(mbokoDTO);
        }

        [HttpPost]
        [Route("Add a Location")]
        public async Task<IActionResult> AddLocationAsync([FromBody] locationRequest Mboko)
        {
            _logger.LogInformation($"Location collected to be added is {Mboko}");
            try
            {
                var domainLocation = _mapper.Map<location>(Mboko);

                // Pass details to Repositpory
                domainLocation = await _locationRep.AddLocationAsync(domainLocation);
                if (domainLocation == null)
                {
                    return NotFound();
                }
                // Convert back to DTO
                var locationDTO = _mapper.Map<locationModel>(domainLocation);
                _logger.LogInformation("Locations collected above where successfully added");
                return Ok(new Response { response = $"Location added successfully with location ID of {locationDTO.locationId}" });
            }
            catch (Exception oxg)
            {
                _logger.LogInformation(oxg.StackTrace, $"An Error occured: {oxg}");
                return BadRequest(oxg.Message);
            }
        }

        [HttpGet]
        [Route("Get Single location")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            _logger.LogInformation($"About fetching location with the ID {id}");
            var repoLocation = await _locationRep.GetByIdAsync(id);
            if (repoLocation == null)
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var locationDTO = _mapper.Map<locationModel>(repoLocation);
            _logger.LogInformation($"Records where successfully fetched for the ID: {id}");
            return Ok(locationDTO);
        }

        private users GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.Claims;
                return new users
                {
                    userName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    //userName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    firstName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    lastName = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Surname)?.Value,
                    role = userClaim.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value,
                };

            }
            return null;
        }
    }
}
