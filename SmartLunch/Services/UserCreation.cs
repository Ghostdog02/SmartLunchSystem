using System.Net;
using Microsoft.EntityFrameworkCore;
using SmartLunch.Api.Dtos;

namespace SmartLunch.Services
{
    public class UserCreation(
        IServiceProvider serviceProvider,
        IHttpClientFactory httpClientFactory
    )
    {
        private IServiceProvider ServiceProvider { get; } = serviceProvider;

        public IHttpClientFactory HttpClientFactory { get; } = httpClientFactory;

        public async Task CreateUserIfNotExistingAsync(UserCreationDto userCreationDto)
        {
            try
            {
                using var scope = ServiceProvider.CreateScope();

                var httpClientFactory = HttpClientFactory.CreateClient("UserManagementAPI");

                string email = userCreationDto.Email!;

                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));
                }

                var getResponse = await httpClientFactory.GetAsync($"api/userManagement/{email}");

                if (!getResponse.IsSuccessStatusCode)
                {
                    if (getResponse.StatusCode != HttpStatusCode.NotFound)
                    {
                        ThrowExceptionBasedOnResponse(
                            getResponse.StatusCode,
                            "Invalid email format"
                        );
                    }

                    var postResponse = await httpClientFactory.PostAsJsonAsync(
                        $"api/userManagement",
                        userCreationDto
                    );

                    if (!postResponse.IsSuccessStatusCode)
                    {
                        ThrowExceptionBasedOnResponse(
                            postResponse.StatusCode,
                            "Invalid user data provided"
                        );
                    }

                    var getResponseNew = await httpClientFactory.GetAsync(
                        $"api/userManagement/{email}"
                    );

                    UserDetailsDto detailsDto =
                        await getResponseNew.Content.ReadFromJsonAsync<UserDetailsDto>()
                        ?? throw new InvalidOperationException(
                            $"Failed to deserialize user data from response"
                        );

                    var assignRoleResponse = await httpClientFactory.PostAsJsonAsync(
                        $"api/userManagement/assignRole",
                        new UserRoleDto(detailsDto.Id, "NormalUser")
                    );

                    if (!assignRoleResponse.IsSuccessStatusCode)
                    {
                        ThrowExceptionBasedOnResponse(
                            assignRoleResponse.StatusCode,
                            "Invalid role assignment data",
                            detailsDto
                        );
                    }

                    var putResponse = httpClientFactory.PutAsJsonAsync(
                        $"api/userManagement/updateLoginDate/{detailsDto.Id}",
                        // Using null to indicate no body content, as the method signature expects an id
                        (object?)null
                    );

                    putResponse.Result.EnsureSuccessStatusCode();
                }
                else
                {
                    UserDetailsDto detailsDto =
                        await getResponse.Content.ReadFromJsonAsync<UserDetailsDto>()
                        ?? throw new InvalidOperationException(
                            $"Failed to deserialize user data from response"
                        );

                    var putResponse = httpClientFactory.PutAsJsonAsync(
                        $"api/userManagement/updateLoginDate/{detailsDto.Id}",
                        // Using null to indicate no body content, as the method signature expects an id
                        (object?)null
                    );

                    putResponse.Result.EnsureSuccessStatusCode();
                }
                // await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("A database error occurred while saving data.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error has occurred", ex);
            }
        }

        private static void ThrowExceptionBasedOnResponse(
            HttpStatusCode httpStatusCode,
            string badRequestMessage,
            UserDetailsDto dto = null
        ) // It is used only for httpStatus not found case
        {
            throw httpStatusCode switch
            {
                HttpStatusCode.BadRequest => new ArgumentException(badRequestMessage),

                HttpStatusCode.Conflict => new InvalidOperationException("User already exists"),

                HttpStatusCode.NotFound => new KeyNotFoundException(
                    $"User with ID {dto.Id} not found"
                ),

                HttpStatusCode.Unauthorized => new UnauthorizedAccessException(
                    "Authentication required"
                ),

                _ => new HttpRequestException($"Request failed: {httpStatusCode}"),
            };
        }
    }
}
