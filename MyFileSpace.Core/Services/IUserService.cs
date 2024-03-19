using MyFileSpace.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileSpace.Core.Services
{
    public interface IUserService
    {

        /// <summary>
        /// Logins a user in the application based on the provided information.
        /// </summary>
        /// <param name="userLogin">the user's credentials for authentication.</param>
        /// <returns>
        /// Returns a <see cref="string"/> object, which contains the the JWT token.
        /// </returns>
        Task<string> Login(AuthDTO userLogin);

        /// <summary>
        /// Register a user in the application with all the information for the person.
        /// </summary>
        /// <param name="userRegister">Contains all information required for the account,
        /// but also for the person that will be shown in the genealogy tree</param>
        /// <returns>
        /// Returns a <see cref="UserDetailsDTO"/> object, which contains all the information
        /// for the user.
        /// </returns>
        Task<UserDetailsDTO> Register(AuthDTO userRegister);

        /// <summary>
        /// Updated the user's password. The new password will be validated in order 
        /// to have a specific format. 
        /// </summary>
        /// <param name="updatePassword">Contains the user's username, old and new password.</param>
        /// <returns>
        /// Returns a <see cref="UserDetailsDTO"/> object, which contains all the information
        /// of the user.
        /// </returns>
        Task UpdatePassword(UpdatePasswordDTO updatePassword);

        /// <summary>
        /// Retrieves a specific user by its Guid.
        /// </summary>
        /// <param name="userId"> The Guid of the user that should be retrieved.</param>
        /// <returns>
        /// Returns an <see cref="UserDetailsDTO"/> object which contains all information about the 
        /// requested user.
        /// </returns>
        Task<UserDetailsDTO> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Retrieves a specific user by its username.
        /// </summary>
        /// <param name="username"> The username of the user that should be retrieved.</param>
        /// <returns>
        /// Returns an <see cref="UserDetailsDTO"/> object which contains all information about the 
        /// requested user.
        /// </returns>
        Task<UserDetailsDTO> GetUser(string username);

        /// <summary>
        /// Updates the information of the specified user.
        /// Only the authenticated user can change its own information.
        /// </summary>
        /// <param name="userId"> The Guid of the user to update.</param>
        /// <param name="user"> Is the <see cref="UserUpdateDTO"/> object which contains information
        /// to update about the user.</param>
        /// <returns>
        /// Returns the <see cref="UserUpdateDTO"/> object for the updated user.
        /// </returns>
        Task UpdateUser(Guid userId, UserUpdateDTO user);

        /// <summary>
        /// Checks if the username is available to use.
        /// </summary>
        /// <param name="username"> The username that should be checked.</param>
        /// <returns>
        /// Returns true if the username is available, or false if the username is already taken.
        /// </returns>
        Task<bool> CheckUsernameAvailable(string username);

        /// <summary>
        /// Checks if the email is available to use.
        /// </summary>
        /// <param name="email"> The email that should be checked.</param>
        /// <returns>
        /// Returns true if the email is available, or false if the username is already taken.
        /// </returns>
        Task<bool> CheckEmailAvailable(string email);
    }
}
