using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppSample.Services
{
    public interface ISettings
    {
        string GetSetting(string key);
    }
}
