using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace vue_core_okta.Controllers
{
  [Route("api/[controller]")]
  public class ForecastController : Controller
  {
    private static string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chillyz", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    [HttpGet("[action]")]
    public IEnumerable<WeatherForecast> WeatherForecasts()
    {
      var rng = new Random();
      return Enumerable.Range(1, 5).Select(index => new WeatherForecast
      {
        DateFormatted = DateTime.Now.AddDays(index).ToString("d"),
        TemperatureC = rng.Next(-20, 55),
        Summary = Summaries[rng.Next(Summaries.Length)]
      });
    }

    public class WeatherForecast
    {
      public string DateFormatted { get; set; }
      public int TemperatureC { get; set; }
      public string Summary { get; set; }

      public int TemperatureF
      {
        get
        {
          return 32 + (int)(TemperatureC / 0.5556);
        }
      }
    }
  }

  [Route("api/[controller]")]
  public class TodoController : Controller
  {
    private static ConcurrentBag<Todo> todos = new ConcurrentBag<Todo> {
            new Todo { Id = Guid.NewGuid(), Description = "Learn Vue" }
        };

    [HttpGet()]
    public IEnumerable<Todo> GetTodos()
    {
      return todos.Where(t => !t.Done);
    }

    [HttpPost()]
    public Todo AddTodo([FromBody]Todo todo)
    {
      todo.Id = Guid.NewGuid();
      todo.Done = false;
      todos.Add(todo);
      return todo;
    }

    [HttpDelete("{id}")]
    public ActionResult CompleteTodo(Guid id)
    {
      var todo = todos.SingleOrDefault(t => t.Id == id);
      if (todo == null) return NotFound();

      todo.Done = true;
      return StatusCode(204);
    }
    public class Todo
    {
      public Guid Id { get; set; }
      public string Description { get; set; }
      public bool Done { get; set; }
    }
  }

  [Route("api/[controller]")]
  public class WeatherController : Controller
  {
    private static string[] cities = { "Sydney", "Melbourne" };

    [HttpGet()]
    public IEnumerable<string> Weather()        // name doesn't matter
    {
      return cities;
    }

    [HttpGet("{city}")]
    public async Task<IActionResult> Weather(string city)
    {
      using (var client = new HttpClient())
      {
        try
        {
          client.BaseAddress = new Uri("http://api.openweathermap.org");
          var response = await client.GetAsync($"/data/2.5/weather?q={city}&appid=bb2ea1ead7a3b5bc86bbc8667edbe88a&units=metric");
          response.EnsureSuccessStatusCode();

          var stringResult = await response.Content.ReadAsStringAsync();
          //   Console.WriteLine(stringResult);
          var rawWeather = JsonConvert.DeserializeObject<OpenWeatherResponse>(stringResult);
          return Ok(new
          {
            Temp = rawWeather.Main.Temp,
            Summary = string.Join(",", rawWeather.Weather.Select(x => x.Main)),
            City = rawWeather.Name
          });
        }
        catch (HttpRequestException httpRequestException)
        {
          return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
        }
      }
    }

    public class OpenWeatherResponse
    {
      public string Name { get; set; }

      public IEnumerable<WeatherDescription> Weather { get; set; }

      public Main Main { get; set; }
    }

    public class WeatherDescription
    {
      public string Main { get; set; }
      public string Description { get; set; }
    }

    public class Main
    {
      public string Temp { get; set; }
    }
  }

  [Route("api/[controller]")]
  public class WardController : Controller
  {
    private static ConcurrentBag<Ward> wards = new ConcurrentBag<Ward> {
        };

    [HttpGet()]
    public IEnumerable<Ward> Wards()        // name doesn't matter
    {
      UserCredential credential;
      string[] Scopes = { SheetsService.Scope.Spreadsheets };
      string ApplicationName = "Google Ward Data";

      using (var stream =
          new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
      {
        string credPath = System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.Personal);
        credPath = Path.Combine(credPath, ".credentials/sheets.googleapis.com-dotnet-quickstart.json");

        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.Load(stream).Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)).Result;
        Console.WriteLine("Credential file saved to: " + credPath);
      }

      // Create Google Sheets API service.
      var service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName,
      });


      // example data
      String spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
      String range = "Class Data!A2:E";
      SpreadsheetsResource.ValuesResource.GetRequest request =
            service.Spreadsheets.Values.Get(spreadsheetId, range);
      ValueRange response = request.Execute();
      IList<IList<Object>> values = response.Values;
      if (values != null && values.Count > 0)
      {
        Console.WriteLine("Titles");
        foreach (var row in values)
        {
            
          // Print columns A and E, which correspond to indices 0 and 4.
          if (row.Count > 4)
          {
            Ward w = new Ward { Id = row[0].ToString(), Name = row[4].ToString() };
            wards.Add(w);
            Console.WriteLine("{0}, {1}", row[0], row[4]);
          }
        }
      }
      else
      {
        Console.WriteLine("No data found.");
      }
      return wards;
    }
    public class Ward
    {
      public string Id { get; set; }
      public string Name { get; set; }
    }

    // [HttpGet("{id}")]
    // public async Task<IActionResult> Ward(string id)
    // {
    //   using (var client = new HttpClient())
    //   {
    //     try
    //     {
    //       client.BaseAddress = new Uri("http://api.openweathermap.org");
    //       var response = await client.GetAsync($"/data/2.5/weather?q={id}&appid=bb2ea1ead7a3b5bc86bbc8667edbe88a&units=metric");
    //       response.EnsureSuccessStatusCode();

    //       var stringResult = await response.Content.ReadAsStringAsync();
    //       return Ok(new
    //       {
    //         Temp = "23",
    //         Summary = "45",
    //         City = "Aqaba"
    //       });
    //     }
    //     catch (HttpRequestException httpRequestException)
    //     {
    //       return BadRequest($"Error getting weather from Google sheets: {httpRequestException.Message}");
    //     }
    //   }
    // }
  }
 }


// to write to a cell

//             String range2 = "Data!F5";  // update cell F5 
//             ValueRange valueRange = new ValueRange();
//             valueRange.MajorDimension = "COLUMNS";//"ROWS";//COLUMNS

//             var oblist = new List<object>() { "My Cell Text" };
//             valueRange.Values = new List<IList<object>> { oblist };

//             SpreadsheetsResource.ValuesResource.UpdateRequest update = service.Spreadsheets.Values.Update(valueRange, spreadsheetId, range2);
//             update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
//             UpdateValuesResponse result2 = update.Execute();
