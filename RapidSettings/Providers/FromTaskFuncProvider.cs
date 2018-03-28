﻿using RapidSettings.Exceptions;
using RapidSettings.Interfaces;
using System;
using System.Threading.Tasks;

namespace RapidSettings.Providers
{
    /// <summary>
    /// Customizable settings provider which provides its value asynchronously.
    /// </summary>
    public class FromTaskFuncProvider : IRawSettingsProviderAsync
    {
        private readonly Func<object, Task<object>> rawSettingResolvingTask;

        /// <summary>
        /// Initializes a new instance of <see cref="FromTaskFuncProvider"/> class.
        /// </summary>
        /// <param name="rawSettingResolvingTask">Func which will be used to get task which retrieves setting's raw value.</param>
        public FromTaskFuncProvider(Func<object, Task<object>> rawSettingResolvingTask)
        {
            this.rawSettingResolvingTask = rawSettingResolvingTask ?? throw new RapidSettingsException($"{nameof(rawSettingResolvingTask)} cannot be null!");
        }

        /// <summary>
        /// Gets task by provided <paramref name="key"/> which in turn should retrieve raw setting value.
        /// </summary>
        /// <param name="key">Key by which raw setting value should be retrieved.</param>
        /// <returns>Task which retrieves raw setting value.</returns>
        public Task<object> GetRawSettingAsync(object key)
        {
            return this.rawSettingResolvingTask.Invoke(key);
        }
    }
}
