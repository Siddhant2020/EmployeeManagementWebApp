using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementWebApp.Security
{
    public class CustomEmailConfirmationTokenProvider<TUser> 
        : DataProtectorTokenProvider<TUser> where TUser : class
    {
        //private readonly ILogger<DataProtectorTokenProvider<TUser>> _logger;

        public CustomEmailConfirmationTokenProvider(IDataProtectionProvider dataProtectionProvider,
            IOptions<CustomEmailConfirmationTokenProviderOptions> options, 
            ILogger<CustomEmailConfirmationTokenProvider<TUser>> logger) 
            : base(dataProtectionProvider, options, logger)
        {
            //_logger = logger;
        }
    }
}
