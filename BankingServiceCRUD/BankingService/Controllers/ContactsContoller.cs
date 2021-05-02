﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BankingService.Models.Contexts;
using BankingService.Models.Entities;
using BankingService.Services.Contracts;
using BankingService.Models.DTOs;
using BankingService.ViewModels;
using System.Net.Http;
using System.Net.Http.Json;

namespace BankingService.Controllers
{
    [Route("BankAccounts/{accountID}/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsService _contactsService;

        public ContactsController(IContactsService statementsService)
        {
            _contactsService = statementsService;
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> CreateContact(int accountID, Contact newContact)
        {
            var result = await _contactsService.CreateContact(accountID, newContact);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return CreatedAtAction("GetContact", new { accountID = accountID, contactID = result.Value.Id }, result.Value);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contact>>> GetContacts(int accountID)
        {
            var result = await _contactsService.GetAllContacts(accountID);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return Ok(result.Value);
        }

        [HttpGet("{contactID}")]
        public async Task<ActionResult<Contact>> GetContact(int accountID, int contactID)
        {
            var result = await _contactsService.GetContact(accountID, contactID);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return Ok(result.Value);
        }


        [HttpPut("{contactID}")]
        public async Task<IActionResult> PutContact(int accountID, int contactID, ContactDTO updatedContact)
        {
            var result = await _contactsService.UpdateContact(accountID, contactID, updatedContact);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return NoContent();
        }

        [HttpDelete("{contactID}")]
        public async Task<IActionResult> DeleteStatement(int accountID, int contactID)
        {
            var result = await _contactsService.DeleteContact(accountID, contactID);

            if (!result.Success)
            {
                return new JsonResult(result) { StatusCode = (int)result.StatusCode };
            }

            return NoContent();
        }
    }
}