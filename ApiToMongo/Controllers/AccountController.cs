using ApiToMongo.Models;
using DataLibrary;
using DataLibrary.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiToMongo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IDbContext _dbContex;
        public AccountController(IDbContext dbContext)
        {
            _dbContex = dbContext;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] RegistrationViewModel formdata)
        {
            // Will hold all the errors related to registration
            List<string> errorList = new List<string>();
            if (ModelState.IsValid)
            {
                if(formdata.Password ==formdata.ConfirmPassword)
                {
                    var entity = (_dbContex.LoadContext("Users") as UserEntity);
                    var filter = entity.CheckUserValidity<UserDetails>(formdata.Email, formdata.LoginId);
                    var record = await _dbContex.LoadRecordByFilterAsync("Users", filter);

                    if (record.Count>0)
                    { 
                        return BadRequest("Email or Password already Present");
                    }
                    else
                    {
                        var User = new UserDetails
                        {
                            Firstname = formdata.FirstName,
                            Lastname = formdata.LastName,
                            Email = formdata.Email,
                            Password = formdata.Password,
                            Contactnumber = formdata.ContactNumber,
                            LoginId = formdata.LoginId
                        };
                        await _dbContex.InsertRecordAsync("Users", User);

                        return Created("User created successfully", null);
                    }
                }
            }
            

            return BadRequest("One or more validation errors");
        }

        [HttpGet("users/all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var records = await _dbContex.LoadRecordsAsync<UserDetails>("Users");
            return Ok(records);
        }



        //[HttpGet("[action]")]
        //public async Task<IActionResult> PostDetails()
        //{
        //    var User = new UserDetails
        //    {
        //        Firstname = "Ashley",
        //        Lastname = "Torque",
        //        Email = "ashleytor@abcd.com",
        //        Password = "HelloWorld",
        //        Contactnumber = "1234567890",
        //        LoginId="AshleyTor@1234"
        //    };

        //    await _dbContex.InsertRecordAsync("Users", User);

        //    return Ok("User created successfully");
        //}

        //[HttpGet("[action]")]
        //public async Task<IActionResult> UpdateDetails()
        //{
        //    var User = new UserDetails
        //    {
        //        Id= new Guid("e0cbdac9-3d77-4fc6-8eda-f11dbb6a5acd"),
        //        Firstname = "Mathey",
        //        Lastname = "Lambornique",
        //        Email = "Mathey@abcd.com",
        //        Password = "HelloWorld",
        //        Contactnumber = "9087654321",
        //        LoginId = "MatheyLamb@1234"
        //    };

        //    await _dbContex.UpsertRecordAsync<UserDetails>("Users",User.Id, User);

        //    return Ok("User created successfully");
        //}

        //[HttpGet("[action]")]
        //public async Task<IActionResult> GetAllRecords()
        //{
        //    var records = await _dbContex.LoadRecordsAsync<UserDetails>("Users");

        //    return Ok(records);
        //}

        //[HttpGet("[action]/{id}")]
        //public async Task<IActionResult> GetRecordById([FromRoute] Guid id)
        //{
        //    var record = await _dbContex.LoadRecordByIdAsync<UserDetails>("Users", id);

        //    return Ok(record);
        //}

        //[HttpGet("[action]/{email}/{loginid}")]
        //public async Task<IActionResult> GetUserByEmailOrLoginId([FromRoute] String email,String loginid)
        //{
        //    var entity = (_dbContex.LoadContext("Users") as UserEntity);
        //    var filter=entity.CheckUserValidity<UserDetails>(email, loginid);

        //    var record = await _dbContex.LoadRecordByFilterAsync("Users", filter);

        //    return Ok(record);
        //}
    }
}
