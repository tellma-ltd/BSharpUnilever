using AutoMapper;
using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Controllers.ViewModels;
using BSharpUnilever.Data;
using BSharpUnilever.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BSharpUnilever.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private const int DEFAULT_PAGE_SIZE = 50;
        private const int MAX_PAGE_SIZE = 5000;

        private readonly BSharpContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public ProductsController(BSharpContext context, IMapper mapper, ILogger<UsersController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductVM>>> GetAll(int top = DEFAULT_PAGE_SIZE,
            int skip = 0, string orderby = nameof(ProductVM.Id), bool desc = false, string search = null)
        {
            try
            {
                // First project the model to the view model using AutoMapper
                IQueryable<Product> query = _context.Products.AsNoTracking();

                // Apply the searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(e => e.Description.Contains(search) || e.Barcode.Contains(search) || e.SapCode.Contains(search));
                }

                // Before ordering or paging, retrieve the total count
                int totalCount = query.Count();

                // Apply the ordering (If orderby does not exist the system just ignores it)
                if (!string.IsNullOrWhiteSpace(orderby) && typeof(ProductVM).HasProperty(orderby))
                {
                    string key = orderby; // TODO get the mapped property from the entity
                    query = query.OrderBy(key, isDescending: desc);
                }

                // Apply the paging (Protect against DOS attacks by enforcing a maximum page size)
                top = Math.Min(top, MAX_PAGE_SIZE);
                query = query.Skip(skip).Take(top);

                // Load the data, transform it and wrap it in some metadata
                var memoryList = await query.ToListAsync();
                var resultData = _mapper.Map<List<Product>, List<ProductVM>>(memoryList);
                var result = new ListResultVM<ProductVM>
                {
                    Skip = skip,
                    Top = resultData.Count,
                    OrderBy = orderby,
                    Desc = desc,
                    TotalCount = totalCount,
                    Data = resultData
                };

                // Finally return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductVM>> Get(int id)
        {
            try
            {
                // Retrieve the user and if missing return a 404
                var result = await InternalGetAsync(id);
                if (result == null)
                {
                    return NotFound($"Could not find a record with id='{id}'");
                }

                // All is good
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        // This method is reused in both Get(id) and Post(), DRY principal applied
        private async Task<ProductVM> InternalGetAsync(int id)
        {
            // Retrieve the user and if missing return a 404
            Product record = await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);

            // All is good
            return _mapper.Map<Product, ProductVM>(record);
        }

        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<ProductVM>> Post(ProductVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("There is something wrong with the request payload"); // TODO: Return friendlier validation errors
            }

            try
            {
                if (model.Id == 0) // Insert logic
                {
                    // Create and insert a new record
                    Product newRecord = _mapper.Map<ProductVM, Product>(model);
                    _context.Products.Add(newRecord);
                    await _context.SaveChangesAsync();

                    // Map and return the newly created record in the same format as a GET request
                    var resultModel = await InternalGetAsync(newRecord.Id);
                    var resourceUrl = Url.Action(nameof(Get), nameof(ProductsController)); // TODO: Fix this bug

                    return Created(resourceUrl, resultModel);
                }
                else // Update logic
                {
                    // Retrieve the record
                    Product dbRecord = await _context.Products.FirstOrDefaultAsync(e => e.Id == model.Id);
                    if (dbRecord == null)
                    {
                        return NotFound($"Could not find a record with id='{model.Id}'");
                    }

                    // Update the record and save changes
                    _mapper.Map(model, dbRecord);
                    await _context.SaveChangesAsync();

                    // Finally return the same result you would get with a GET request
                    var resultModel = await InternalGetAsync(model.Id);
                    return Ok(resultModel);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                // Fetch the record to delete
                var recordToDelete = await _context.Products.FirstOrDefaultAsync(e => e.Id == id);
                if (recordToDelete == null)
                {
                    return NotFound($"Could not find a record with an Id ='{id}'");
                }

                _context.Products.Remove(recordToDelete);
                await _context.SaveChangesAsync();

                // Return the result
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                return BadRequest(ex);
            }
        }
    }
}
