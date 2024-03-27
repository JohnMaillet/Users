using Microsoft.AspNetCore.Mvc;
using ServerlessAPI.Entities.Interface;
using ServerlessAPI.Repositories.Interface;
using ServerlessAPI.Models.Implementations;
using ServerlessAPI.Utilities;
using ServerlessAPI.Utilities.Types;

namespace ServerlessAPI.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> logger;
    private readonly IUserRepository userRepository;
    private readonly IUserEntity userEntity;
    private readonly IErrorMessages errorMessages;

    public UsersController(ILogger<UsersController> logger, IUserRepository userRepository, IUserEntity userEntity, IErrorMessages errorMessages)
    {
        Console.Write($"type of IUserRepository: {userRepository.GetType()}");
        this.logger = logger;
        this.userRepository = userRepository;
        this.userEntity = userEntity;
        this.errorMessages = errorMessages;
    }

    // GET api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<IUserEntity>>> Get([FromQuery] int limit = 10)
    {
        if (limit <= 0 || limit > 100) return BadRequest("The limit should been between [1-100]");

        return Ok(await userRepository.GetAllAsync(limit));
    }

    // GET api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<IUserEntity>> Get(string id)
    {
        var result = await userRepository.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    // POST api/users
    [HttpPost]
    public async Task<ActionResult<IUserEntity>> Post([FromBody] User user)
    {
        try
        {
            if (user == null) return ValidationProblem("Invalid input! User not informed");            
            var result = await userRepository.CreateAsync(user);

            if (result)
            {
                return CreatedAtAction(
                    nameof(Get),
                    user);
            }
            else
            {
                return BadRequest("Fail to persist");
            }
        } catch(Exception ex)
        {
            if( ex.Message == errorMessages.GetErrorMessage(ErrorCode.UserNameOrEmailExists))
            {
                return Conflict(errorMessages.GetErrorMessage(ErrorCode.UserNameOrEmailExists));
            }
            
            return Problem(errorMessages.GetErrorMessage(ErrorCode.UnknownError));
        }
    }

    // PUT api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(string id, [FromBody] IUserEntity user)
    {
        if (id == "" || user == null) return ValidationProblem("Invalid request payload");

        // Retrieve the user.
        var userRetrieved = await userRepository.GetByIdAsync(id);

        if (userRetrieved == null)
        {
            var errorMsg = $"Invalid input! No user found with id:{id}";
            return NotFound(errorMsg);
        }

        user.Id = userRetrieved.Id;

        await userRepository.UpdateAsync(user);
        return Ok();
    }

    // DELETE api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        if (id == "" | id == null) return ValidationProblem("Invalid request payload");

        var userRetrieved = await userRepository.GetByIdAsync(id);

        if (userRetrieved == null)
        {
            var errorMsg = $"Invalid input! No user found with id:{id}";
            return NotFound(errorMsg);
        }

        await userRepository.DeleteAsync(userRetrieved);
        return Ok();
    }
}
