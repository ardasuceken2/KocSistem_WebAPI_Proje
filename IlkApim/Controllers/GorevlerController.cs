using IlkApim;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[Authorize]
[ApiController]
[Route("api/gorevler")]
public class GorevlerController : ControllerBase
{
    private static List<string> gorevler = new List<string>
    {
        "C# çalış",
        "Git öğren"
    };

    [HttpGet]
    public IActionResult ListeleTumunu()
    {
        return Ok(gorevler);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult Ekle([FromBody] GorevEkleDto gelenForm)
    {
        gorevler.Add(gelenForm.YeniGorev);
        return Ok("görev eklendi: " + gelenForm.YeniGorev);
    }


    [Authorize(Roles = "Admin")]
    [Authorize]
    [HttpDelete("{index}")]
    public IActionResult Sil(int index)
    {
        if (index < 0 || index >= gorevler.Count)
        {
            return NotFound("böyle bir task yok");
        }

        string silinen = gorevler[index];
        gorevler.RemoveAt(index);

        return Ok("silindi: " + silinen);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut("{index}")]
    public IActionResult Guncelle(int index, string yeniBaslik)
    {
        if (index < 0 || index >= gorevler.Count)
        {
            return NotFound("böyle bi task yok");
        }

        gorevler[index] = yeniBaslik;
        return Ok("veri güncellendi: " + yeniBaslik);
    }
}