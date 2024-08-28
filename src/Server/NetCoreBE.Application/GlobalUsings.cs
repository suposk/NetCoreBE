//global using Application;
global using System.Net;
global using MediatR;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Storage;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.DependencyInjection;
global using System.ComponentModel.DataAnnotations;
global using System.Linq.Expressions;
global using AutoMapper;
global using FluentValidation;
global using System.ComponentModel.DataAnnotations.Schema;
global using Microsoft.Extensions.Configuration;
global using System.Text;
global using Microsoft.Extensions.Logging;

//Shared
global using SharedKernel.Base;
global using SharedCommon;
global using SharedCommon.Services;
global using SharedCommon.Helpers;

//Contracts
global using SharedContract.Common;
global using SharedContract.Types;
global using SharedContract.Dtos;

//CommonCleanArch
global using CommonCleanArch;
global using CommonCleanArch.Domain.Base;
global using CommonCleanArch.Application;
global using CommonCleanArch.Application.Helpers;
global using CommonCleanArch.Application.CustomExceptions;
global using CommonCleanArch.Application.Search;
global using CommonCleanArch.Domain.Events;
global using CommonCleanArch.Application.Services;

//Application
global using NetCoreBE.Application;
global using NetCoreBE.Application.Interfaces;
global using NetCoreBE.Application.Tickets;
global using NetCoreBE.Application.OldTickets;
global using NetCoreBE.Application.OutboxDomaintEvents;

//Domain
global using NetCoreBE.Domain.Entities;
global using NetCoreBE.Domain.Events;
