﻿using API.Models;

namespace API.Services.Interfaces
{
	public interface IAuthService
	{
		Task<bool> Login(string username, string password);
		Task<UserModel> GetCurrentUser();
	}
}
