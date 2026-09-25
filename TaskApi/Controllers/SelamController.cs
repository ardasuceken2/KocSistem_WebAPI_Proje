using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/selam")]
public class SelamController : ControllerBase
{
    [HttpGet]
    public string Merhaba()
    {
        return "SELAM";
    }
}