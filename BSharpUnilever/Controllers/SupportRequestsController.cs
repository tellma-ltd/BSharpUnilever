using AutoMapper;
using BSharpUnilever.Controllers.Util;
using BSharpUnilever.Controllers.ViewModels;
using BSharpUnilever.Data;
using BSharpUnilever.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    [Authorize] // TODO: Make sure the user is active too
    public class SupportRequestsController : ControllerBase
    {
        private const int DEFAULT_PAGE_SIZE = 50;
        private const int MAX_PAGE_SIZE = 5000;

        private readonly BSharpContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;
        private readonly UserManager<User> _userManager;

        public SupportRequestsController(BSharpContext context, IMapper mapper, ILogger<UsersController> logger, UserManager<User> userManager)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportRequestVM>>> GetAll(int top = DEFAULT_PAGE_SIZE,
            int skip = 0, string orderby = nameof(SupportRequestVM.SerialNumber), bool desc = true, string search = null)
        {
            try
            {
                // First project the model to the view model using AutoMapper
                IQueryable<SupportRequest> query = _context.SupportRequests.AsNoTracking();

                // Apply row level security
                query = await ApplyRowLevelSecurityAsync(query);

                // Apply the searching
                if (!string.IsNullOrWhiteSpace(search))
                {
                    if (int.TryParse(search, out int serial))
                    {
                        // IF the user input is an int number => search by serial number
                        query = query.Where(e => e.SerialNumber == serial);
                    }
                    else if (DateTime.TryParse(search, out DateTime date))
                    {
                        // IF the user input is a valid date => search by date
                        query = query.Where(e => e.Date == date);
                    }
                    else
                    {
                        // ELSE search KAE and store names and comments
                        query = query.Where(e =>
                        e.AccountExecutive.FullName.Contains(search) ||
                        e.Store.Name.Contains(search) ||
                        e.Comment.Contains(search));
                    }
                }

                // Before ordering or paging, retrieve the total count
                int totalCount = query.Count();

                // Apply the ordering (If orderby does not exist the system just ignores it)
                if (!string.IsNullOrWhiteSpace(orderby) && typeof(SupportRequestVM).HasProperty(orderby))
                {
                    string key = orderby; // TODO get the mapped property from the entity
                    query = query.OrderBy(key, isDescending: desc);
                }

                // Apply the paging (Protect against DOS attacks by enforcing a maximum page size)
                top = Math.Min(top, MAX_PAGE_SIZE);
                query = query.Skip(skip).Take(top);

                // Load the data, transform it and wrap it in some metadata
                var memoryList = await query
                    .Include(e => e.AccountExecutive)
                    .Include(e => e.Manager)
                    .Include(e => e.Store)
                    .ToListAsync();

                var resultData = _mapper.Map<List<SupportRequest>, List<SupportRequestVM>>(memoryList);
                var result = new ListResultVM<SupportRequestVM>
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
        public async Task<ActionResult<SupportRequestVM>> Get(int id)
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

        [HttpPost]
        public async Task<ActionResult<SupportRequestVM>> Post(SupportRequestVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("There is something wrong with the request payload"); // TODO: Return friendlier validation errors
            }

            try
            {
                var user = await GetCurrentUserAsync();

                // TODO Validating

                if (model.Id == 0) // Insert logic
                {
                    // TODO Inserting logico

                    // Prepare the new record
                    SupportRequest newRecord = _mapper.Map<SupportRequestVM, SupportRequest>(model);
                    List<SupportRequestLineItem> lineItems = _mapper
                        .Map<List<SupportRequestLineItemVM>, List<SupportRequestLineItem>>(model.LineItems);
                    newRecord.LineItems = lineItems;

                    // Save it to the database, this automatically saves the line items too
                    _context.SupportRequests.Add(newRecord);
                    await _context.SaveChangesAsync();

                    // Map and return the newly created record in the same format as a GET request
                    var resultModel = await InternalGetAsync(newRecord.Id);
                    var resourceUrl = Url.Action(nameof(Get), nameof(SupportRequestsController)); // TODO: Fix this bug

                    return Created(resourceUrl, resultModel);
                }
                else // Update logic
                {
                    // Retrieve the record from the DB
                    IQueryable<SupportRequest> secureQuery = await ApplyRowLevelSecurityAsync(_context.SupportRequests);
                    SupportRequest dbRecord = await secureQuery.Include(e => e.LineItems).FirstOrDefaultAsync(e => e.Id == model.Id);
                    if (dbRecord == null)
                    {
                        return NotFound($"Could not find a record with id='{model.Id}'");
                    }

                    var originalModel = _mapper.Map<SupportRequest, SupportRequestVM>(dbRecord);

                    // TODO Updating logic

                    // Update the record and save changes
                    _mapper.Map(model, dbRecord); // TODO handle line items
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

        private Task ValidateAsync(SupportRequestVM model, User user)
        {
            // TODO
            throw new NotImplementedException();
        }

        private Task InsertingAsync(SupportRequestVM model, User user)
        {
            // TODO
            throw new NotImplementedException();
        }

        private Task UpdatingAsync(SupportRequestVM model, SupportRequestVM originalModel, User user)
        {
            // TODO
            throw new NotImplementedException();
        }


        // Helper method
        private async Task<User> GetCurrentUserAsync()
        {
            // The requirement is that Key account executives can only see their own requests
            string userName = User.UserName();
            return await _userManager.FindByNameAsync(userName);
        }

        // Helper Method
        private async Task<IQueryable<SupportRequest>> ApplyRowLevelSecurityAsync(IQueryable<SupportRequest> query)
        {
            var user = await GetCurrentUserAsync();
            if (user.Role == Roles.KAE)
            {
                query = query.Where(e => e.AccountExecutive.UserName == user.UserName);
            }

            return query;
        }

        // This method is reused in both Get(id) and Post(), an example of the DRY principal
        private async Task<SupportRequestVM> InternalGetAsync(int id)
        {
            // Retrieve the user and if missing return a 404
            var secureQuery = await ApplyRowLevelSecurityAsync(_context.SupportRequests.AsNoTracking());
            SupportRequest record = await secureQuery
                                            .Include(e => e.AccountExecutive)
                                            .Include(e => e.Manager)
                                            .Include(e => e.Store)
                                            .Include(e => e.LineItems)
                                            .ThenInclude(e => e.Product)
                                            .Include(e => e.StateChanges)
                                            .ThenInclude(e => e.User)
                                            .Include(e => e.GeneratedDocuments)
                                            .FirstOrDefaultAsync(e => e.Id == id);

            return _mapper.Map<SupportRequest, SupportRequestVM>(record);
        }
    }
}
