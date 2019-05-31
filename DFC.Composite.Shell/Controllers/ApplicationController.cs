﻿using DFC.Composite.Shell.Common;
using DFC.Composite.Shell.Models;
using DFC.Composite.Shell.Services.Application;
using DFC.Composite.Shell.Services.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;
using System;
using System.Threading.Tasks;

namespace DFC.Composite.Shell.Controllers
{
    public class ApplicationController : BaseController
    {
        private const string MainRenderViewName = "Application/RenderView";

        private readonly IApplicationService _applicationService;
        private readonly IMapper<ApplicationModel, PageViewModel> _mapper;
        private readonly ILogger<ApplicationController> _logger;

        public ApplicationController(IApplicationService applicationService, IMapper<ApplicationModel, PageViewModel> mapper, ILogger<ApplicationController> logger)
        {
            _applicationService = applicationService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Action(ActionGetRequestModel requestViewModel)
        {
            var vm = new PageViewModel();

            try
            {
                var application = await _applicationService.GetApplicationAsync(requestViewModel.Path);

                if (application == null)
                {
                    ModelState.AddModelError(string.Empty, string.Format(Messages.PathNotRegistered, requestViewModel.Path));
                }
                else
                {
                    vm = _mapper.Map(application);
                    await _applicationService.GetMarkupAsync(requestViewModel.Path, requestViewModel.Route, vm);
                }
            }
            catch (BrokenCircuitException ex)
            {
                _logger.LogError(ex, $"{nameof(BrokenCircuitException)} {ex.Message}");
                var errorString = $"{requestViewModel.Path}: BrokenCircuit: {ex.Message}";
                ModelState.AddModelError(string.Empty, errorString);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(Exception)} {ex.Message}");
                var errorString = $"{requestViewModel.Path}: {ex.Message}";
                ModelState.AddModelError(string.Empty, errorString);
            }

            return View(MainRenderViewName, vm);
        }

    }
}