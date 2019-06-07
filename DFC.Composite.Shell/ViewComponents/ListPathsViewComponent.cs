﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DFC.Composite.Shell.Services.Paths;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

namespace DFC.Composite.Shell.ViewComponents
{
    public class ListPathsViewComponent : ViewComponent
    {
        private readonly ILogger<ListPathsViewComponent> _logger;
        private readonly IPathService _pathService;

        public ListPathsViewComponent(ILogger<ListPathsViewComponent> logger, IPathService pathService)
        {
            _logger = logger;
            _pathService = pathService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var vm = new ListPathsViewModel();

            try
            {
                var paths = await _pathService.GetPaths();

                vm.Paths = paths.Where(w => w.IsOnline);
            }
            catch (BrokenCircuitException ex)
            {
                var errorString = $"{nameof(ListPathsViewComponent)}: BrokenCircuit: {ex.Message}";

                _logger.LogError(ex, errorString);
            }
            catch (Exception ex)
            {
                var errorString = $"{nameof(ListPathsViewComponent)}: {ex.Message}";

                _logger.LogError(ex, errorString);

                ModelState.AddModelError(string.Empty, errorString);
            }

            return View(vm);
        }
    }
}
