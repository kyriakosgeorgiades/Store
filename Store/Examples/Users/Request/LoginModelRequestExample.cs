using Store.Models.Users.Request;
using Swashbuckle.AspNetCore.Filters;

namespace Store.Examples.Users.Request;

public class LoginModelRequestExample: IExamplesProvider<LoginModelRequest>
{
    public LoginModelRequest GetExamples()
    {
        return new LoginModelRequest
        {
            UserNameOrEmail = "User1",
            Password = "Password1",
        };
    }
}
