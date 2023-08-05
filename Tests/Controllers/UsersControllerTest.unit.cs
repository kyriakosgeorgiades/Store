using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NUnit.Framework;
using Store;
using Store.Context;
using Store.Entities;
using Store.Models.Users.Request;
using Store.Models.Users.Response;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace Tests.Controllers;

public class UsersControllerTest : BaseController
{
    public UsersControllerTest() : base(new WebApplicationFactory<Program>().CreateClient()) { }

    [Test]
    public async Task Login_Returns_JwtToken_With_Valid_Credentials()
    {
        var loginRequest = new LoginModelRequest { UserNameOrEmail = "User1", Password = "Password1" };

        var response = await Client.PostAsync("/api/Users/Login", new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json"));

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonConvert.DeserializeObject<LoginModelResponse>(content);
        Assert.IsNotNull(loginResponse);
        Assert.IsNotEmpty(loginResponse.Jwt);
    }

    [Test]
    public async Task Register_Creates_User_And_Returns_Created()
    {
        var registerRequest = new RegisterModelRequest
        {
            UserName= GenerateRandomUsername(),
            Email= GenerateRandomEmail(),
            Password = "Password1!"
        };

        var response = await Client.PostAsync("/api/Users/Register", new StringContent(JsonConvert.SerializeObject(registerRequest), Encoding.UTF8, "application/json"));

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [Test]
    public async Task ValidateToken_Returns_Valid_With_Valid_Token()
    {
        // Assuming you have a method to get a valid JWT token like in your BooksControllerTest
        var token = await GetJwtToken();

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var response = await Client.GetAsync("/api/Users/ValidateToken");

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadAsStringAsync();
        StringAssert.Contains("Token is valid.", result);
    }

    // You can copy the GetJwtToken method here or make it a shared utility method for all tests
    private async Task<string> GetJwtToken()
    {
        var loginRequest = new LoginModelRequest { UserNameOrEmail = "User1", Password = "Password1" };
        var response = await Client.PostAsync("/api/Users/Login", new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json"));
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var tokenResponse = JsonConvert.DeserializeObject<LoginModelResponse>(content);
        return tokenResponse.Jwt;
    }

    private static readonly Random _random = new Random();

    public static string GenerateRandomUsername(int length = 8)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
                                    .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

    public static string GenerateRandomEmail()
    {
        string username = GenerateRandomUsername();
        string domain = "test.com";  // You can randomize this as well if needed
        return $"{username}@{domain}";
    }
}
