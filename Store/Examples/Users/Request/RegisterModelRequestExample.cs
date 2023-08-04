using Store.Models.Users.Request;
using Swashbuckle.AspNetCore.Filters;

namespace Store.Examples.Users.Request;

public class RegisterModelRequestExample : IExamplesProvider<RegisterModelRequest>
{
    public RegisterModelRequest GetExamples()
    {
        return new RegisterModelRequest
        {
            Email = "papaki@gmail.com",
            UserName = "User3",
            Password = "Password3",
        };
    }
}
