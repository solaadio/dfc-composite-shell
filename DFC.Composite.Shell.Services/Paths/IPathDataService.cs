﻿using DFC.Composite.Shell.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DFC.Composite.Shell.Services.Paths
{
    public interface IPathDataService
    {
        Task<PathModel> GetPath(string path);

        Task<IEnumerable<PathModel>> GetPaths();
    }
}