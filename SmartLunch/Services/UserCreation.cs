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

                var httpClient = HttpClientFactory.CreateClient("UserManagementAPI");

                string email = userCreationDto.Email!;

                if (string.IsNullOrEmpty(email))
                {
                    throw new ArgumentException("Email cannot be null or empty", nameof(email));
                }

                var getResponse = await GetHttpResponseMessageAsync(httpClient, email);

                if (!getResponse.IsSuccessStatusCode)
                {
                    if (getResponse.StatusCode != HttpStatusCode.NotFound)
                    {
                        ThrowExceptionBasedOnResponse(
                            getResponse.StatusCode,
                            "Invalid email format"
                        );
                    }

                    var postResponse = await httpClient.PostAsJsonAsync(
                        $"api/userManagement",
                        userCreationDto
                    );

                    CheckStatusCode(postResponse);

                    var getResponseNewUser = await GetHttpResponseMessageAsync(httpClient, email);

                    UserDetailsDto detailsDto = await GetUserDetailsDtoAsync(getResponseNewUser);

                    var assignRoleResponse = await httpClient.PostAsJsonAsync(
                        $"api/userManagement/assignRole",
                        new UserRoleDto(detailsDto.Id, "NormalUser")
                    );

                    CheckStatusCode(assignRoleResponse, detailsDto);

                    await UpdateLastLoginDateAsync(httpClient, detailsDto);
                }

                else
                {
                    UserDetailsDto detailsDto = await GetUserDetailsDtoAsync(getResponse);

                    await UpdateLastLoginDateAsync(httpClient, detailsDto);
                }
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

        private static async Task<HttpResponseMessage> GetHttpResponseMessageAsync(
            HttpClient client,
            string email
        )
        {
            return await client.GetAsync($"api/userManagement/{email}");
        }

        private static async Task<UserDetailsDto> GetUserDetailsDtoAsync(
            HttpResponseMessage responseMessage
        )
        {
            UserDetailsDto detailsDto =
                await responseMessage.Content.ReadFromJsonAsync<UserDetailsDto>()
                ?? throw new InvalidOperationException(
                    $"Failed to deserialize user data from response"
                );

            return detailsDto;
        }

        private static async Task UpdateLastLoginDateAsync(
            HttpClient httpClient,
            UserDetailsDto detailsDto
        )
        {
            var putResponse = await httpClient.PutAsJsonAsync(
                $"api/userManagement/updateLoginDate/{detailsDto.Id}",
                // Using null to indicate no body content, as the method signature expects an id
                (object?)null
            );

            putResponse.EnsureSuccessStatusCode();
        }

        private static void CheckStatusCode(
            HttpResponseMessage responseMessage,
            // detailsDto is optional as it used only for assignRoleResponse
            UserDetailsDto detailsDto = null!
        )
        {
            if (!responseMessage.IsSuccessStatusCode)
            {
                ThrowExceptionBasedOnResponse(
                    responseMessage.StatusCode,
                    "Invalid role assignment data",
                    detailsDto
                );
            }
        }

        private static void ThrowExceptionBasedOnResponse(
            HttpStatusCode httpStatusCode,
            string badRequestMessage,
            UserDetailsDto dto = null!
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
