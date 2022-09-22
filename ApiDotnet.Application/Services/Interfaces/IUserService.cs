using System;
using ApiDotnet.Application.DTOs;

namespace ApiDotnet.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResultService<dynamic>> GenerateTokenAsync(UserDTO userDTO);
    }
}