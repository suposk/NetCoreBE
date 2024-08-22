//global using Api;
global using Asp.Versioning;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using System.Linq.Expressions;
global using AutoMapper;
global using FluentValidation;
global using Carter;
global using System.Net;
global using System.Text;
global using Microsoft.AspNetCore.RateLimiting;
global using Serilog;
global using System.Text.Json;

//Shared
global using SharedKernel.Base;
//global using SharedCommon;
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
global using CommonCleanArch.Application.Search;
global using CommonCleanArch.Application.CustomExceptions;
global using CommonCleanArch.Domain.Events;
global using CommonCleanArch.Application.Services;
global using CommonCleanArch.Infrastructure;
global using CommonCleanArch.Infrastructure.ApiMiddleware;
global using CommonCleanArch.Infrastructure.Enums;
global using CommonCleanArch.Infrastructure.Persistence;

//Infrastructure
global using NetCoreBE.Infrastructure;
global using NetCoreBE.Infrastructure.Persistence;

//Application
global using NetCoreBE.Application;
global using NetCoreBE.Application.Interfaces;
global using NetCoreBE.Application.Tickets;
global using NetCoreBE.Application.OldTickets;
global using NetCoreBE.Application.OutboxDomaintEvents;

//Domain
global using NetCoreBE.Domain.Entities;
global using NetCoreBE.Domain.Events;