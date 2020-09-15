using System;
using System.Configuration;
using System.Linq;

namespace ConsoleAppSample.Services
{
    public class Settings : ISettings
    {
        public string GetSetting(string key)
        {
            if(!ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                throw new ApplicationException(string.Format("The key could not be found: {0}", key));
            }
            return ConfigurationManager.AppSettings[key];
        }
    }
}
