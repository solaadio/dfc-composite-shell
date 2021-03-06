﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DFC.Composite.Shell.Services.BaseUrl
{
    public interface IBaseUrlService
    {
        string GetBaseUrl(HttpRequest request, IUrlHelper urlHelper);
    }
}