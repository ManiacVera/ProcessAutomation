using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProcessAutomation.Common.DTO.Constants;

[SkipTokenValidation]
[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly IMemoryCache memorycache;

    public TokenController(IMemoryCache memorycache)
    {
        this.memorycache = memorycache;
    }

    [HttpPost(AdapterConstants.TOKEN_REGISTER_ROUTE)]
    public IActionResult Register([FromBody] TokenRegisterDTO tokenRegister)
    {
        if (tokenRegister == null || string.IsNullOrWhiteSpace(tokenRegister.Token))
        {
            return BadRequest(new { message = "The token is mandatory." });
        }

        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(5));

        memorycache.Set(tokenRegister.Token, true, cacheOptions);

        return Ok(new { message = "Token successfully registered." });
    }
}

public class TokenRegisterDTO
{
    public string? Token { get; set; }
}