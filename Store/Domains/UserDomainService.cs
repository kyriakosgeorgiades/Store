using AutoMapper;
using Store.Auth;
using Store.Domains.Interfaces.Users;
using Store.Entities;
using Store.IRepository;
using Store.Models.Users.Request;

namespace Store.Domains.Users
{
    public class UserDomainService : IUserDomainService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDomainService"/> class.
        /// </summary>
        /// <param name="userRepository">Repository responsible for user data operations.</param>
        /// <param name="configuration">App configuration, useful for authentication.</param>
        /// <param name="mapper">Mapper for converting data between DTOs and entities.</param>
        public UserDomainService(IUserRepository userRepository, IConfiguration configuration, IMapper mapper)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _mapper = mapper;
        }


        /// <summary>
        /// Authenticates a user based on provided login details.
        /// </summary>
        /// <param name="model">DTO containing user's login credentials.</param>
        /// <returns>Authenticated user entity if credentials are valid.</returns>
        public async Task<User> AuthenticateAsync(LoginModelRequest model)
        {
            var user = await _userRepository.GetUserByUserNameOrEmail(model.UserNameOrEmail);

            if (user == null || !Authentication.VerifyPassword(model.Password, user.Password))
            {
                throw new UnauthorizedAccessException("User not found or invalid credentials.");
            }

            return user;
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="model">DTO containing user registration details.</param>
        public async Task RegisterAsync(RegisterModelRequest model)
        {
            if (await _userRepository.UsernameExists(model.UserName) || await _userRepository.EmailExists(model.Email))
            {
                throw new InvalidOperationException("Username already exists or email is in use.");
            }

            model.Password = Authentication.HashPassword(model.Password);
            await _userRepository.RegisterUser(_mapper.Map<User>(model));
        }

        /// <summary>
        /// Validates a provided authentication token.
        /// </summary>
        /// <param name="token">The JWT token to validate.</param>
        /// <returns>True if the token is valid, otherwise false.</returns>
        public bool ValidateToken(string token)
        {
            return Authentication.ValidateJwtToken(token, _configuration);
        }
    }
}
